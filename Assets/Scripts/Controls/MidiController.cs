using MidiJack;
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
    }
}
