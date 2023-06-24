using Assets;
using Assets.Scripts.Utils;
using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class MidiConfigurationHelper : ViewModelBase
{
    public delegate void ConfigurationEndedEventHandler(object sender, MidiConfigurationReturn e);
    public event ConfigurationEndedEventHandler ConfigurationEnded;

    private enum ConfigurationState { Initializing, Started, WaitingLowerNote, WaitingHigherNote, WaitingConfirm, Ended, Canceled }

    private ConfigurationState _currentState = ConfigurationState.Initializing;

    private PianoNote _lowerNote;
    private PianoNote _higherNote;

    private IController _controller;

    private bool _isCustomConfiguration = false;
    public bool IsCustomConfiguration
    {
        get => _isCustomConfiguration;
        set 
        {
            _isCustomConfiguration = value;
            OnPropertyChanged();
        }
    }

    private bool _isLowerNoteComplete;
    public bool IsLowerNoteComplete
    {
        get { return _isLowerNoteComplete; }
        set 
        { 
            _isLowerNoteComplete = value;
            OnPropertyChanged();
        }
    }

    private bool _isHigherNoteComplete;
    public bool IsHigherNoteComplete
    {
        get { return _isHigherNoteComplete; }
        set
        {
            _isHigherNoteComplete = value;
            OnPropertyChanged();
        }
    }

    private Color _lowerPanelColor;
    public Color LowerPanelColor
    {
        get { return _lowerPanelColor; }
        set
        {
            _lowerPanelColor = value;
            OnPropertyChanged();
        }
    }

    private Color _higherPanelColor;
    public Color HigherPanelColor
    {
        get { return _higherPanelColor; }
        set
        {
            _higherPanelColor = value;
            OnPropertyChanged();
        }
    }


    private void Awake()
    {
        ChangeState(ConfigurationState.Initializing);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiserNotifyPropertyChanged();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(IController controller)
    {
        _controller = controller;
        controller.NoteDown += Controller_NoteDown;

        ChangeState(ConfigurationState.Started);
    }

    private void Controller_NoteDown(object sender, NoteEventArgs e)
    {
        if (_currentState == ConfigurationState.WaitingLowerNote)
        {
            _lowerNote = e.Note;
            ChangeState(ConfigurationState.WaitingHigherNote);
        }
        else if (_currentState == ConfigurationState.WaitingHigherNote)
        {
            _higherNote = e.Note;
            ChangeState(ConfigurationState.WaitingConfirm);
        }
    }

    private void SetMode(MidiConfigurationType type)
    {
        if (type == MidiConfigurationType.Touches88)
        {
            ChangeState(ConfigurationState.WaitingConfirm);
        }
        else if (type == MidiConfigurationType.Touches61)
        {
            ChangeState(ConfigurationState.WaitingConfirm);
        }
        else if (type == MidiConfigurationType.Custom)
        {
            ChangeState(ConfigurationState.WaitingLowerNote);
        }
    }

    public void CancelConfiguration()
    {
        ChangeState(ConfigurationState.Canceled);
    }

    public void ConfirmConfiguration()
    {
        ChangeState(ConfigurationState.Ended);
    }

    public void Touches88_Click()
    {
        _higherNote = MusicHelper.HigherNote;
        _lowerNote = MusicHelper.LowerNote;
        SetMode(MidiConfigurationType.Touches88);
    }

    public void Touches61_Click()
    {
        _higherNote = MusicHelper.HigherNote_66Touches;
        _lowerNote = MusicHelper.LowerNote_66Touches;
        SetMode(MidiConfigurationType.Touches61);
    }

    public void Custom_Click()
    {
        SetMode(MidiConfigurationType.Custom);
    }

    private void ChangeState(ConfigurationState newState)
    {
        bool stateChanged = false;

        if (_currentState == ConfigurationState.Initializing && newState == ConfigurationState.Started)
        {
            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.Started && newState == ConfigurationState.WaitingLowerNote)
        {
            IsCustomConfiguration = true;

            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.WaitingLowerNote && newState == ConfigurationState.WaitingHigherNote)
        {
            IsLowerNoteComplete = true;
            LowerPanelColor = UIHelper.GetColorFromHEX("3BFF5B");

            Debug.Log("Lower note configured : " + _lowerNote);
            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.WaitingHigherNote && newState == ConfigurationState.WaitingConfirm)
        {
            IsHigherNoteComplete = true;
            HigherPanelColor = UIHelper.GetColorFromHEX("3BFF5B");

            Debug.Log("Higher note configured : " + _higherNote);

            stateChanged = true;
        }
        else if((_currentState == ConfigurationState.Started || _currentState == ConfigurationState.WaitingHigherNote) && newState == ConfigurationState.WaitingConfirm)
        {
            stateChanged = true;
        }
        else if(_currentState == ConfigurationState.WaitingConfirm && newState == ConfigurationState.Ended)
        {
            if (_lowerNote > _higherNote) // if the user reverse the two notes
            {
                var tmp = _lowerNote;
                _lowerNote = _higherNote;
                _higherNote = tmp;
            }

            Debug.Log("Configuration is complete. Lower note = " + _lowerNote + ", higher note = " + _higherNote);

            ConfigurationEnded?.Invoke(this, new MidiConfigurationReturn(true, _higherNote, _lowerNote));

            _controller.NoteDown -= Controller_NoteDown;

            stateChanged = true;
        }
        else if(newState == ConfigurationState.Canceled)
        {
            ConfigurationEnded?.Invoke(this, new MidiConfigurationReturn(false, PianoNote.C8, PianoNote.A0));

            _controller.NoteDown -= Controller_NoteDown;

            stateChanged = true;
        }

        if (stateChanged)
        {
            Debug.Log("Change state from " + _currentState + " to " + newState);
            _currentState = newState;
        }

        if(_currentState == ConfigurationState.Canceled || _currentState == ConfigurationState.Ended)
        {
            gameObject.SetActive(false);
        }
    }
}

public class MidiConfigurationReturn : EventArgs
{
    private bool _statusCode = false;
    public bool StatusCode => _statusCode;

    private PianoNote _higherNote;
    public PianoNote HigherNote => _higherNote;

    private PianoNote _lowerNote;
    public PianoNote LowerNote => _lowerNote;

    public MidiConfigurationReturn(bool result, PianoNote higherNote, PianoNote lowerNote)
    {
        _statusCode = result;
        _higherNote = higherNote;
        _lowerNote = lowerNote;
    }
}