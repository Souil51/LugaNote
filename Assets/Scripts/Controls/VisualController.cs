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
    public PianoNote HigherNote => PianoNote.B4;
    public PianoNote LowerNote => PianoNote.C4;

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;

    public void Configure()
    {
        Configuration?.Invoke(this, new ConfigurationEventArgs(true)); // no visual configuration yet
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _notesDown.Clear();
        _notesUp.Clear();
    }

    private void Awake()
    {
        var goCanvas = Instantiate(Resources.Load("UICanvas")) as GameObject;
        goCanvas.transform.localPosition = Vector3.zero;
        goCanvas.transform.SetParent(transform);

        float noteCount = HigherNote - LowerNote;

        var goTmpButton = Instantiate(Resources.Load("NoteButton")) as GameObject;
        var rectTmpButton = goTmpButton.GetComponent<RectTransform>();

        var goTmpSharp = Instantiate(Resources.Load("NoteButtonSharp")) as GameObject;
        var rectTmpSharp = goTmpSharp.GetComponent<RectTransform>();

        float flatNoteWidth = rectTmpButton.sizeDelta.x;
        float sharpNoteWidth = rectTmpSharp.sizeDelta.x;

        float currentX = -1f * flatNoteWidth * (noteCount / 2f);

        for(int i = (int)LowerNote; i <= (int)HigherNote; i++)
        {
            var note = (PianoNote)i;

            GameObject goButtonNote = null;

            if (StaticResource.FlatNotes.Contains(note))
            {
                goButtonNote = Instantiate(Resources.Load(StaticResource.PREFAB_NOTE_BUTTON)) as GameObject;
            }
            else
            {
                goButtonNote = Instantiate(Resources.Load(StaticResource.PREFAB_NOTE_BUTTON_SHARP)) as GameObject;
            }

            var currentWidth = goButtonNote.GetComponent<RectTransform>().sizeDelta.x;

            goButtonNote.transform.SetParent(goCanvas.transform);
            goButtonNote.transform.localPosition = new Vector3(currentX, 0f, 0f);

            var tmp = goButtonNote.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tmp.text = note.ToString();

            var btn = goButtonNote.GetComponent<EventTrigger>();

            var pointerUp = btn.triggers.Where(x => x.eventID == EventTriggerType.PointerUp).FirstOrDefault();
            pointerUp.callback.AddListener((data) =>
            {
                NotesUp.Add(note);

                bool noteDown = Notes.Any(x => x == note);
                if (noteDown)
                    Notes.Remove(note);
            });

            var pointerDown = btn.triggers.Where(x => x.eventID == EventTriggerType.PointerDown).FirstOrDefault();
            pointerDown.callback.AddListener((data) =>
            {
                NotesDown.Add(note);
                Notes.Add(note);
            });

            currentX += currentWidth;
        }
    }
}
