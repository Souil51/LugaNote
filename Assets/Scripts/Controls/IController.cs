using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public delegate void NoteDownEventHandler(object sender, NoteEventArgs e);
public delegate void ConfigurationEventHandled(object sender, ConfigurationEventArgs e);

/// <summary>
/// Used to manage Controllers of any types (midi, keyboard, UI...)
/// </summary>
public interface IController
{
    event NoteDownEventHandler NoteDown;
    event ConfigurationEventHandled Configuration;

    PianoNote HigherNote { get; }
    PianoNote LowerNote { get; }

    List<PianoNote> NotesDown { get; }
    List<PianoNote> NotesUp { get; }
    List<PianoNote> Notes { get; }

    int C4Offset { get; }

    void Configure();
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

public class ConfigurationEventArgs : EventArgs
{
    private bool _result;
    public bool Result => _result;


    public ConfigurationEventArgs(bool result)
    {
        this._result = result;
    }
}