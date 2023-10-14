using Assets.Scripts.Controls;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Utils;
using DataBinding.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualController : MonoBehaviour, IControllerWithUI
{
    public bool IsEnabled => true;

    private List<ControllerNote> _notesDown = new List<ControllerNote>();
    public List<ControllerNote> NotesDown => _notesDown;

    private List<ControllerNote> _notesUp = new List<ControllerNote>();
    public List<ControllerNote> NotesUp => _notesUp;

    private List<ControllerNote> _notes = new List<ControllerNote>();
    public List<ControllerNote> Notes => _notes;

    // VIsual controller will show only one octave
    public PianoNote HigherNote => PianoNote.C8;
    public PianoNote LowerNote => PianoNote.A0;

    private PianoNote HigherVisibleNote => PianoNote.B4;
    private PianoNote LowerVisibleNote => PianoNote.C4;

    public int C4Offset => 0;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;
    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<ControllerNote> _notesWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesWithOffset => _notesWithOffset;

    private List<ControllerNote> _notesDownWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<ControllerNote> _notesUpWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesUpWithOffset => _notesUpWithOffset;

    private string _label = "Touch screen";
    public string Label
    {
        get => _label;
        set
        {
            _label = value;
        }
    }
    public string LabelEntryName => "Touch screen";

    public List<PianoNote> AvailableNotes => Enumerable.Range((int)LowerVisibleNote, (int)HigherVisibleNote - (int)LowerVisibleNote + 1).Select(x => (PianoNote)x).ToList();

    private bool _isControllerUIVisible;
    public bool IsControllerUIVisible => _isControllerUIVisible;

    public bool HasUI => true;

    public bool IsConfigurable => false;

    public bool IsReplacementModeForced => true;

    private GameObject _buttonCanvas;

    public VisualControllerMode _mode = VisualControllerMode.Classic;
    public VisualControllerMode Mode => _mode;

    private GameObject _currentButtonsCanvas = null;

    private List<PianoNote> _notesList = new List<PianoNote>();
    private List<List<PianoNote>> _intervalsList = new List<List<PianoNote>>();
    private List<PianoChord> _chordsList = new List<PianoChord>();

    // These contains notes pressed based on Buttons callback on PointerDown and PointerUp
    // As this is note like Input.Key and button don't have "KeyDown" (PointerDown trigger each frame if button is pressed)
    // I use list to track which button is down or not
    private List<PianoNote> _lastUpdateButtonsNoteDown = new List<PianoNote>();
    private List<PianoNote> _buttonsNoteDown = new List<PianoNote>();

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;
    public event ConfigurationDestroyedEventHandled ConfigurationDestroyed;

    public GameObject Configure(bool newDevice = false)
    {
        Configuration?.Invoke(this, new ConfigurationEventArgs(true)); // no visual configuration yet
        ConfigurationDestroyed?.Invoke(this, new GameObjectEventArgs(null));

        return null;
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
                // SoundManager.PlayNote(buttonNote);
                _notesDown.Add(new ControllerNote(buttonNote, IsReplacementModeForced, ControllerType.Visual));

                _notes.Add(new ControllerNote(buttonNote, IsReplacementModeForced, ControllerType.Visual));// For note's holding, just add if Down and remove if Up
            }
        }

        // Bases on button pressed list, track if a note is Up (= note changed from Down to Up this exact frame)
        foreach (var buttonNote in _lastUpdateButtonsNoteDown)
        {
            if (!_buttonsNoteDown.Contains(buttonNote))
            {
                _notesUp.Add(new ControllerNote(buttonNote, IsReplacementModeForced, ControllerType.Visual));

                _notes.Remove(new ControllerNote(buttonNote, IsReplacementModeForced, ControllerType.Visual));// For note's holding, just add if Down and remove if Up
            }
        }

        UpdateNotesWithOffset();

        if (_notesDown.Count > 0)
            NoteDown?.Invoke(this, new ControllerNoteEventArgs(_notesDown[0]));

        _lastUpdateButtonsNoteDown = new List<PianoNote>(_buttonsNoteDown);

        if (Input.GetKeyDown(KeyCode.N))
        {
            // StartCoroutine(Co_Test());
        }
    }

    private void Awake()
    {
        UpdateNotesWithOffset();

        GenerateUI();

        HideControllerUI();
    }

    public void GenerateUI()
    {
        if (Mode == VisualControllerMode.Classic)
            _buttonCanvas = GenerateButtons();
        else if(Mode == VisualControllerMode.IntervalName)
            _buttonCanvas = GenerateButtonsIntervalsName();
        else
            _buttonCanvas = GenerateButtonsChordsName();
    }

    private void UpdateNotesWithOffset()
    {
        _notesWithOffset = new List<ControllerNote>(Notes);
        _notesDownWithOffset = new List<ControllerNote>(NotesDown);
        if (C4Offset != 0)
        {
            _notesWithOffset = _notesWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, IsReplacementModeForced, ControllerType.Visual)).ToList();
            _notesDownWithOffset = _notesDownWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, IsReplacementModeForced, ControllerType.Visual)).ToList();
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
        _notesList.Clear();

        if (_currentButtonsCanvas != null)
            Destroy(_currentButtonsCanvas);

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

            bool altered = MusicHelper.IsSharp(note);

            string prefabName = MusicHelper.IsNatural(note) ? StaticResource.PREFAB_NOTE_BUTTON : StaticResource.PREFAB_NOTE_BUTTON_SHARP;
            GameObject goButtonNote = Instantiate(Resources.Load(prefabName)) as GameObject;
            _notesList.Add(note);
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

        _currentButtonsCanvas = goCanvas;
        return _currentButtonsCanvas;
    }

    private GameObject GenerateButtonsIntervalsName()
    {
        _intervalsList.Clear();

        if (_currentButtonsCanvas != null)
            Destroy(_currentButtonsCanvas);

        // UI Canvas that will contains all buttons
        var goCanvas = Instantiate(Resources.Load(StaticResource.PREFAB_VISUAL_KEYBOARD)) as GameObject;
        goCanvas.transform.localPosition = Vector3.zero;
        goCanvas.transform.SetParent(transform);

        var btnPanel = goCanvas.transform.GetChild(0);
        btnPanel.gameObject.transform.localPosition += new Vector3(0, -25f, 0);

        // 2 prefabs to calculate the center position of the buttons
        var goTmpButton = Instantiate(Resources.Load("NoteButton")) as GameObject;
        var rectTmpButton = goTmpButton.GetComponent<RectTransform>();

        // Compute Y note position
        float canvasHeight = goCanvas.GetComponent<RectTransform>().sizeDelta.y;
        float yPosition = -(canvasHeight / 2f) + 25f;

        var intervalNames = Enum.GetValues(typeof(IntervalName));

        // Compute the X of the first note
        var buttonsCount = intervalNames.Length;

        float totalWidth = (rectTmpButton.sizeDelta.x * buttonsCount);

        float spaceBetween = 5f;
        float currentX = -(totalWidth / 4) - (buttonsCount * spaceBetween / 2);
        float startingY = -(rectTmpButton.sizeDelta.x / 4) + 10f;

        var startColor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_ULTRALIGHT_RED);
        float colorStep = 0.033f;

        // Destroy the 2 temp prefabs
        Destroy(goTmpButton);

        PianoNote baseNote = PianoNote.A0;

        int index = 0;
        foreach (IntervalName intervalName in intervalNames)
        {
            PianoNote intervalNote = (PianoNote)(index + 1);

            string prefabName = StaticResource.PREFAB_NOTE_BUTTON;
            GameObject goButtonNote = Instantiate(Resources.Load(prefabName)) as GameObject;
            _intervalsList.Add(new List<PianoNote>() { baseNote, intervalNote });
            goButtonNote.transform.SetParent(btnPanel);

            var buttonTransform = goButtonNote.GetComponent<RectTransform>();
            buttonTransform.localPosition = new Vector3(currentX, startingY, 0f);

            // Get the size of the button
            var buttonWidth = buttonTransform.sizeDelta.x;

            // Update the text of the button
            var buttonTMP = goButtonNote.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonTMP.text = Enums.GetEnumDescription(intervalName);

            // Add the button down and button up event here because UI Button only have Click event
            // But we want to know when button is up or held
            var buttonEventTrigger = goButtonNote.GetComponent<EventTrigger>();

            var image = goButtonNote.GetComponent<Image>();
            image.color = new Color(startColor.r, startColor.g - (index * colorStep), startColor.b - (index * colorStep));

            // PointerDown and PointerUp just fill list to track which button is pressed and which is not
            // PianoNote list used by the game are managed in the update method, like other controllers
            // It's possible to handle this in callbacks but easier to do like this others controllers

            // For interval we set note based on A0 (PianoNote = 0) and A0 + interval length
            var pointerUp = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerUp).FirstOrDefault();
            pointerUp.callback.AddListener((data) =>
            {
                _buttonsNoteDown.Clear(); // Clear buttons to be sure that we input only this interval

                //_buttonsNoteDown.Remove(baseNote);
                //_buttonsNoteDown.Remove(intervalNote);
            });

            var pointerDown = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerDown).FirstOrDefault();
            pointerDown.callback.AddListener((data) =>
            {
                _buttonsNoteDown.Clear(); // Clear buttons to be sure that we input only this interval

                if (!_buttonsNoteDown.Contains(baseNote))
                    _buttonsNoteDown.Add(baseNote);

                if (!_buttonsNoteDown.Contains(intervalNote))
                    _buttonsNoteDown.Add(intervalNote);
            });

            currentX += buttonWidth / 2f + spaceBetween;
            index++;
        }

        _currentButtonsCanvas = goCanvas;
        return _currentButtonsCanvas;
    }

    private GameObject GenerateButtonsChordsName()
    {
        _chordsList.Clear();

        if (_currentButtonsCanvas != null)
            Destroy(_currentButtonsCanvas);

        // UI Canvas that will contains all buttons
        var goCanvas = Instantiate(Resources.Load(StaticResource.PREFAB_VISUAL_KEYBOARD)) as GameObject;
        goCanvas.transform.localPosition = Vector3.zero;
        goCanvas.transform.SetParent(transform);

        var btnPanel = goCanvas.transform.GetChild(0);
        btnPanel.gameObject.transform.localPosition += new Vector3(0, -20, 0);

        // 2 prefabs to calculate the center position of the buttons
        var goTmpButton = Instantiate(Resources.Load(StaticResource.PREFAB_NOTE_BUTTON_SMALL)) as GameObject;
        var rectTmpButton = goTmpButton.GetComponent<RectTransform>();

        // Compute Y note position
        float canvasHeight = goCanvas.GetComponent<RectTransform>().sizeDelta.y;
        float yPosition = -(canvasHeight / 2f) + 35;

        var chordsNames = Enum.GetValues(typeof(SharpPianoNote));

        // Compute the X of the first note
        var halfButtonsCount = chordsNames.Length;

        float totalWidth = (rectTmpButton.sizeDelta.x * halfButtonsCount);

        float spaceBetween = 5f;
        float startingX = -(totalWidth / 4) - (halfButtonsCount * spaceBetween / 2);
        float startingY = -(rectTmpButton.sizeDelta.x / 4) + ((rectTmpButton.sizeDelta.y / 2) + spaceBetween);

        float currentX = startingX;
        float currentY = startingY;

        var colorMajor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_ULTRALIGHT_GREEN);
        var colorMinor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_ULTRALIGHT_BLUE);

        // Destroy the 2 temp prefabs
        Destroy(goTmpButton);

        for (int i = 0; i < 2; i++)
        {
            currentX = startingX;
            int index = 0;
            Tonality tonality = i == 0 ? Tonality.Major : Tonality.Minor;

            // Generating text for minor and major
            var tonalityGo = Instantiate(new GameObject());
            var tonalityText = tonalityGo.AddComponent<TextMeshProUGUI>();

            var canvasRenderer = tonalityGo.GetComponent<CanvasRenderer>();
            if(canvasRenderer == null)
                tonalityGo.AddComponent<CanvasRenderer>();

            tonalityText.text = tonality == Tonality.Major ? "Major" : "Minor";
            tonalityText.color = Color.black;
            tonalityText.verticalAlignment = VerticalAlignmentOptions.Middle;
            tonalityGo.transform.SetParent(btnPanel);
            tonalityGo.transform.localPosition = new Vector3(currentX - (rectTmpButton.sizeDelta.y / 6f), currentY + (rectTmpButton.sizeDelta.y / 4f), 0);
            

            foreach (SharpPianoNote intervalName in chordsNames)
            {
                PianoNote chordNote = (PianoNote)((int)intervalName);

                PianoChord chord;
                if(tonality == Tonality.Major)
                    chord = MusicHelper.GetMajorChords().Where(x => x.Tonality == tonality && x.BaseNote == chordNote && x.Inversion == Inversion.None).FirstOrDefault();
                else
                    chord = MusicHelper.GetMinorChords().Where(x => x.Tonality == tonality && x.BaseNote == chordNote && x.Inversion == Inversion.None).FirstOrDefault();

                string prefabName = StaticResource.PREFAB_NOTE_BUTTON_SMALL;
                GameObject goButtonNote = Instantiate(Resources.Load(prefabName)) as GameObject;
                _chordsList.Add(chord);
                goButtonNote.transform.SetParent(btnPanel);

                var buttonTransform = goButtonNote.GetComponent<RectTransform>();
                buttonTransform.localPosition = new Vector3(currentX, currentY, 0f);

                // Get the size of the button
                var buttonWidth = buttonTransform.sizeDelta.x;

                // Update the text of the button
                var buttonTMP = goButtonNote.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonTMP.text = Enums.GetEnumDescription(intervalName);

                // Add the button down and button up event here because UI Button only have Click event
                // But we want to know when button is up or held
                var buttonEventTrigger = goButtonNote.GetComponent<EventTrigger>();

                var image = goButtonNote.GetComponent<Image>();
                image.color = tonality == Tonality.Major ? colorMajor : colorMinor;
                // PointerDown and PointerUp just fill list to track which button is pressed and which is not
                // PianoNote list used by the game are managed in the update method, like other controllers
                // It's possible to handle this in callbacks but easier to do like this others controllers

                // For chords, we add all notes of the chord
                var pointerUp = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerUp).FirstOrDefault();
                pointerUp.callback.AddListener((data) =>
                {
                    _buttonsNoteDown.Clear(); // clear to be sure we input only one chord at a time

                    //foreach (var chordNote in chord.Notes)
                    //{
                    //    var noteToAdd = (PianoNote)((int)chordNote + (int)PianoNote.A3);
                    //    _buttonsNoteDown.Remove(noteToAdd);
                    //}
                });

                var pointerDown = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerDown).FirstOrDefault();
                pointerDown.callback.AddListener((data) =>
                {
                    _buttonsNoteDown.Clear(); // clear to be sure we input only one chord at a time

                    foreach (var chordNote in chord.Notes)
                    {
                        var noteToAdd = (PianoNote)((int)chordNote + (int)PianoNote.A3);
                        if (!_buttonsNoteDown.Contains(noteToAdd))
                            _buttonsNoteDown.Add(noteToAdd);
                    }
                });

                currentX += buttonWidth / 2f + spaceBetween;
                
                index++;
            }

            currentY -= ((rectTmpButton.sizeDelta.y / 2) + spaceBetween);
        }

        _currentButtonsCanvas = goCanvas;
        return _currentButtonsCanvas;
    }

    public void ChangeMode(VisualControllerMode mode)
    {
        _mode = mode;

        //Debug.Log("Change visual mode to " + mode);
    }

    public async Task UpdateLabel()
    {
        Label = await LocalizationHelper.GetStringAsync(StaticResource.LOCALIZATION_CONTROLLER_LABEL_VISUAL);
    }

    private IEnumerator Co_Test()
    {
        // Debug.Log("CLICK");

        if (Mode == VisualControllerMode.Classic)
        {
            for (int j = 0; j < _notesList.Count; j++)
            {
                for (int i = 0; i < 30; i++)
                {
                    PianoNote note = _notesList[j];

                    if (!_buttonsNoteDown.Contains(note))
                    {
                        _buttonsNoteDown.Add(note);
                    }

                    yield return new WaitForSecondsRealtime(0.1f);

                    _buttonsNoteDown.Remove(note);

                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }
        else if (Mode == VisualControllerMode.IntervalName)
        {
            List<PianoNote> notes = null;

            // Debug.Log("DEBUT TEST");
            for (int j = 0; j < _intervalsList.Count; j++)
            {
                for (int i = 0; i < 30; i++)
                {
                    notes = _intervalsList[j];

                    if (!_buttonsNoteDown.Contains(notes[0]))
                        _buttonsNoteDown.Add(notes[0]);

                    if (!_buttonsNoteDown.Contains(notes[1]))
                        _buttonsNoteDown.Add(notes[1]);

                    yield return new WaitForSecondsRealtime(0.1f);

                    _buttonsNoteDown.Remove(notes[0]);
                    _buttonsNoteDown.Remove(notes[1]);

                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
            // Debug.Log("FIN TEST");
        }
        else
        {
            PianoChord chord = null;

            // Debug.Log("DEBUT TEST");
            for (int j = 0; j < _chordsList.Count; j++)
            {
                for (int i = 0; i < 30; i++)
                {
                    chord = _chordsList[j];

                    foreach (var chordNote in chord.Notes)
                    {
                        var noteToAdd = (PianoNote)((int)chordNote + (int)PianoNote.A3);
                        if (!_buttonsNoteDown.Contains(noteToAdd))
                            _buttonsNoteDown.Add(noteToAdd);
                    }

                    yield return new WaitForSecondsRealtime(0.1f);

                    foreach (var chordNote in chord.Notes)
                    {
                        var noteToAdd = (PianoNote)((int)chordNote + (int)PianoNote.A3);
                        _buttonsNoteDown.Remove(noteToAdd);
                    }

                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
            // Debug.Log("FIN TEST");
        }
    }

    public void ResetInputs()
    {
        //_notesDown.Clear();
        //_notesUp.Clear();
    }
}
