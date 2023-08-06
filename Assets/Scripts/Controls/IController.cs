using Assets;
using Assets.Scripts.Game.Model;
using DataBinding.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public delegate void NoteDownEventHandler(object sender, ControllerNoteEventArgs e);
public delegate void ConfigurationEventHandled(object sender, ConfigurationEventArgs e);
public delegate void ConfigurationDestroyedEventHandled(object sender, GameObjectEventArgs e);

/// <summary>
/// Used to manage Controllers of any types (midi, keyboard, UI...)
/// </summary>
public interface IController
{
    event NoteDownEventHandler NoteDown;
    event ConfigurationEventHandled Configuration;
    event ConfigurationDestroyedEventHandled ConfigurationDestroyed;

    PianoNote HigherNote { get; }
    PianoNote LowerNote { get; }

    List<ControllerNote> NotesDown { get; }
    List<ControllerNote> NotesUp { get; }
    List<ControllerNote> Notes { get; }

    List<PianoNote> AvailableNotes { get; }

    int C4Offset { get; }

    public PianoNote HigherNoteWithOffset { get; }
    public PianoNote LowerNoteWithOffset { get; }
    public List<ControllerNote> NotesWithOffset { get; }
    public List<ControllerNote> NotesDownWithOffset { get; }
    public List<ControllerNote> NotesUpWithOffset { get; }

    public string Label { get; }

    public bool HasUI { get; }
    public bool IsControllerUIVisible { get; }
    public bool IsConfigurable { get; }

    public bool IsReplacementModeForced { get; }

    GameObject Configure();

    void ShowControllerUI();
    void HideControllerUI();
}

public class ControllerNoteEventArgs : EventArgs
{
    private ControllerNote _controllerNote;
    public ControllerNote ControllerNote => _controllerNote;

    public ControllerNoteEventArgs(ControllerNote note)
    {
        _controllerNote = note;
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