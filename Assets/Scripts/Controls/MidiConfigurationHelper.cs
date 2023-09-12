using Assets;
using Assets.Scripts.Utils;
using DataBinding.Core;
using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MidiConfigurationHelper : ViewModelBase, IInstantiableUIElement
{
    public delegate void ConfigurationEndedEventHandler(object sender, MidiConfigurationReturn e);
    public event ConfigurationEndedEventHandler ConfigurationEnded;

    public delegate void ConfigurationDestroyedEventHandler(object sender, GameObjectEventArgs e);
    public event ConfigurationDestroyedEventHandler ConfigurationDestroyed;

    private enum ConfigurationState { Initializing, Started, WaitingLowerNote, WaitingHigherNote, Ended, Canceled }

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

    private Color _confirmTextColor;
    public Color ConfirmTextColor
    {
        get { return _confirmTextColor; }
        set
        {
            _confirmTextColor = value;
            OnPropertyChanged();
        }
    }

    private bool _confirmInteractable;
    public bool ConfirmInteractable
    {
        get { return _confirmInteractable; }
        set
        {
            _confirmInteractable = value;
            OnPropertyChanged();
        }
    }

    private string _title = "MIDI device configuration";
    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    private string _titleLocalizeEntry = "";
    public string TitleLocalizeEntry
    {
        get { return _titleLocalizeEntry; }
        set
        {
            _titleLocalizeEntry = value;
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

    public void Initialize(IController controller, bool newDevice = false)
    {
        _controller = controller;
        controller.NoteDown += Controller_NoteDown;

        ChangeState(ConfigurationState.Started);

        if (newDevice)
        {
            Title = "New MIDI device connected, please choose your configuration";
            TitleLocalizeEntry = StaticResource.LOCALIZATION_MIDI_CONFIGURATION_NEW_TITLE;
        }
        else
        {
            Title = "MIDI device configuration";
            TitleLocalizeEntry = StaticResource.LOCALIZATION_MIDI_CONFIGURATION_TITLE;
        }
    }

    private void Controller_NoteDown(object sender, ControllerNoteEventArgs e)
    {
        if (_currentState == ConfigurationState.WaitingLowerNote)
        {
            _lowerNote = e.ControllerNote.Note;
            ChangeState(ConfigurationState.WaitingHigherNote);
        }
        else if (_currentState == ConfigurationState.WaitingHigherNote)
        {
            _higherNote = e.ControllerNote.Note;
            ChangeState(ConfigurationState.Ended);
        }
    }

    private void SetMode(MidiConfigurationType type)
    {
        if (type == MidiConfigurationType.Touches88)
        {
            ChangeState(ConfigurationState.Ended);
        }
        else if (type == MidiConfigurationType.Touches61)
        {
            ChangeState(ConfigurationState.Ended);
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
            LowerPanelColor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_DARKGREEN);

            Debug.Log("Lower note configured : " + _lowerNote);
            stateChanged = true;
        }
        else if (_currentState == ConfigurationState.WaitingHigherNote && newState == ConfigurationState.Ended)
        {
            Debug.Log("Higher note configured : " + _higherNote);

            IsHigherNoteComplete = true;
            HigherPanelColor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_DARKGREEN);

            ConfirmInteractable = true;
            ConfirmTextColor = Color.white;

            EndConfiguration();

            stateChanged = true;
        }
        else if((_currentState == ConfigurationState.Started || _currentState == ConfigurationState.WaitingHigherNote) && newState == ConfigurationState.Ended)
        {
            ConfirmInteractable = true;
            ConfirmTextColor = Color.white;

            EndConfiguration();

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
            // gameObject.SetActive(false);
            ConfigurationDestroyed?.Invoke(this, new GameObjectEventArgs(this.gameObject));
            Destroy(gameObject);
        }
    }

    private void EndConfiguration()
    {
        Debug.Log("Configuration is complete. Lower note = " + _lowerNote + ", higher note = " + _higherNote);

        if (_lowerNote > _higherNote) // if the user reverse the two notes
        {
            (_higherNote, _lowerNote) = (_lowerNote, _higherNote);
        }

        ConfigurationEnded?.Invoke(this, new MidiConfigurationReturn(true, _higherNote, _lowerNote));
        _controller.NoteDown -= Controller_NoteDown;
    }

    public void UI_ButtonClick()
    {
        SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_CLICK, 1f);
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