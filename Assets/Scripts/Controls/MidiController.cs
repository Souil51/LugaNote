using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public void Configure()
    {
        GameObject test = new GameObject();
        var go = Instantiate(test, Vector3.zero, Quaternion.identity) as GameObject;
        _configurationHelper = go.AddComponent<MidiConfigurationHelper>();
        _configurationHelper.ConfigurationEnded += Config_ConfigurationEnded;

        _configurationHelper.Initialize(this);
    }
}
