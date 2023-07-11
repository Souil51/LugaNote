using Assets.Scripts.Utils;
using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MidiController : MonoBehaviour, IController
{
    private static int A0StartingMidiNote = 21; // A0 is commonly the note number 21 on MIDI (C3 is 60)

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

    private int _c4Offset = 0;
    public int C4Offset => _c4Offset;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;
    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<PianoNote> _notesWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesWithOffset => _notesWithOffset;

    private List<PianoNote> _notesDownWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<PianoNote> _notesUpWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesUpWithOffset => _notesUpWithOffset;

    public string Label 
    {
        get
        {
            if((int)HigherNote - (int)LowerNote + 1 == 88)
            {
                return string.Format(Strings.MENU_MIDI_61_TOUCHES);
            } 
            else if((int)HigherNote - (int)LowerNote + 1 == 61)
            {
                return string.Format(Strings.MENU_MIDI_61_TOUCHES);
            }
            else
            {
                return string.Format(Strings.MENU_MIDI_CUSTOM_TOUCHES, HigherNote - LowerNote, LowerNote, HigherNote);
            }
        }
    }

    public List<PianoNote> AvailableNotes => Enumerable.Range((int)LowerNote, (int)HigherNote - (int)LowerNote + 1).Select(x => (PianoNote)x).ToList();

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;

    private MidiConfigurationHelper _configurationHelper;

    public MidiController()
    {
        _higherNote = PianoNote.C8;
        _lowerNote = PianoNote.A0;

        // For MIDI keyboard with reduced note count, keyboard will be centered on C4
        var middleC = MusicHelper.GetMiddleCBetweenTwoNotes(HigherNote, LowerNote);
        _c4Offset = PianoNote.C4 - middleC;
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
    }

    // Update is called once per frame
    void Update()
    {
        _notesDown.Clear();
        _notesUp.Clear();
        _notes.Clear();

        for(int i = A0StartingMidiNote; i < A0StartingMidiNote + StaticResource.PIANO_KEY_COUNT; i++)
        {
            if (MidiMaster.GetKeyDown(i))
            {
                SoundManager.PlayNote((PianoNote)(i - A0StartingMidiNote));
                _notesDown.Add((PianoNote)(i - A0StartingMidiNote));
            }

            if (MidiMaster.GetKeyUp(i))
            {
                _notesUp.Add((PianoNote)(i - A0StartingMidiNote));
            }

            if (MidiMaster.GetKey(i) > 0)
            {
                _notes.Add((PianoNote)(i - A0StartingMidiNote));
            }
        }

        if (_notesDown.Count > 0)
            NoteDown?.Invoke(this, new NoteEventArgs(_notesDown[0]));
    }

    private void Config_ConfigurationEnded(object sender, MidiConfigurationReturn e)
    {
        _configurationHelper.ConfigurationEnded -= Config_ConfigurationEnded;

        if (e.StatusCode)
        {
            this._lowerNote = e.LowerNote;
            this._higherNote = e.HigherNote;
        }

        Configuration?.Invoke(this, new ConfigurationEventArgs(e.StatusCode));
    }

    public void Configure(Canvas canvas)
    {
        // GameObject test = new GameObject();
        var go = Instantiate(Resources.Load("ConfigurationPanel"), Vector3.zero, Quaternion.identity) as GameObject;
        go.transform.SetParent(canvas.transform);
        _configurationHelper = go.GetComponent<MidiConfigurationHelper>();
        _configurationHelper.ConfigurationEnded += Config_ConfigurationEnded;

        _configurationHelper.Initialize(this);
    }

    public void ShowControllerUI()
    {
        // Do nothing (show key pressed here ?)
    }

    public void HideControllerUI()
    {
        // Do nothing (show key pressed here ?)
    }
}
