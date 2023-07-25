using Assets;
using Assets.Scripts.Data;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MenuViewModel : ViewModelBase
{
    [SerializeField] private InfoMessage Info;

    public delegate void OpenMidiConfigurationEventHandler(object sender, EventArgs e);
    public event OpenMidiConfigurationEventHandler OpenMidiConfiguration;

    public delegate void ToggleAlterationsEventHandler(object sender, BoolEventArgs e);
    public event ToggleAlterationsEventHandler ToggleAlteration;

    public delegate void ToggleReplacementModeEventHandler(object sender, BoolEventArgs e);
    public event ToggleReplacementModeEventHandler ToggleReplacementMode;

    public delegate void OpenScoresModeEventHandler(object sender, GameModeEventArgs e);
    public event OpenScoresModeEventHandler OpenScores;

    public delegate void CloseScoresModeEventHandler(object sender, EventArgs e);
    public event CloseScoresModeEventHandler CloseScores;

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

    private Color _withAlterationColor;
    public Color WithAlterationColor
    {
        get => _withAlterationColor;
        private set
        {
            _withAlterationColor = value;
            OnPropertyChanged();
        }
    }

    private Color _withoutAlterationColor;
    public Color WithoutAlterationColor
    {
        get => _withoutAlterationColor;
        private set
        {
            _withoutAlterationColor = value;
            OnPropertyChanged();
        }
    }

    private Color _withAlterationTextColor;
    public Color WithAlterationTextColor
    {
        get => _withAlterationTextColor;
        private set
        {
            _withAlterationTextColor = value;
            OnPropertyChanged();
        }
    }

    private Color _withoutAlterationTextColor;
    public Color WithoutAlterationTextColor
    {
        get => _withoutAlterationTextColor;
        private set
        {
            _withoutAlterationTextColor = value;
            OnPropertyChanged();
        }
    }

    private Color _replacementModeColor;
    public Color ReplacementModeColor
    {
        get => _replacementModeColor;
        private set
        {
            _replacementModeColor = value;
            OnPropertyChanged();
        }
    }

    private Color _noReplacementModeColor;
    public Color NoReplacementModeColor
    {
        get => _noReplacementModeColor;
        private set
        {
            _noReplacementModeColor = value;
            OnPropertyChanged();
        }
    }

    private Color _replacementModeTextColor;
    public Color ReplacementModeTextColor
    {
        get => _replacementModeTextColor;
        private set
        {
            _replacementModeTextColor = value;
            OnPropertyChanged();
        }
    }

    private Color _noReplacementModeTextColor;
    public Color NoReplacementModeTextColor
    {
        get => _noReplacementModeTextColor;
        private set
        {
            _noReplacementModeTextColor = value;
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

        UpdateAlterationButtons(MenuController.Instance.WithAlteration);
        UpdateReplacementModeButtons(MenuController.Instance.ReplaceReplacementMode);
    }

    private void OnDisable()
    {
        Info.Disappeared -= Info_Disappeared;
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

    private List<LeaderboardScore> GenerateLeaderboardScoreList(GameModeType gameModeType, IntervalMode intervalMode, bool withRandomAlteration)
    {
        var gameModeData = SaveManager.Save.GetGameModeData(gameModeType, intervalMode, withRandomAlteration);
        var scores = gameModeData.Scores.OrderByDescending(x => x.Value).Take(10).ToList();

        List<LeaderboardScore> leaderboard = new List<LeaderboardScore>();

        for (int i = 0; i < scores.Count; i++)
        {
            leaderboard.Add(new LeaderboardScore(i + 1, scores[i].Value, scores[i].Date));
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

    public void UpdateAlterationButtons(bool withAlteration)
    {
        if (!withAlteration)
        {
            WithoutAlterationColor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_LIGHT_BLUE);
            WithAlterationColor = Color.white;

            WithoutAlterationTextColor = Color.white;
            WithAlterationTextColor = Color.black;
        }
        else
        {
            WithoutAlterationColor = Color.white;
            WithAlterationColor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_LIGHT_BLUE);

            WithoutAlterationTextColor = Color.black;
            WithAlterationTextColor = Color.white;
        }
    }

    public void UpdateReplacementModeButtons(bool replacementMode)
    {
        if (!replacementMode)
        {
            NoReplacementModeColor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_LIGHT_BLUE);
            ReplacementModeColor = Color.white;

            NoReplacementModeTextColor = Color.white;
            ReplacementModeTextColor = Color.black;
        }
        else
        {
            NoReplacementModeColor = Color.white;
            ReplacementModeColor = UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_LIGHT_BLUE);

            NoReplacementModeTextColor = Color.black;
            ReplacementModeTextColor = Color.white;
        }
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
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.Trebble, IntervalMode.Note, false);
        ScorePanelVisible = true;
        OpenScores?.Invoke(this, new GameModeEventArgs(new GameMode(0, GameModeType.Trebble, IntervalMode.Note, false)));
    }

    public void ViewScore_Bass()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.Bass, IntervalMode.Note, false);
        ScorePanelVisible = true;
        OpenScores?.Invoke(this, new GameModeEventArgs(new GameMode(0, GameModeType.Bass, IntervalMode.Note, false)));
    }

    public void ViewScore_TrebbleBass()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.TrebbleBass, IntervalMode.Note, false);
        ScorePanelVisible = true;
        OpenScores?.Invoke(this, new GameModeEventArgs(new GameMode(0, GameModeType.TrebbleBass, IntervalMode.Note, false)));
    }

    public void ViewScore_Close()
    {
        ScorePanelVisible = false;
        CloseScores?.Invoke(this, EventArgs.Empty);
    }

    public void Alteration_WithAlteration()
    {
        UpdateAlterationButtons(true);
        ToggleAlteration?.Invoke(this, new BoolEventArgs(true));
    }

    public void Alteration_WithoutAlteration()
    {
        UpdateAlterationButtons(false);
        ToggleAlteration?.Invoke(this, new BoolEventArgs(false));
    }

    public void ReplacementMode_Replacement()
    {
        UpdateReplacementModeButtons(true);
        ToggleReplacementMode?.Invoke(this, new BoolEventArgs(true));
    }

    public void ReplacementMode_NoReplacement()
    {
        UpdateReplacementModeButtons(false);
        ToggleReplacementMode?.Invoke(this, new BoolEventArgs(false));
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

public class GameModeEventArgs : EventArgs
{
    private GameMode _gameMode;
    public GameMode GameMode => _gameMode;

    public GameModeEventArgs(GameMode gameMode)
    {
        _gameMode = gameMode;
    }
}