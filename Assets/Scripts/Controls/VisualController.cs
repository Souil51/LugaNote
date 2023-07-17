using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.UI;

public class VisualController : MonoBehaviour, IController
{
    private List<PianoNote> _notesDown = new List<PianoNote>();
    public List<PianoNote> NotesDown => _notesDown;

    private List<PianoNote> _notesUp = new List<PianoNote>();
    public List<PianoNote> NotesUp => _notesUp;

    private List<PianoNote> _notes = new List<PianoNote>();
    public List<PianoNote> Notes => _notes;

    // VIsual controller will show only one octave
    public PianoNote HigherNote => PianoNote.C8;
    public PianoNote LowerNote => PianoNote.A0;

    private PianoNote HigherVisibleNote => PianoNote.B3;
    private PianoNote LowerVisibleNote => PianoNote.C3;

    public int C4Offset => 0;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;
    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<PianoNote> _notesWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesWithOffset => _notesWithOffset;

    private List<PianoNote> _notesDownWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<PianoNote> _notesUpWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesUpWithOffset => _notesUpWithOffset;

    public string Label => "Touch screen";

    public List<PianoNote> AvailableNotes => Enumerable.Range((int)LowerNote, (int)HigherNote - (int)LowerNote + 1).Select(x => (PianoNote)x).ToList();

    private bool _isControllerUIVisible;
    public bool IsControllerUIVisible => _isControllerUIVisible;

    public bool HasUI => true;

    public bool IsConfigurable => false;

    private GameObject _buttonCanvas;

    // These contains notes pressed based on Buttons callback on PointerDown and PointerUp
    // As this is note like Input.Key and button don't have "KeyDown" (PointerDown trigger each frame if button is pressed)
    // I use list to track which button is down or not
    private List<PianoNote> _lastUpdateButtonsNoteDown = new List<PianoNote>();
    private List<PianoNote> _buttonsNoteDown = new List<PianoNote>();

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;

    public void Configure(Canvas canvas)
    {
        Configuration?.Invoke(this, new ConfigurationEventArgs(true)); // no visual configuration yet
    }

    // Update is called once per frame
    void Update()
    {
        _notesDown.Clear();
        _notesUp.Clear();

        // Bases on button pressed list, track if a note is Down (= note changed from Up to Down this exact frame)
        foreach (var buttonNote in _buttonsNoteDown)
        {
            if (!_lastUpdateButtonsNoteDown.Contains(buttonNote))
            {
                SoundManager.PlayNote(buttonNote);
                _notesDown.Add(buttonNote);

                _notes.Add(buttonNote);// For note's holding, just add if Down and remove if Up
            }
        }

        // Bases on button pressed list, track if a note is Up (= note changed from Down to Up this exact frame)
        foreach (var buttonNote in _lastUpdateButtonsNoteDown)
        {
            if (!_buttonsNoteDown.Contains(buttonNote))
            {
                _notesUp.Add(buttonNote);

                _notes.Remove(buttonNote);// For note's holding, just add if Down and remove if Up
            }
        }

        UpdateNotesWithOffset();

        if (_notesDown.Count > 0)
            NoteDown?.Invoke(this, new NoteEventArgs(_notesDown[0]));

        _lastUpdateButtonsNoteDown = new List<PianoNote>(_buttonsNoteDown);
    }

    private void Awake()
    {
        UpdateNotesWithOffset();

        _buttonCanvas = GenerateButtons();
        HideControllerUI();
    }

    private void UpdateNotesWithOffset()
    {
        _notesWithOffset = new List<PianoNote>(Notes);
        _notesDownWithOffset = new List<PianoNote>(NotesDown);
        if (C4Offset != 0)
        {
            _notesWithOffset = _notesWithOffset.Select(x => x + C4Offset).ToList();
            _notesDownWithOffset = _notesDownWithOffset.Select(x => x + C4Offset).ToList();
        }
    }

    public void ShowControllerUI()
    {
        _isControllerUIVisible = true;
        _buttonCanvas.SetActive(true);
    }

    public void HideControllerUI()
    {
        _isControllerUIVisible = false;
        _buttonCanvas.SetActive(false);
    }

    /// <summary>
    /// Generate all UI buttons and their events
    /// </summary>
    private GameObject GenerateButtons()
    {
        // UI Canvas that will contains all buttons
        var goCanvas = Instantiate(Resources.Load(StaticResource.PREFAB_VISUAL_KEYBOARD)) as GameObject;
        goCanvas.transform.localPosition = Vector3.zero;
        goCanvas.transform.SetParent(transform);

        var btnPanel = goCanvas.transform.GetChild(0);
        btnPanel.gameObject.transform.localPosition += new Vector3(0, -25f, 0);

        // 2 prefabs to calculate the center position of the buttons
        var goTmpButton = Instantiate(Resources.Load("NoteButton")) as GameObject;
        var rectTmpButton = goTmpButton.GetComponent<RectTransform>();

        var goTmpSharp = Instantiate(Resources.Load("NoteButtonSharp")) as GameObject;
        var rectTmpSharp = goTmpSharp.GetComponent<RectTransform>();

        // Compute Y note position
        float canvasHeight = goCanvas.GetComponent<RectTransform>().sizeDelta.y;
        float yPosition = -(canvasHeight / 2f) + 25f;

        // Compute the X of the first note
        var naturalNotesCount = MusicHelper.NaturalNotes.Count(x => x >= LowerVisibleNote && x <= HigherVisibleNote);
        var sharpNotesCount = MusicHelper.SharpNotes.Count(x => x >= LowerVisibleNote && x <= HigherVisibleNote);

        float totalWidth = (rectTmpButton.sizeDelta.x * naturalNotesCount) + (rectTmpSharp.sizeDelta.x * sharpNotesCount);

        float spaceBetween = 5f;
        float currentX = -(totalWidth / 4) - ((naturalNotesCount + sharpNotesCount) * spaceBetween / 2);
        float startingY = -(rectTmpButton.sizeDelta.x / 4);
        
        // Destroy the 2 temp prefabs
        Destroy(goTmpButton);
        Destroy(goTmpSharp);

        for (int i = (int)LowerVisibleNote; i <= (int)HigherVisibleNote; i++)
        {
            var note = (PianoNote)i;

            bool altered = MusicHelper.SharpNotes.Contains(note);

            string prefabName = MusicHelper.NaturalNotes.Contains(note) ? StaticResource.PREFAB_NOTE_BUTTON : StaticResource.PREFAB_NOTE_BUTTON_SHARP;
            GameObject goButtonNote = Instantiate(Resources.Load(prefabName)) as GameObject;
            goButtonNote.transform.SetParent(btnPanel);

            var buttonTransform = goButtonNote.GetComponent<RectTransform>();
            buttonTransform.localPosition = new Vector3(currentX, startingY + (altered ? 20 : 0), 0f);

            // Get the size of the button
            var buttonWidth = buttonTransform.sizeDelta.x;

            // Update the text of the button
            var buttonTMP = goButtonNote.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonTMP.text = MusicHelper.GetNoteCommonName(note);

            // Add the button down and button up event here because UI Button only have Click event
            // But we want to know when button is up or held
            var buttonEventTrigger = goButtonNote.GetComponent<EventTrigger>();

            // PointerDown and PointerUp just fill list to track which button is pressed and which is not
            // PianoNote list used by the game are managed in the update method, like other controllers
            // It's possible to handle this in callbacks but easier to do like this others controllers
            var pointerUp = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerUp).FirstOrDefault();
            pointerUp.callback.AddListener((data) =>
            {
                _buttonsNoteDown.Remove(note);
            });

            var pointerDown = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerDown).FirstOrDefault();
            pointerDown.callback.AddListener((data) =>
            {
                if (!_buttonsNoteDown.Contains(note))
                {
                    _buttonsNoteDown.Add(note);
                }
            });

            currentX += buttonWidth / 2f + spaceBetween;
        }

        return goCanvas;
    }
}
