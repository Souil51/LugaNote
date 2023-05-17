using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MidiConfigurationHelper : MonoBehaviour
{
    public delegate void ConfigurationEndedEventHandler(object sender, MidiConfigurationReturn e);
    public event ConfigurationEndedEventHandler ConfigurationEnded;

    private IController _controller;
    public IController Controller
    {
        get => _controller;
        set
        {
            _controller = value;

            _controller.NoteDown += _controller_NoteDown;
        }
    }

    private enum ConfigurationState { Started, WaitingLowerNote, WaitingHigherNote, Ended }

    private ConfigurationState _currentState = ConfigurationState.Started;

    private PianoNote _lowerNote;
    private PianoNote _higherNote;

    private void Awake()
    {
        ChangeState(ConfigurationState.WaitingLowerNote);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void _controller_NoteDown(object sender, NoteEventArgs e)
    {
        if (_currentState == ConfigurationState.WaitingLowerNote)
        {
            _lowerNote = e.Note;
            ChangeState(ConfigurationState.WaitingHigherNote);
        }else if(_currentState == ConfigurationState.WaitingHigherNote)
        {
            _higherNote = e.Note;
            ChangeState(ConfigurationState.Ended);
        }
    }

    private void ChangeState(ConfigurationState newState)
    {
        bool stateChanged = false;

        if(_currentState == ConfigurationState.Started && newState == ConfigurationState.WaitingLowerNote)
        {
            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.WaitingLowerNote && newState == ConfigurationState.WaitingHigherNote)
        {
            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.WaitingHigherNote && newState == ConfigurationState.Ended)
        {
            Debug.Log("Configuration is complete. Lower note = " + _lowerNote + ", higher note = " + _higherNote);

            ConfigurationEnded?.Invoke(this, new MidiConfigurationReturn(PianoNote.C8, PianoNote.A0));
            Destroy(gameObject); // Configuration is ended, destroy the object

            stateChanged = true;
        }

        if (stateChanged)
        {
            _currentState = newState;
        }
    }
}

public class MidiConfigurationReturn : EventArgs
{
    private PianoNote _higherNote;
    public PianoNote HigherNote => _higherNote;

    private PianoNote _lowerNote;
    public PianoNote LowerNote => _lowerNote;

    public MidiConfigurationReturn(PianoNote higherNote, PianoNote lowerNote)
    {
        _higherNote = higherNote;
        _lowerNote = lowerNote;
    }
}