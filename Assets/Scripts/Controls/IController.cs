using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public delegate void NoteDownEventHandler(object sender, NoteEventArgs e);

public interface IController
{
    event NoteDownEventHandler NoteDown;

    PianoNote HigherNote { get; }
    PianoNote LowerNote { get; }

    List<PianoNote> NotesDown { get; }
    List<PianoNote> NotesUp { get; }
    List<PianoNote> Notes { get; }
}

public class NoteEventArgs : EventArgs
{
    private PianoNote _note;
    public PianoNote Note => _note;

    public NoteEventArgs(PianoNote note)
    {
        _note = note;
    }
}