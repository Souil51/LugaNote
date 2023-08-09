using Assets.Scripts.Game.Model;
using DataBinding.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class KeyboardController : MonoBehaviour, IController
{
    private List<ControllerNote> _notesDown = new List<ControllerNote>();
    public List<ControllerNote> NotesDown => _notesDown;

    private List<ControllerNote> _notesUp = new List<ControllerNote>();
    public List<ControllerNote> NotesUp => _notesUp;

    private List<ControllerNote> _notes = new List<ControllerNote>();
    public List<ControllerNote> Notes => _notes;

    private PianoNote _higherNote;
    public PianoNote HigherNote => _higherNote;

    private PianoNote _lowerNote;
    public PianoNote LowerNote => _lowerNote;

    public int C4Offset => 0;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;
    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<ControllerNote> _notesWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesWithOffset => _notesWithOffset;

    private List<ControllerNote> _notesDownWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<ControllerNote> _notesUpWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesUpWithOffset => _notesUpWithOffset;

    public string Label => "Keyboard";

    public List<PianoNote> AvailableNotes => keys.Select(x => x.Value).ToList();

    public bool IsControllerUIVisible => true;

    public bool HasUI => false;

    public bool IsConfigurable => false;

    public bool IsReplacementModeForced => true;

    // Keyboard has only few notes

    private Dictionary<KeyCode, PianoNote> keys = new Dictionary<KeyCode, PianoNote>()
    {
        { KeyCode.A, PianoNote.C4 },
        { KeyCode.Alpha2, PianoNote.C4Sharp },
        { KeyCode.Z, PianoNote.D4 },
        { KeyCode.Alpha3, PianoNote.D4Sharp },
        { KeyCode.E, PianoNote.E4 },
        { KeyCode.R, PianoNote.F4 },
        { KeyCode.Alpha5, PianoNote.F4Sharp },
        { KeyCode.T, PianoNote.G4 },
        { KeyCode.Alpha6, PianoNote.G4Sharp },
        { KeyCode.Y, PianoNote.A4 },
        { KeyCode.Alpha7, PianoNote.A4Sharp },
        { KeyCode.U, PianoNote.B4 },
        { KeyCode.I, PianoNote.C5 },
        { KeyCode.Alpha9, PianoNote.C5Sharp },
        { KeyCode.O, PianoNote.D5 },
        { KeyCode.Alpha0, PianoNote.D5Sharp },
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
                _notesDown.Add(new ControllerNote(kvp.Value, IsReplacementModeForced));
            }

            if (Input.GetKeyUp(kvp.Key))
            {
                _notesUp.Add(new ControllerNote(kvp.Value, IsReplacementModeForced));
            }

            if (Input.GetKey(kvp.Key))
            {
                _notes.Add(new ControllerNote(kvp.Value, IsReplacementModeForced));
            }
        }

        UpdateNotesWithOffset();

        if (_notesDown.Count > 0)
            NoteDown?.Invoke(this, new ControllerNoteEventArgs(_notesDown[0]));
    }

    /*public void Configure()
    {
        Configuration?.Invoke(this, new ConfigurationEventArgs(true)); // no keyboard configuration yet
    }*/

    private void UpdateNotesWithOffset()
    {
        _notesWithOffset = new List<ControllerNote>(Notes);
        _notesDownWithOffset = new List<ControllerNote>(NotesDown);
        if (C4Offset != 0)
        {
            _notesWithOffset = _notesWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, IsReplacementModeForced)).ToList();
            _notesDownWithOffset = _notesDownWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, IsReplacementModeForced)).ToList();
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
        ConfigurationDestroyed?.Invoke(this, new GameObjectEventArgs(null));

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
