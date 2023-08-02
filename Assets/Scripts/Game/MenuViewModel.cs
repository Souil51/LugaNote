using Assets;
using Assets.Scripts.Data;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Utils;
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
    [SerializeField] private RadioButtonsGroupViewModel AlterationsButtons;
    [SerializeField] private RadioButtonsGroupViewModel ReplacementButtons;
    [SerializeField] private RadioButtonsGroupViewModel IntervalButtons;
    [SerializeField] private RadioButtonsGroupViewModel KeyButtons;

    public delegate void OpenMidiConfigurationEventHandler(object sender, EventArgs e);
    public event OpenMidiConfigurationEventHandler OpenMidiConfiguration;

    public delegate void OpenScoresModeEventHandler(object sender, GameModeEventArgs e);
    public event OpenScoresModeEventHandler OpenScores;

    public delegate void CloseScoresModeEventHandler(object sender, EventArgs e);
    public event CloseScoresModeEventHandler CloseScores;

    public delegate void SelectedLevelChangedEventHandler(object sender, LevelEventArgs e);
    public event SelectedLevelChangedEventHandler SelectedLevelChanged;

    public delegate void SelectedAlterationsChangedEventHandler(object sender, BoolEventArgs e);
    public event SelectedAlterationsChangedEventHandler SelectedAlterationsChanged;

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

    private void Awake()
    {
        MenuController.Instance.PropertyChanged += Instance_PropertyChanged;
    }

    private void Start()
    {
        Info.Disappeared += Info_Disappeared;
        LevelButtons.SelectedButtonChanged += LevelsButton_SelectedButtonChanged;
        AlterationsButtons.SelectedButtonChanged += AlterationsButtons_SelectedButtonChanged;
        ReplacementButtons.SelectedButtonChanged += ReplacementButtons_SelectedButtonChanged;
        IntervalButtons.SelectedButtonChanged += IntervalButtons_SelectedButtonChanged;
        KeyButtons.SelectedButtonChanged += KeyButtons_SelectedButtonChanged;

        IsMIDIDeviceConnected = MenuController.Instance.Controller.IsConfigurable;

        UpdateDisplayedScores();
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
    }

    private void ReplacementButtons_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedReplacementChanged?.Invoke(sender, new BoolEventArgs(e.Value != 0));
        UpdateDisplayedScores();
    }

    private void AlterationsButtons_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedAlterationsChanged?.Invoke(sender, new BoolEventArgs(e.Value == 0));
        UpdateDisplayedScores();
    }

    private void LevelsButton_SelectedButtonChanged(object sender, IntEventArgs e)
    {
        SelectedLevelChanged?.Invoke(sender, new LevelEventArgs((Level)e.Value));
        UpdateDisplayedScores();
    }

    private void OnDisable()
    {
        Info.Disappeared -= Info_Disappeared;
        LevelButtons.SelectedButtonChanged -= LevelsButton_SelectedButtonChanged;
        AlterationsButtons.SelectedButtonChanged -= AlterationsButtons_SelectedButtonChanged;
        ReplacementButtons.SelectedButtonChanged -= ReplacementButtons_SelectedButtonChanged;
        IntervalButtons.SelectedButtonChanged -= IntervalButtons_SelectedButtonChanged;
        KeyButtons.SelectedButtonChanged -= KeyButtons_SelectedButtonChanged;
    }

    public void InitializeViewModel()
    {
        InitialiserNotifyPropertyChanged();

        IsMidiConfigurationVisible = MenuController.Instance.IsMidiConfigurationVisible;

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
        var alterations = AlterationsButtons.SelectedIndex == 1;

        DisplayedScores = GenerateLeaderboardScoreList(key, interval, level, alterations);
    }

    private List<LeaderboardScore> GenerateLeaderboardScoreList(GameModeType gameModeType, IntervalMode intervalMode, Level level, bool withRandomAlteration)
    {
        List<LeaderboardScore> leaderboard = new List<LeaderboardScore>();

        var gameModeData = SaveManager.Save.GetGameModeData(gameModeType, intervalMode, level, withRandomAlteration);
        if(gameModeData != null)
        {
            var scores = gameModeData.Scores.OrderByDescending(x => x.Value).Take(10).ToList();

            for (int i = 0; i < scores.Count; i++)
            {
                leaderboard.Add(new LeaderboardScore(i + 1, scores[i].Value, scores[i].Date));
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
            AlterationsButtons.SelectButton(gameMode.WithRandomAlteration ? 1 : 0);
        }
        
        ReplacementButtons.SelectButton(replacementMode ? 1 : 0);
    }

    public void ShowInfo(string info, float duration = 2f)
    {
        InfoText = info;
        IsInfoVisible = true;

        StartCoroutine(Co_Info(duration));
    }

    public void HideInfo()
    {
        Info.Disappear();
    }

    private void Info_Disappeared(object sender, System.EventArgs e)
    {
        IsInfoVisible = false;
    }

    public void StartControllerConfiguration()
    {
        OpenMidiConfiguration?.Invoke(this, EventArgs.Empty);
    }

    public void ViewScore_Trebble()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.Trebble, IntervalMode.Note, (Level)LevelButtons.SelectedIndex, false);
        ScorePanelVisible = true;
        OpenScores?.Invoke(this, new GameModeEventArgs(new GameMode(0, GameModeType.Trebble, IntervalMode.Note, (Level)LevelButtons.SelectedIndex, false)));
    }

    public void ViewScore_Bass()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.Bass, IntervalMode.Note, (Level)LevelButtons.SelectedIndex, false);
        ScorePanelVisible = true;
        OpenScores?.Invoke(this, new GameModeEventArgs(new GameMode(0, GameModeType.Bass, IntervalMode.Note, (Level)LevelButtons.SelectedIndex, false)));
    }

    public void ViewScore_TrebbleBass()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.TrebbleBass, IntervalMode.Note, (Level)LevelButtons.SelectedIndex, false);
        ScorePanelVisible = true;
        OpenScores?.Invoke(this, new GameModeEventArgs(new GameMode(0, GameModeType.TrebbleBass, IntervalMode.Note, (Level)LevelButtons.SelectedIndex, false)));
    }

    public void ViewScore_Close()
    {
        ScorePanelVisible = false;
        CloseScores?.Invoke(this, EventArgs.Empty);
    }

    private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(ControllerLabel))
        {
            ControllerLabel = MenuController.Instance.ControllerLabel;
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