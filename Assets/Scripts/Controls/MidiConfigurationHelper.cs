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

    private enum ConfigurationState { Initializing, Started, WaitingLowerNote, WaitingHigherNote, Ended, Canceled }

    private ConfigurationState _currentState = ConfigurationState.Initializing;

    private PianoNote _lowerNote;
    private PianoNote _higherNote;

    private MidiController _controller;

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

    public void Initialize(MidiController controller)
    {
        _controller = controller;
        controller.NoteDown += Controller_NoteDown;

        ChangeState(ConfigurationState.Started);
        ChangeState(ConfigurationState.WaitingLowerNote);
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
            ChangeState(ConfigurationState.Ended);
        }
    }

    private void CancelConfiguration()
    {
        ChangeState(ConfigurationState.Canceled);
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
            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.WaitingLowerNote && newState == ConfigurationState.WaitingHigherNote)
        {
            Debug.Log("Lower note configured : " + _lowerNote);
            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.WaitingHigherNote && newState == ConfigurationState.Ended)
        {
            Debug.Log("Higher note configured : " + _higherNote);

            if (_lowerNote > _higherNote) // if the user reverse the two notes
            {
                var tmp = _lowerNote;
                _lowerNote = _higherNote;
                _higherNote = tmp;
            }

            Debug.Log("Configuration is complete. Lower note = " + _lowerNote + ", higher note = " + _higherNote);

            ConfigurationEnded?.Invoke(this, new MidiConfigurationReturn(true, _higherNote, _lowerNote));

            _controller.NoteDown -= Controller_NoteDown;

            Destroy(gameObject); // Configuration is ended, destroy the object

            stateChanged = true;
        }
        else if(newState == ConfigurationState.Canceled)
        {
            ConfigurationEnded?.Invoke(this, new MidiConfigurationReturn(false, PianoNote.C8, PianoNote.A0));

            _controller.NoteDown -= Controller_NoteDown;

            Destroy(gameObject); // Configuration is ended, destroy the object
        }

        if (stateChanged)
        {
            _currentState = newState;
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