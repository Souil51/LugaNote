using Assets;
using Assets.Scripts.Data;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Game.Save;
using Assets.Scripts.Utils;
using DataBinding.Core;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MenuViewModel : ViewModelBase
{
    [SerializeField] private InfoMessage Info;
    [SerializeField] private RadioButtonsGroupViewModel LevelButtons;
    [SerializeField] private RadioButtonsGroupViewModel ReplacementButtons;
    [SerializeField] private RadioButtonsGroupViewModel IntervalButtons;
    [SerializeField] private RadioButtonsGroupViewModel KeyButtons;
    [SerializeField] private ToggleSwitch AccidentalSwitch;
    [SerializeField] private ToggleSwitch InversionSwitch;
    [SerializeField] private ToggleSwitch GuessNameSwitch;
    [SerializeField] private ToggleSwitch ReplacementSwitch;

    public delegate void OpenMidiConfigurationEventHandler(object sender, EventArgs e);
    public event OpenMidiConfigurationEventHandler OpenMidiConfiguration;

    //public delegate void OpenScoresModeEventHandler(object sender, GameModeEventArgs e);
    //public event OpenScoresModeEventHandler OpenScores;

    //public delegate void CloseScoresModeEventHandler(object sender, EventArgs e);
    //public event CloseScoresModeEventHandler CloseScores;

    public delegate void SelectedLevelChangedEventHandler(object sender, LevelEventArgs e);
    public event SelectedLevelChangedEventHandler SelectedLevelChanged;

    public delegate void SelectedAccidentalsChangedEventHandler(object sender, BoolEventArgs e);
    public event SelectedAccidentalsChangedEventHandler SelectedAccidentalsChanged;

    public delegate void SelectedInversionChangedEventHandler(object sender, BoolEventArgs e);
    public event SelectedInversionChangedEventHandler SelectedInversionChanged;

    public delegate void SelectedGuessNameChangedEventHandler(object sender, BoolEventArgs e);
    public event SelectedGuessNameChangedEventHandler SelectedGuessNameChanged;

    public delegate void SelectedReplacementChangedEventHandler(object sender, BoolEventArgs e);
    public event SelectedReplacementChangedEventHandler SelectedReplacementChanged;

    public delegate void SelectedIntervalChangedEventHandler(object sender, IntervalEventArgs e);
    public event SelectedIntervalChangedEventHandler SelectedIntervalChanged;

    public delegate void SelectedKeyChangedEventHandler(object sender, GameModeTypeEventArgs e);
    public event SelectedKeyChangedEventHandler SelectedKeyChanged;

    private bool _isInfoVisible = false;
    public bool IsInfoVisible
    {
        get => _isInfoVisible;
        private set
        {
            _isInfoVisible = value;
            OnPropertyChanged();
        }
    }

    private string _infoText = string.Empty;
    public string InfoText
    {
        get => _infoText;
        private set
        {
            _infoText = value;
            OnPropertyChanged();
        }
    }

    private List<string> _infoText_Variables;
    public List<string> InfoText_Variables
    {
        get => _infoText_Variables;
        private set
        {
            _infoText_Variables = value;
            OnPropertyChanged();
        }
    }

    private string _controllerLabel = "MIDI controller (88 keys)";
    public string ControllerLabel
    {
        get => _controllerLabel;
        private set
        {
            _controllerLabel = value;
            OnPropertyChanged();
        }
    }

    private bool _isMidiConfigurationVisible;
    public bool IsMidiConfigurationVisible
    {
        get => _isMidiConfigurationVisible;
        private set
        {
            _isMidiConfigurationVisible = value;
            OnPropertyChanged();
        }
    }

    private bool _scorePanelVisible = false;
    public bool ScorePanelVisible
    {
        get => _scorePanelVisible;
        private set
        {
            _scorePanelVisible = value;
            OnPropertyChanged();
        }
    }

    private List<LeaderboardScore> _displayedScores;
    public List<LeaderboardScore> DisplayedScores
    {
        get => _displayedScores;
        private set
        {
            _displayedScores = value;
            OnPropertyChanged();
        }
    }

    private bool _isMIDIDeviceConnected;
    public bool IsMIDIDeviceConnected
    {
        get => _isMIDIDeviceConnected;
        private set
        {
            _isMIDIDeviceConnected = value;
            OnPropertyChanged();
        }
    }

    private bool _lessThan61Keys;
    public bool LessThan61Keys
    {
        get => _lessThan61Keys;
        private set
        {
            _lessThan61Keys = value;
            OnPropertyChanged();
        }
    }

    public bool IsInversionModeVisible => IntervalButtons.SelectedIndex == (int)IntervalMode.Chord;
    public bool IsGuessNameModeVisible => IntervalButtons.SelectedIndex != (int)IntervalMode.Note;
    public bool IsGuessNameIntervalVisible => IntervalButtons.SelectedIndex == (int)IntervalMode.Interval;
    public bool IsGuessNameChordVisible => IntervalButtons.SelectedIndex == (int)IntervalMode.Chord;

    private void Awake()
    {
        MenuController.Instance.PropertyChanged += Instance_PropertyChanged;
    }

    private void Start()
    {
        Info.Disappeared += Info_Disappeared;
        LevelButtons.SelectedButtonChanged += LevelsButton_SelectedButtonChanged;
        AccidentalSwitch.ValueChanged += AccidentalSwitch_ValueChanged;
        InversionSwitch.ValueChanged += InversionSwitch_ValueChanged;
        //ReplacementButtons.SelectedButtonChanged += ReplacementButtons_SelectedButtonChanged;
        ReplacementSwitch.ValueChanged += ReplacementSwtch_ValueChanged;
        IntervalButtons.SelectedButtonChanged += IntervalButtons_SelectedButtonChanged;
        KeyButtons.SelectedButtonChanged += KeyButtons_SelectedButtonChanged;
        GuessNameSwitch.ValueChanged += GuessNameSwitch_ValueChanged;

        OnPropertyChanged(nameof(IsInversionModeVisible));
        OnPropertyChanged(nameof(IsGuessNameModeVisible));
        OnPropertyChanged(nameof(IsGuessNameIntervalVisible));
        OnPropertyChanged(nameof(IsGuessNameChordVisible));

        UpdateMidiConnection();
        UpdateDisplayedScores();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    ShowInfo(StaticResource.LOCALIZATION_MENU_INFO_MIDI_CUSTOM_TOUCHES, 2f, "a", "b", "c");
        //}
    }

    private void KeyButtons_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedKeyChanged?.Invoke(sender, new GameModeTypeEventArgs((GameModeType)e.Value));
        UpdateDisplayedScores();
    }

    private void IntervalButtons_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedIntervalChanged?.Invoke(sender, new IntervalEventArgs((IntervalMode)e.Value));
        UpdateDisplayedScores();
        OnPropertyChanged(nameof(IsInversionModeVisible));
        OnPropertyChanged(nameof(IsGuessNameModeVisible));
        OnPropertyChanged(nameof(IsGuessNameIntervalVisible));
        OnPropertyChanged(nameof(IsGuessNameChordVisible));
    }

    private void ReplacementButtons_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedReplacementChanged?.Invoke(sender, new BoolEventArgs(e.Value != 0));
        UpdateDisplayedScores();
    }

    private void ReplacementSwtch_ValueChanged(object sender, BoolEventArgs e)
    {
        SelectedReplacementChanged?.Invoke(sender, e);
        UpdateDisplayedScores();
    }

    private void AccidentalsButtons_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedAccidentalsChanged?.Invoke(sender, new BoolEventArgs(e.Value != 0));
        UpdateDisplayedScores();
    }

    private void AccidentalSwitch_ValueChanged(object sender, BoolEventArgs e)
    {
        SelectedAccidentalsChanged?.Invoke(sender, e);
        UpdateDisplayedScores();
    }

    private void InversionSwitch_ValueChanged(object sender, BoolEventArgs e)
    {
        SelectedInversionChanged?.Invoke(sender, e);
        UpdateDisplayedScores();
    }

    private void LevelsButton_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedLevelChanged?.Invoke(sender, new LevelEventArgs((Level)e.Value));
        UpdateDisplayedScores();
    }

    private void GuessNameSwitch_ValueChanged(object sender, BoolEventArgs e)
    {
        SelectedGuessNameChanged?.Invoke(sender, e);
        UpdateDisplayedScores();
    }

    private void OnDisable()
    {
        Info.Disappeared -= Info_Disappeared;
        LevelButtons.SelectedButtonChanged -= LevelsButton_SelectedButtonChanged;
        AccidentalSwitch.ValueChanged -= AccidentalSwitch_ValueChanged;
        InversionSwitch.ValueChanged -= InversionSwitch_ValueChanged;
        //ReplacementButtons.SelectedButtonChanged -= ReplacementButtons_SelectedButtonChanged;
        ReplacementSwitch.ValueChanged -= ReplacementSwtch_ValueChanged;
        IntervalButtons.SelectedButtonChanged -= IntervalButtons_SelectedButtonChanged;
        KeyButtons.SelectedButtonChanged -= KeyButtons_SelectedButtonChanged;
        GuessNameSwitch.ValueChanged -= GuessNameSwitch_ValueChanged;
    }

    public void InitializeViewModel()
    {
        InitialiserNotifyPropertyChanged();

        IsMidiConfigurationVisible = true; // MenuController.Instance.IsMidiConfigurationVisible;

    }

    public void ShowScorePanel(List<LeaderboardScore> scores)
    {
        DisplayedScores = scores;
        ScorePanelVisible = true;
    }

    private void UpdateDisplayedScores()
    {
        var key = (GameModeType)KeyButtons.SelectedIndex;
        var interval = (IntervalMode)IntervalButtons.SelectedIndex;
        var level = (Level)LevelButtons.SelectedIndex;
        var accidentals = AccidentalSwitch.Value;
        var inversions = InversionSwitch.Value;
        var guessName = GuessNameSwitch.Value;

        DisplayedScores = GenerateLeaderboardScoreList(key, interval, level, guessName, accidentals, inversions);
    }

    private List<LeaderboardScore> GenerateLeaderboardScoreList(GameModeType gameModeType, IntervalMode intervalMode, Level level, bool guessName, bool withRandomAccidental, bool withInversion)
    {
        List<LeaderboardScore> leaderboard = new List<LeaderboardScore>();

        var gameModeData = SaveManager.Save.GetGameModeData(gameModeType, intervalMode, level, guessName, withRandomAccidental, withInversion);
        if(gameModeData != null)
        {
            var scores = gameModeData.Scores.OrderByDescending(x => x.Value).ToList();

            for (int i = 0; i < scores.Count; i++)
            {
                leaderboard.Add(new LeaderboardScore(i + 1, scores[i].Value, scores[i].Date, scores[i].UsedControllers));
            }
        }
        
        return leaderboard;
    }

    public void CallGameObjectCreated(object sender, GameObjectEventArgs args)
    {
        OnGameObjectCreated(sender, args);
    }

    public void CallGameObjectDestroyed(object sender, GameObjectEventArgs args)
    {
        OnGameObjectDestroyed(sender, args);
    }

    public void InitOptions(GameMode gameMode, bool replacementMode)
    {
        if(gameMode != null)
        {
            KeyButtons.SelectButton((int)gameMode.GameModeType);
            IntervalButtons.SelectButton((int)gameMode.IntervalMode);
            LevelButtons.SelectButton((int)gameMode.Level);
            AccidentalSwitch.InitialState(gameMode.WithRandomAccidental);
            InversionSwitch.InitialState(gameMode.WithInversion);
        }
        
        //ReplacementButtons.SelectButton(replacementMode ? 1 : 0);
        ReplacementSwitch.InitialState(replacementMode);
    }

    public void ShowInfo(string info, float duration = 2f, params string[] variables)
    {
        InfoText = info;

        InfoText_Variables = variables?.ToList();

        /*if (variables.Length > 0)
            InfoText_FirstVariable = variables[0];

        if (variables.Length > 1)
            InfoText_SecondVariable = variables[1];

        if (variables.Length > 2)
            InfoText_ThirdVariable = variables[2];*/

        IsInfoVisible = true;

        StartCoroutine(Co_Info(duration));
    }

    public void HideInfo()
    {
        Info.Disappear();
    }

    public void UpdateRecommendation()
    {
        // Disable replacement for less than 61 keys because we need at least 56 keys (and 61 keys midi devices will cover all notes)
        LessThan61Keys = MenuController.Instance.Controller.HigherNote - MenuController.Instance.Controller.LowerNote < 61 - 1;

        //if (ReplacementButtons.SelectedIndex == 0 && LessThan61Keys)
        //    ReplacementButtons.SelectButton(1);

        if (!ReplacementSwitch.Value && LessThan61Keys)
            ReplacementSwitch.InitialState(true);
    }

    public void UpdateMidiConnection()
    {
        IsMIDIDeviceConnected = MenuController.Instance.Controller.IsConfigurable && MenuController.Instance.IsMidiConnected();
    }

    private void Info_Disappeared(object sender, System.EventArgs e)
    {
        IsInfoVisible = false;
    }

    public void StartControllerConfiguration()
    {
        OpenMidiConfiguration?.Invoke(this, EventArgs.Empty);
    }

    private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(ControllerLabel))
        {
            ControllerLabel = MenuController.Instance.ControllerLabel;
        }
        else if(e.PropertyName == nameof(IsMidiConfigurationVisible))
        {
            IsMidiConfigurationVisible = true; // MenuController.Instance.IsMidiConfigurationVisible;
        }
    }

    private IEnumerator Co_Info(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideInfo();
    }
}

public class BoolEventArgs : EventArgs
{
    private bool _value;
    public bool Value => _value;

    public BoolEventArgs(bool value)
    {
        _value = value;
    }
}

public class IntEventArgs : EventArgs
{
    private int _value;
    public int Value => _value;

    public IntEventArgs(int value)
    {
        _value = value;
    }
}

public class IntervalEventArgs : EventArgs
{
    private IntervalMode _interval;
    public IntervalMode Interval => _interval;

    public IntervalEventArgs(IntervalMode interval)
    {
        _interval = interval;
    }
}

public class GameModeTypeEventArgs : EventArgs
{
    private GameModeType _gameModeType;
    public GameModeType GameModeType => _gameModeType;

    public GameModeTypeEventArgs(GameModeType gameModeType)
    {
        _gameModeType = gameModeType;
    }
}

public class LevelEventArgs : EventArgs
{
    private Level _level;
    public Level Level => _level;

    public LevelEventArgs(Level level)
    {
        _level = level;
    }
}

public class GameModeEventArgs : EventArgs
{
    private GameMode _gameMode;
    public GameMode GameMode => _gameMode;

    public GameModeEventArgs(GameMode gameMode)
    {
        _gameMode = gameMode;
    }
}