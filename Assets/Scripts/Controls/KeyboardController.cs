using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class KeyboardController : MonoBehaviour, IController
{
    private List<PianoNote> _notesDown = new List<PianoNote>();
    public List<PianoNote> NotesDown => _notesDown;

    private List<PianoNote> _notesUp = new List<PianoNote>();
    public List<PianoNote> NotesUp => _notesUp;

    private List<PianoNote> _notes = new List<PianoNote>();
    public List<PianoNote> Notes => _notes;

    private PianoNote _higherNote;
    public PianoNote HigherNote => _higherNote;

    private PianoNote _lowerNote;
    public PianoNote LowerNote => _lowerNote;


    private Dictionary<KeyCode, PianoNote> keys = new Dictionary<KeyCode, PianoNote>()
    {
        { KeyCode.A, PianoNote.C4 },
        { KeyCode.Alpha2, PianoNote.CSharp4 },
        { KeyCode.Z, PianoNote.D4 },
        { KeyCode.Alpha3, PianoNote.DSharp4 },
        { KeyCode.E, PianoNote.E4 },
        { KeyCode.R, PianoNote.F4 },
        { KeyCode.Alpha5, PianoNote.FSharp4 },
        { KeyCode.T, PianoNote.G4 },
        { KeyCode.Alpha6, PianoNote.GSharp4 },
        { KeyCode.Y, PianoNote.A5 },
        { KeyCode.Alpha7, PianoNote.ASharp5 },
        { KeyCode.U, PianoNote.B5 },
        { KeyCode.I, PianoNote.C5 },
        { KeyCode.Alpha9, PianoNote.CSharp5 },
        { KeyCode.O, PianoNote.D5 },
        { KeyCode.Alpha0, PianoNote.DSharp5 },
        { KeyCode.P, PianoNote.E5 }
    };

    public KeyboardController()
    {
        _higherNote = keys.Last().Value;
        _lowerNote = keys.First().Value;

        _higherNote = PianoNote.C8;
        _lowerNote = PianoNote.A0;
    }

    // Start is called before the first frame upd-ate
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _notesDown.Clear();
        _notesUp.Clear();
        _notes.Clear();

        foreach(var kvp in keys)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                _notesDown.Add(kvp.Value);
            }

            if (Input.GetKeyUp(kvp.Key))
            {
                _notesUp.Add(kvp.Value);
            }

            if (Input.GetKey(kvp.Key))
            {
                _notes.Add(kvp.Value);
            }
        }
    }
}
