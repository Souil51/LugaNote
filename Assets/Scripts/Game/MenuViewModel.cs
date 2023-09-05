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
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using static MenuViewModel;

public class MenuViewModel : ViewModelBase
{
    [SerializeField] private InfoMessage Info;
    [SerializeField] private RadioButtonsGroupViewModel LevelButtons;
    [SerializeField] private RadioButtonsGroupViewModel IntervalButtons;
    [SerializeField] private RadioButtonsGroupViewModel KeyButtons;
    [SerializeField] private ToggleSwitch AccidentalSwitch;
    [SerializeField] private ToggleSwitch InversionSwitch;
    [SerializeField] private ToggleSwitch GuessNameSwitch;
    [SerializeField] private ToggleSwitch ReplacementSwitch;

    public delegate void GenericEventHandler<T>(object sender, T e);

    public event GenericEventHandler<EventArgs> OpenMidiConfiguration;

    public event GenericEventHandler<GenericEventArgs<Level>> SelectedLevelChanged;

    public event GenericEventHandler<GenericEventArgs<bool>> SelectedAccidentalsChanged;

    public event GenericEventHandler<GenericEventArgs<bool>> SelectedInversionChanged;

    public event GenericEventHandler<GenericEventArgs<bool>> SelectedGuessNameChanged;
    
    public event GenericEventHandler<GenericEventArgs<bool>> SelectedReplacementChanged;

    public event GenericEventHandler<GenericEventArgs<IntervalMode>> SelectedIntervalChanged;

    public event GenericEventHandler<GenericEventArgs<GameModeType>> SelectedKeyChanged;

    private bool _isInfoVisible = false;
    public bool IsInfoVisible
    {
        get => _isInfoVisible;
        private set => SetProperty(ref _isInfoVisible, value);
    }

    private string _infoText = string.Empty;
    public string InfoText
    {
        get => _infoText;
        private set => SetProperty(ref _infoText, value);
    }

    private List<string> _infoText_Variables;
    public List<string> InfoText_Variables
    {
        get => _infoText_Variables;
        private set => SetProperty(ref _infoText_Variables, value);
    }

    private string _controllerLabel = "MIDI controller (88 keys)";
    public string ControllerLabel
    {
        get => _controllerLabel;
        private set => SetProperty(ref _controllerLabel, value);
    }

    private bool _isMidiConfigurationVisible;
    public bool IsMidiConfigurationVisible
    {
        get => _isMidiConfigurationVisible;
        private set => SetProperty(ref _isMidiConfigurationVisible, value);
    }

    private bool _scorePanelVisible = false;
    public bool ScorePanelVisible
    {
        get => _scorePanelVisible;
        private set => SetProperty(ref _scorePanelVisible, value);
    }

    private List<LeaderboardScore> _displayedScores;
    public List<LeaderboardScore> DisplayedScores
    {
        get => _displayedScores;
        private set => SetProperty(ref _displayedScores, value);
    }

    private bool _isMIDIDeviceConnected;
    public bool IsMIDIDeviceConnected
    {
        get => _isMIDIDeviceConnected;
        private set => SetProperty(ref _isMIDIDeviceConnected, value);
    }

    private bool _lessThan61Keys;
    public bool LessThan61Keys
    {
        get => _lessThan61Keys;
        private set => SetProperty(ref _lessThan61Keys, value);
    }

    public bool IsInversionModeVisible => IntervalButtons.SelectedIndex == (int)IntervalMode.Chord;
    public bool IsGuessNameModeVisible => IntervalButtons.SelectedIndex != (int)IntervalMode.Note;
    public bool IsGuessNameCompatibilityVisible => IntervalButtons.SelectedIndex != (int)IntervalMode.Note && IsMIDIDeviceConnected;
    public bool IsGuessNameIntervalVisible => IntervalButtons.SelectedIndex == (int)IntervalMode.Interval;
    public bool IsGuessNameChordVisible => IntervalButtons.SelectedIndex == (int)IntervalMode.Chord;

    public bool IsReplacementModeVisible => IsMIDIDeviceConnected;

    public List<string> InfoText_Variables1 { get => _infoText_Variables; set => _infoText_Variables = value; }

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

    private void IntervalButtons_SelectedButtonChanged(object sender, GenericEventArgs<int> e)
    {
        SelectedIntervalChanged?.Invoke(sender, new GenericEventArgs<IntervalMode>((IntervalMode)e.Value));
        UpdateDisplayedScores();
        OnPropertyChanged(nameof(IsInversionModeVisible));
        OnPropertyChanged(nameof(IsGuessNameModeVisible));
        OnPropertyChanged(nameof(IsGuessNameIntervalVisible));
        OnPropertyChanged(nameof(IsGuessNameChordVisible));
    }

    private void KeyButtons_SelectedButtonChanged(object sender, GenericEventArgs<int> e) => InvokeAndRefresh(sender, new GenericEventArgs<GameModeType>((GameModeType)e.Value), SelectedKeyChanged);

    private void ReplacementButtons_SelectedButtonChanged(object sender, GenericEventArgs<int> e) => InvokeAndRefresh(sender, new GenericEventArgs<bool>(e.Value != 0), SelectedReplacementChanged);

    private void ReplacementSwtch_ValueChanged(object sender, GenericEventArgs<bool> e) => InvokeAndRefresh(sender, e, SelectedReplacementChanged);

    private void AccidentalsButtons_SelectedButtonChanged(object sender, GenericEventArgs<int> e) => InvokeAndRefresh(sender, new GenericEventArgs<bool>(e.Value != 0), SelectedAccidentalsChanged);

    private void AccidentalSwitch_ValueChanged(object sender, GenericEventArgs<bool> e) => InvokeAndRefresh(sender, e, SelectedAccidentalsChanged);

    private void InversionSwitch_ValueChanged(object sender, GenericEventArgs<bool> e) => InvokeAndRefresh(sender, e, SelectedInversionChanged);

    private void LevelsButton_SelectedButtonChanged(object sender, GenericEventArgs<int> e) => InvokeAndRefresh(sender, new GenericEventArgs<Level>((Level)e.Value), SelectedLevelChanged);

    private void GuessNameSwitch_ValueChanged(object sender, GenericEventArgs<bool> e) => InvokeAndRefresh(sender, e, SelectedGuessNameChanged);

    private void InvokeAndRefresh<T>(object sender, T args, GenericEventHandler<T> eventHandler)
    {
        eventHandler?.Invoke(sender, args);
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
        List<LeaderboardScore> leaderboard = new();

        var gameModeData = SaveManager.Save.GetGameModeData(gameModeType, intervalMode, level, guessName, withRandomAccidental, withInversion);
        if(gameModeData != null)
        {
            var scores = gameModeData.Scores.OrderByDescending(x => x.Value).Take(25).ToList();

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
            GuessNameSwitch.InitialState(gameMode.GuessName);
        }
        
        //ReplacementButtons.SelectButton(replacementMode ? 1 : 0);
        ReplacementSwitch.InitialState(replacementMode);
    }

    public void ShowInfo(string info, float duration = 2f, params string[] variables)
    {
        InfoText = info;

        InfoText_Variables = variables?.ToList();

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
        OnPropertyChanged(nameof(IsMIDIDeviceConnected));
        OnPropertyChanged(nameof(IsGuessNameModeVisible));
        OnPropertyChanged(nameof(IsReplacementModeVisible));
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

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    private IEnumerator Co_Info(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideInfo();
    }
}

public class GenericEventArgs<T> : EventArgs
{
    private readonly T _value;
    public T Value => _value;

    public GenericEventArgs(T value)
    {
        _value = value;
    }
}