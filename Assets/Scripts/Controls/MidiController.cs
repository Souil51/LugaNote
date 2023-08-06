using Assets.Scripts.Data;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Game.Save;
using Assets.Scripts.Utils;
using DataBinding.Core;
using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MidiController : MonoBehaviour, IController
{
    private static int A0StartingMidiNote = 21; // A0 is commonly the note number 21 on MIDI (C3 is 60)

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

    private int _c4Offset = 0;
    public int C4Offset => _c4Offset;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;
    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<ControllerNote> _notesWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesWithOffset => _notesWithOffset;

    private List<ControllerNote> _notesDownWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<ControllerNote> _notesUpWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesUpWithOffset => _notesUpWithOffset;

    public string Label 
    {
        get
        {
            if((int)HigherNote - (int)LowerNote + 1 == 88)
            {
                return string.Format(Strings.MENU_MIDI_88_TOUCHES);
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

    public bool IsControllerUIVisible => true;

    public bool HasUI => false;

    public bool IsConfigurable => true;

    public bool IsReplacementModeForced => false;

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;
    public event ConfigurationDestroyedEventHandled ConfigurationDestroyed;

    private MidiConfigurationHelper _configurationHelper;

    private List<ControllerType> _midiControllerTypeList = new List<ControllerType>()
    {
        ControllerType.MIDI,
        ControllerType.KeyboardAndMidi,
        ControllerType.KeyboardVisualMidi
    };

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
        // Read the save file to get last MIDI configuration
        Save save = SaveManager.Save;
        var controllerData = save.GetControllerData();
        if (controllerData != null && (_midiControllerTypeList.Contains(controllerData.ControllerType)))
        {
            _lowerNote = controllerData.MidiLowerNote;
            _higherNote = controllerData.MidiHigherNote;
        }

        _notesWithOffset = Notes;
        _notesDownWithOffset = NotesDown;
        if (C4Offset != 0)
        {
            _notesWithOffset = _notesWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, IsReplacementModeForced)).ToList();
            _notesDownWithOffset = _notesDownWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, IsReplacementModeForced)).ToList();
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
                // SoundManager.PlayNote((PianoNote)(i - A0StartingMidiNote));
                _notesDown.Add(new ControllerNote((PianoNote)(i - A0StartingMidiNote), IsReplacementModeForced));
            }

            if (MidiMaster.GetKeyUp(i))
            {
                _notesUp.Add(new ControllerNote((PianoNote)(i - A0StartingMidiNote), IsReplacementModeForced));
            }

            if (MidiMaster.GetKey(i) > 0)
            {
                _notes.Add(new ControllerNote((PianoNote)(i - A0StartingMidiNote), IsReplacementModeForced));
            }
        }

        if (_notesDown.Count > 0)
            NoteDown?.Invoke(this, new ControllerNoteEventArgs(_notesDown[0]));
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

    public GameObject Configure()
    {
        // GameObject test = new GameObject();
        var go = Instantiate(Resources.Load(StaticResource.PREFAB_MIDI_CONFIGURATION_PANEL), Vector3.zero, Quaternion.identity) as GameObject;
        // go.transform.SetParent(canvas.transform);
        _configurationHelper = go.GetComponent<MidiConfigurationHelper>();
        _configurationHelper.ConfigurationEnded += Config_ConfigurationEnded;
        _configurationHelper.ConfigurationDestroyed += _configurationHelper_ConfigurationDestroyed;

        _configurationHelper.Initialize(this);

        return go;
    }

    private void _configurationHelper_ConfigurationDestroyed(object sender, GameObjectEventArgs e)
    {
        _configurationHelper.ConfigurationDestroyed -= _configurationHelper_ConfigurationDestroyed;

        ConfigurationDestroyed?.Invoke(sender, e);
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
