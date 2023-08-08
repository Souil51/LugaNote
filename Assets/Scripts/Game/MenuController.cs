using Assets;
using Assets.Scripts.Data;
using Assets.Scripts.Game;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Game.Save;
using Assets.Scripts.Utils;
using DataBinding.Core;
using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu script
/// Handle main menu UI events and scene transition
/// </summary>
public class MenuController : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    #region Properties
    [SerializeField] private MidiConfigurationHelper Configuration;
    [SerializeField] private Canvas MainCanvas;
    [SerializeField] private MenuViewModel ViewModel;

    [SerializeField] private List<GameModeController> GameModes;

    [SerializeField] private AudioSource AudioSourceTest;

    public Canvas Menu;
    public Transition Transition;

    private IController _controller;
    public IController Controller => _controller;

    private static MenuController _instance;
    public static MenuController Instance => _instance;

    private string _controllerLabel = "MIDI controller (88 keys)";
    public string ControllerLabel => _controllerLabel;

    public bool IsMidiConfigurationVisible => _controller.IsConfigurable;

    private bool _withAlteration = false;
    public bool WithAlteration => _withAlteration;

    private bool _replacementMode = false;
    public bool ReplaceReplacementMode => _replacementMode;

    private Level _selectedLevel = 0;
    public Level SelectedLevel => _selectedLevel;

    private IntervalMode _intervalMode;
    public IntervalMode IntervalMode => _intervalMode;

    private GameModeType _gameModeType;
    public GameModeType GameModeType => _gameModeType;

    private MenuState CurrentState = MenuState.Loaded;

    #endregion

    #region Unity methods
    private void Awake()
    {
        if (MenuController.Instance == null)
            MenuController._instance = this;

        GameSceneManager.Instance.SceneLoaded += GameSceneManager_SceneLoaded;

        _controller = ControllerFactory.Instance.GetController();
        _controller.Configuration += controller_Configuration;
        _controller.ConfigurationDestroyed += _controller_ConfigurationDestroyed;
    }

    private void OnEnable()
    {
        Transition.Closed += Transition_Closed;

        ViewModel.OpenMidiConfiguration += ViewModel_OpenMidiConfiguration;
        ViewModel.OpenScores += ViewModel_OpenScores;
        ViewModel.CloseScores += ViewModel_CloseScores;
        ViewModel.SelectedLevelChanged += ViewModel_SelectedLevelChanged;
        ViewModel.SelectedAlterationsChanged += ViewModel_SelectedAlterationsChanged;
        ViewModel.SelectedReplacementChanged += ViewModel_SelectedReplacementChanged;
        ViewModel.SelectedIntervalChanged += ViewModel_SelectedIntervalChanged;
        ViewModel.SelectedKeyChanged += ViewModel_SelectedKeyChanged;

        MidiMaster.deviceConnectedDelegate += MidiMaster_DeviceConnected;
        MidiMaster.deviceDisconnectedDelegate += MidiMaster_DeviceDisconnected;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.SceneLoaded -= GameSceneManager_SceneLoaded;
        Transition.Closed -= Transition_Closed;
        _controller.Configuration -= controller_Configuration;

        ViewModel.OpenMidiConfiguration -= ViewModel_OpenMidiConfiguration;
        ViewModel.OpenScores -= ViewModel_OpenScores;
        ViewModel.CloseScores -= ViewModel_CloseScores;

        ViewModel.SelectedLevelChanged -= ViewModel_SelectedLevelChanged;
        ViewModel.SelectedAlterationsChanged -= ViewModel_SelectedAlterationsChanged;
        ViewModel.SelectedReplacementChanged -= ViewModel_SelectedReplacementChanged;
        ViewModel.SelectedIntervalChanged -= ViewModel_SelectedIntervalChanged;
        ViewModel.SelectedKeyChanged -= ViewModel_SelectedKeyChanged;

        MidiMaster.deviceConnectedDelegate -= MidiMaster_DeviceConnected;
        MidiMaster.deviceDisconnectedDelegate -= MidiMaster_DeviceDisconnected;
    }

    private void Start()
    {
        SoundManager.LoadAllSounds();

        Save save = SaveManager.Save;
        var currentControllerType = ControllerFactory.Instance.GetCurrentType();
        var controllerData = save.GetControllerData();
        if (controllerData == null || controllerData.ControllerType != currentControllerType)
        {
            if(_controller.IsConfigurable)
                save.SetControllerData(currentControllerType, _controller.LowerNote, _controller.HigherNote);
            else
                save.SetControllerData(currentControllerType, MusicHelper.LowerNote, MusicHelper.HigherNote);

            SaveManager.SaveGame();
        }

        var lastGameMode = GameSceneManager.Instance.GetValue<GameMode>(Enums.GetEnumDescription(SceneSessionKey.GameMode));
        bool lastReplacementMode = GameSceneManager.Instance.GetValue<bool>(Enums.GetEnumDescription(SceneSessionKey.ReplacementMode));

        ViewModel.InitOptions(lastGameMode, lastReplacementMode);
        if(lastGameMode != null)
            LoadGameMode(lastGameMode, lastReplacementMode);

        ChangeState(MenuState.Idle);

        ViewModel.HideInfo();

        _controllerLabel = _controller.Label;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControllerLabel)));
        ViewModel.InitializeViewModel();
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
            // DisplayedScores = new List<LeaderboardScore>() { new LeaderboardScore(1, 99, DateTime.Now) };
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
                ViewModel.CallGameObjectCreated(this, new GameObjectEventArgs(goConfiguration));
            }

            stateChanged = true;
        }
        else if(CurrentState == MenuState.Idle && newState == MenuState.ViewScore)
        {
            stateChanged = true;
        }
        else if ((CurrentState == MenuState.Configuration || CurrentState == MenuState.ViewScore)
            && newState == MenuState.Idle)
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
        ViewModel.ShowInfo(info, duration);
    }

    public void LoadGameMode(GameMode gameMode, bool replacementMode)
    {
        if (gameMode != null)
        {
            _gameModeType = gameMode.GameModeType;
            _intervalMode = gameMode.IntervalMode;
            _selectedLevel = gameMode.Level;
            _withAlteration = gameMode.WithRandomAlteration;
        }

        _replacementMode = replacementMode;
    }

    #region Events callbacks
    private void controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        if (e.Result)
        {
            string info = "";
            _controllerLabel = _controller.Label;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControllerLabel)));

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
            Save save = SaveManager.Save;
            save.SetControllerData(ControllerFactory.Instance.GetCurrentType(), _controller.LowerNote, _controller.HigherNote);
            SaveManager.SaveGame();
        }

        ChangeState(MenuState.Idle);
    }

    private void _controller_ConfigurationDestroyed(object sender, GameObjectEventArgs e)
    {
        ViewModel.CallGameObjectDestroyed(sender, e);
    }

    private void GameSceneManager_SceneLoaded(object sender, SceneEventArgs e)
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;

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

    private void ViewModel_SelectedLevelChanged(object sender, LevelEventArgs e)
    {
        _selectedLevel = e.Level;
    }

    private void ViewModel_SelectedKeyChanged(object sender, GameModeTypeEventArgs e)
    {
        _gameModeType = e.GameModeType;
    }

    private void ViewModel_SelectedIntervalChanged(object sender, IntervalEventArgs e)
    {
        _intervalMode = e.Interval;
    }

    private void ViewModel_SelectedReplacementChanged(object sender, BoolEventArgs e)
    {
        _replacementMode = e.Value;
    }

    private void ViewModel_SelectedAlterationsChanged(object sender, BoolEventArgs e)
    {
        _withAlteration = e.Value;
    }

    private void MidiMaster_DeviceConnected(string deviceName)
    {
        Debug.Log("MidiMaster_DeviceConnected");

        var save = SaveManager.Save;
        save.AddDeviceData(deviceName);
    }

    private void MidiMaster_DeviceDisconnected(string deviceName)
    {
        Debug.Log("MidiMaster_DeviceDisconnected");
    }

    #endregion

    #region UI event

    public void UI_Play()
    {
        if (CurrentState != MenuState.Idle) return;

        ChangeScene();
    }

    private void ChangeScene()
    {
        var gameMode = GameModeManager.GetGameMode(_gameModeType, _intervalMode, _selectedLevel, _withAlteration);
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), gameMode);
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.ReplacementMode), _replacementMode);
        Transition.Close();
    }

    private void ViewModel_CloseScores(object sender, EventArgs e)
    {
        ChangeState(MenuState.Idle);
    }

    private void ViewModel_OpenScores(object sender, GameModeEventArgs e)
    {
        ChangeState(MenuState.ViewScore);
    }

    private void ViewModel_OpenMidiConfiguration(object sender, EventArgs e)
    {
        if (CurrentState == MenuState.Idle)
            ChangeState(MenuState.Configuration);
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

    #endregion
}
