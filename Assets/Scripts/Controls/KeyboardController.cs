using System;
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

    public int C4Offset => 0;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;
    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<PianoNote> _notesWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesWithOffset => _notesWithOffset;

    private List<PianoNote> _notesDownWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<PianoNote> _notesUpWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesUpWithOffset => _notesUpWithOffset;

    public string Label => "Keyboard";

    public List<PianoNote> AvailableNotes => keys.Select(x => x.Value).ToList();

    public bool IsControllerUIVisible => true;

    public bool HasUI => false;

    public bool IsConfigurable => false;

    // Keyboard has only few notes

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
        { KeyCode.Y, PianoNote.A4 },
        { KeyCode.Alpha7, PianoNote.ASharp4 },
        { KeyCode.U, PianoNote.B4 },
        { KeyCode.I, PianoNote.C5 },
        { KeyCode.Alpha9, PianoNote.CSharp5 },
        { KeyCode.O, PianoNote.D5 },
        { KeyCode.Alpha0, PianoNote.DSharp5 },
        { KeyCode.P, PianoNote.E5 }
    };

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;
    public event ConfigurationDestroyedEventHandled ConfigurationDestroyed;

    public KeyboardController()
    {
        _higherNote = keys.Last().Value;
        _lowerNote = keys.First().Value;

        _higherNote = PianoNote.C8;
        _lowerNote = PianoNote.A0;
    }

    private void Awake()
    {
        UpdateNotesWithOffset();
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
                // SoundManager.PlayNote(kvp.Value);
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

        UpdateNotesWithOffset();

        if (_notesDown.Count > 0)
            NoteDown?.Invoke(this, new NoteEventArgs(_notesDown[0]));
    }

    /*public void Configure()
    {
        Configuration?.Invoke(this, new ConfigurationEventArgs(true)); // no keyboard configuration yet
    }*/

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

    private void Config_ConfigurationEnded(object sender, MidiConfigurationReturn e)
    {
        if (e.StatusCode)
        {
            this._lowerNote = e.LowerNote;
            this._higherNote = e.HigherNote;
        }

        Configuration?.Invoke(this, new ConfigurationEventArgs(e.StatusCode));
    }

    public GameObject Configure()
    {
        Configuration?.Invoke(this, new ConfigurationEventArgs(true)); // no visual configuration yet
        ConfigurationDestroyed?.Invoke(this, new Assets.GameObjectEventArgs(null));

        return null;
    }

    public void ShowControllerUI()
    {
        // Do nothing (qwerty / azerty handle here ?)
    }

    public void HideControllerUI()
    {
        // Do nothing (qwerty / azerty handle here ?)
    }
}
