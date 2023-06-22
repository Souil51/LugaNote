using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private PianoNote HigherVisibleNote => PianoNote.C4;
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

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;

    public void Configure()
    {
        Configuration?.Invoke(this, new ConfigurationEventArgs(true)); // no visual configuration yet
    }

    // Update is called once per frame
    void Update()
    {
        _notesDown.Clear();
        _notesUp.Clear();
    }

    private void Awake()
    {
        _notesWithOffset = Notes;
        _notesDownWithOffset = NotesDown;
        if (C4Offset != 0)
        {
            _notesWithOffset = _notesWithOffset.Select(x => x + C4Offset).ToList();
            _notesDownWithOffset = _notesDownWithOffset.Select(x => x + C4Offset).ToList();
        }

        GenerateButtons();
    }

    /// <summary>
    /// Generate all UI buttons and their events
    /// </summary>
    private void GenerateButtons()
    {
        // UI Canvas that will contains all buttons
        var goCanvas = Instantiate(Resources.Load("UICanvas")) as GameObject;
        goCanvas.transform.localPosition = Vector3.zero;
        goCanvas.transform.SetParent(transform);

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

        float currentX = -(totalWidth / 2);

        // Destroy the 2 temp prefabs
        Destroy(goTmpButton);
        Destroy(goTmpSharp);

        for (int i = (int)LowerVisibleNote; i <= (int)HigherVisibleNote; i++)
        {
            var note = (PianoNote)i;

            bool altered = MusicHelper.SharpNotes.Contains(note);

            string prefabName = MusicHelper.NaturalNotes.Contains(note) ? StaticResource.PREFAB_NOTE_BUTTON : StaticResource.PREFAB_NOTE_BUTTON_SHARP;
            GameObject goButtonNote = Instantiate(Resources.Load(prefabName)) as GameObject;
            goButtonNote.transform.SetParent(goCanvas.transform);
            goButtonNote.transform.localPosition = new Vector3(currentX, yPosition + (altered ? 20 : 0), 0f);

            // Get the size of the button
            var buttonWidth = goButtonNote.GetComponent<RectTransform>().sizeDelta.x;

            // Update the text of the button
            var buttonTMP = goButtonNote.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonTMP.text = MusicHelper.GetNoteCommonName(note);

            // Add the button down and button up event here because UI Button only have Click event
            // But we want to know when button is up or held
            var buttonEventTrigger = goButtonNote.GetComponent<EventTrigger>();

            var pointerUp = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerUp).FirstOrDefault();
            pointerUp.callback.AddListener((data) =>
            {
                NotesUp.Add(note);

                bool noteDown = Notes.Any(x => x == note);
                if (noteDown)
                    Notes.Remove(note);
            });

            var pointerDown = buttonEventTrigger.triggers.Where(x => x.eventID == EventTriggerType.PointerDown).FirstOrDefault();
            pointerDown.callback.AddListener((data) =>
            {
                NotesDown.Add(note);
                Notes.Add(note);

                NoteDown?.Invoke(this, new NoteEventArgs(note));
            });

            currentX += buttonWidth;
        }
    }
}
