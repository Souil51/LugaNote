using Assets;
using Assets.Scripts.Data;
using Assets.Scripts.Game;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu script
/// Handle main menu UI events and scene transition
/// </summary>
public class MenuController : ViewModelBase
{
    #region Properties
    [SerializeField] private MidiConfigurationHelper Configuration;
    [SerializeField] private Canvas MainCanvas;
    [SerializeField] private InfoMessage Info;

    [SerializeField] private List<GameModeController> GameModes;

    [SerializeField] private AudioSource AudioSourceTest;

    public Canvas Menu;
    public Transition Transition;

    private IController _controller;

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

    private MenuState CurrentState = MenuState.Loaded;

    #endregion

    #region Unity methods
    private void Awake()
    {
        GameSceneManager.Instance.SceneLoaded += GameSceneManager_SceneLoaded;

        _controller = ControllerFactory.Instance.GetController();
        _controller.Configuration += controller_Configuration;
        _controller.ConfigurationDestroyed += _controller_ConfigurationDestroyed;
    }

    private void OnEnable()
    {
        Transition.Closed += Transition_Closed;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.SceneLoaded -= GameSceneManager_SceneLoaded;
        Transition.Closed -= Transition_Closed;
        _controller.Configuration -= controller_Configuration;
    }

    private void Start()
    {
        Info.Disappeared += Info_Disappeared;
        SoundManager.LoadAllNotes();

        ChangeState(MenuState.Idle);

        //var gameModeData = SaveManager.Save.GetGameModeData(GameModeType.Trebble, IntervalMode.Note);
        //DisplayedScores = gameModeData.Scores.OrderByDescending(x => x.Value).Take(10).ToList();

        InitialiserNotifyPropertyChanged();

        HideInfo();

        ControllerLabel = _controller.Label;
        IsMidiConfigurationVisible = _controller.IsConfigurable;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AudioSourceTest.clip = SoundManager.GetNoteClip(PianoNote.C4);
            AudioSourceTest.Play();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SoundManager.PlayNote(PianoNote.C4);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DisplayedScores = new List<LeaderboardScore>() { new LeaderboardScore(1, 99, DateTime.Now) };
        }
    }



    #endregion

    private void ChangeState(MenuState newState)
    {
        bool stateChanged = false;

        if (CurrentState == MenuState.Loaded && newState == MenuState.Idle)
        {
            stateChanged = true;
        }
        else if (CurrentState == MenuState.Idle && newState == MenuState.Configuration)
        {
            var goConfiguration = _controller.Configure();

            if (goConfiguration != null)
            {
                OnGameObjectCreated(this, new GameObjectEventArgs(goConfiguration));
            }

            stateChanged = true;
        }
        else if (CurrentState == MenuState.Configuration && newState == MenuState.Idle)
        {
            stateChanged = true;
        }

        if (stateChanged)
        {
            CurrentState = newState;
        }
    }

    private void ShowInfo(string info, float duration = 2f)
    {
        InfoText = info;
        IsInfoVisible = true;

        StartCoroutine(Co_Info(duration));
    }

    private void HideInfo()
    {
        Info.Disappear();
    }

    public void StartControllerConfiguration()
    {
        if (CurrentState == MenuState.Idle)
            ChangeState(MenuState.Configuration);
    }

    #region Events callbacks
    private void controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        if (e.Result)
        {
            string info = "";
            ControllerLabel = _controller.Label;

            if ((int)_controller.HigherNote - (int)_controller.LowerNote + 1 == 88)
            {
                info = string.Format(Strings.MENU_INFO_MIDI_88_TOUCHES);
            }
            else if ((int)_controller.HigherNote - (int)_controller.LowerNote + 1 == 61)
            {
                info = string.Format(Strings.MENU_INFO_MIDI_61_TOUCHES);
            }
            else
            {
                info = string.Format(Strings.MENU_MIDI_CUSTOM_TOUCHES, _controller.HigherNote - _controller.LowerNote, _controller.LowerNote, _controller.HigherNote);
            }

            ShowInfo(info);
        }

        ChangeState(MenuState.Idle);
    }

    private void _controller_ConfigurationDestroyed(object sender, GameObjectEventArgs e)
    {
        OnGameObjectDestroyed(sender, e);
    }

    private void GameSceneManager_SceneLoaded(object sender, SceneEventArgs e)
    {
        if (e.Scene.name == StaticResource.SCENE_MAIN_MENU)
        {
            Transition.SetPositionClose();
            StartCoroutine(Co_WaitForLoading());
        }
    }

    private void Transition_Closed(object sender, System.EventArgs e)
    {
        GameSceneManager.Instance.LoadScene(StaticResource.SCENE_MAIN_SCENE);
    }

    private void Info_Disappeared(object sender, System.EventArgs e)
    {
        IsInfoVisible = false;
    }
    #endregion

    #region UI event
    public void ChangeScene_Trebble()
    {
        if (CurrentState != MenuState.Idle) return;

        var gameMode = GameModeManager.GetGameMode(GameModeType.Trebble, IntervalMode.Note);
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), gameMode);
        Transition.Close();
    }

    public void ChangeScene_Bass()
    {
        if (CurrentState != MenuState.Idle) return;

        var gameMode = GameModeManager.GetGameMode(GameModeType.Bass, IntervalMode.Note);
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), gameMode);
        Transition.Close();
    }

    public void ChangeScene_TrebbleBass()
    {
        if (CurrentState != MenuState.Idle) return;

        var gameMode = GameModeManager.GetGameMode(GameModeType.TrebbleBass, IntervalMode.Note);
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), gameMode);
        Transition.Close();
    }

    public void ViewScore_Trebble()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.Trebble, IntervalMode.Note);
        ScorePanelVisible = true;
    }

    public void ViewScore_Bass()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.Bass, IntervalMode.Note);
        ScorePanelVisible = true;
    }

    public void ViewScore_TrebbleBass()
    {
        DisplayedScores = GenerateLeaderboardScoreList(GameModeType.TrebbleBass, IntervalMode.Note);
        ScorePanelVisible = true;
    }

    public void ViewScore_Close()
    {
        ScorePanelVisible = false;
    }

    private List<LeaderboardScore> GenerateLeaderboardScoreList(GameModeType gameModeType, IntervalMode intervalMode)
    {
        var gameModeData = SaveManager.Save.GetGameModeData(gameModeType, intervalMode);
        var scores = gameModeData.Scores.OrderByDescending(x => x.Value).Take(10).ToList();

        List<LeaderboardScore> leaderboard = new List<LeaderboardScore>();

        for(int i = 0; i < scores.Count; i++)
        {
            leaderboard.Add(new LeaderboardScore(i + 1, scores[i].Value, scores[i].Date));
        }

        return leaderboard;
    }

    #endregion

    #region Coroutines
    /// <summary>
    /// Wait 0.25s before opening the transition, to wait all loading
    /// </summary>
    private IEnumerator Co_WaitForLoading()
    {
        yield return new WaitForSecondsRealtime(.25f);
        Transition.Open_1();
    }

    /// <summary>
    /// Hide the info panel after X seconds
    /// </summary>
    private IEnumerator Co_Info(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideInfo();
    }
    #endregion
}
