using Assets;
using Assets.Scripts.Data;
using Assets.Scripts.Game;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Game.Save;
using DataBinding.Core;
using MidiJack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;

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

    private bool _withAccidental = false;
    public bool WithAccidental => _withAccidental;

    private bool _withInversion = false;
    public bool WithInversion => _withInversion;

    private bool _guessName = false;
    public bool GuessName => _guessName;

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
        Debug.Log("AWAKE");
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        if (MenuController.Instance == null)
            MenuController._instance = this;

        GameSceneManager.Instance.SceneLoaded += GameSceneManager_SceneLoaded;

        _controller = ControllerFactory.Instance.GetController();
        _controller.Configuration += Controller_Configuration;
        _controller.ConfigurationDestroyed += Controller_ConfigurationDestroyed;
    }

    private void OnEnable()
    {
        Transition.Closed += Transition_Closed;

        ViewModel.OpenMidiConfiguration += ViewModel_OpenMidiConfiguration;
        ViewModel.SelectedLevelChanged += ViewModel_SelectedLevelChanged;
        ViewModel.SelectedAccidentalsChanged += ViewModel_SelectedAccidentalsChanged;
        ViewModel.SelectedInversionChanged += ViewModel_SelectedInversionChanged;
        ViewModel.SelectedReplacementChanged += ViewModel_SelectedReplacementChanged;
        ViewModel.SelectedIntervalChanged += ViewModel_SelectedIntervalChanged;
        ViewModel.SelectedKeyChanged += ViewModel_SelectedKeyChanged;
        ViewModel.SelectedGuessNameChanged += ViewModel_SelectedGuessNameChanged;
        ViewModel.QuitClicked += ViewModel_QuitClicked;

        MidiMaster.deviceConnectedDelegate += MidiMaster_DeviceConnected;
        MidiMaster.deviceDisconnectedDelegate += MidiMaster_DeviceDisconnected;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.SceneLoaded -= GameSceneManager_SceneLoaded;
        Transition.Closed -= Transition_Closed;
        _controller.Configuration -= Controller_Configuration;

        ViewModel.OpenMidiConfiguration -= ViewModel_OpenMidiConfiguration;

        ViewModel.SelectedLevelChanged -= ViewModel_SelectedLevelChanged;
        ViewModel.SelectedAccidentalsChanged -= ViewModel_SelectedAccidentalsChanged;
        ViewModel.SelectedInversionChanged -= ViewModel_SelectedInversionChanged;
        ViewModel.SelectedReplacementChanged -= ViewModel_SelectedReplacementChanged;
        ViewModel.SelectedIntervalChanged -= ViewModel_SelectedIntervalChanged;
        ViewModel.SelectedKeyChanged -= ViewModel_SelectedKeyChanged;
        ViewModel.SelectedGuessNameChanged -= ViewModel_SelectedGuessNameChanged;
        ViewModel.QuitClicked -= ViewModel_QuitClicked;

        MidiMaster.deviceConnectedDelegate -= MidiMaster_DeviceConnected;
        MidiMaster.deviceDisconnectedDelegate -= MidiMaster_DeviceDisconnected;
    }

    private async void Start()
    {
        SoundManager.LoadAllSounds();
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;

        Save save = SaveManager.Save;
        var currentControllerType = ControllerFactory.Instance.GetCurrentType();
        
        var lastGameMode = GameSceneManager.Instance.GetValue<GameMode>(Enums.GetEnumDescription(SceneSessionKey.GameMode));
        bool lastReplacementMode = GameSceneManager.Instance.GetValue<bool>(Enums.GetEnumDescription(SceneSessionKey.ReplacementMode));

        if(lastGameMode == null && save._lastGameMode != null)
        {
            lastGameMode = save._lastGameMode;
            lastReplacementMode = save._lastReplacementMode;
        }

        ViewModel.InitOptions(lastGameMode, lastReplacementMode);
        if(lastGameMode != null)
            LoadGameMode(lastGameMode, lastReplacementMode);
        
        ChangeState(MenuState.Idle);

        ViewModel.HideInfo();

        await UpdateControllerLabel();
        ViewModel.InitializeViewModel();
        ViewModel.UpdateRecommendation();
    }

    private async void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj) => await UpdateControllerLabel();

    /// <summary>
    /// Controller label is a concatenation of localized strings so we handle it
    /// </summary>
    private async Task UpdateControllerLabel()
    {
        await _controller.UpdateLabel();
        _controllerLabel = _controller.Label;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControllerLabel)));
    }

    #endregion

    private void ChangeState(MenuState newState)
    {
        bool stateChanged = false;

        if (CurrentState == MenuState.Loaded && newState == MenuState.Idle)
            stateChanged = true;
        else if (CurrentState == MenuState.Idle && newState == MenuState.Configuration)
        {
            var save = SaveManager.Save;
            var existingDevice = save.GetControllerData(MidiMaster.GetDeviceName());
            var goConfiguration = _controller.Configure(existingDevice == null);

            if (goConfiguration != null)
            {
                ViewModel.CallGameObjectCreated(this, new GameObjectEventArgs(goConfiguration));
            }

            stateChanged = true;
        }
        else if(CurrentState == MenuState.Idle && newState == MenuState.ViewScore)
            stateChanged = true;
        else if ((CurrentState == MenuState.Configuration || CurrentState == MenuState.ViewScore) && newState == MenuState.Idle)
            stateChanged = true;

        if (stateChanged)
            CurrentState = newState;
    }

    private void ShowInfo(string info, float duration = 3f, params string[] variables) => ViewModel.ShowInfo(info, duration, variables);

    public void LoadGameMode(GameMode gameMode, bool replacementMode)
    {
        if (gameMode != null)
        {
            _gameModeType = gameMode.GameModeType;
            _intervalMode = gameMode.IntervalMode;
            _selectedLevel = gameMode.Level;
            _withAccidental = gameMode.WithRandomAccidental;
            _withInversion = gameMode.WithInversion;
            _guessName = gameMode.GuessName;
        }

        _replacementMode = replacementMode;
    }

    public bool IsMidiConnected() => !String.IsNullOrEmpty(MidiMaster.GetDeviceName());

    #region Events callbacks
    private void Controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        if (e.Result)
        {
            string info;
            UpdateFooter();
            var variablesList = new List<string>();

            if ((int)_controller.HigherNote - (int)_controller.LowerNote + 1 == 88)
                info = string.Format(StaticResource.LOCALIZATION_MENU_INFO_MIDI_88_TOUCHES);
            else if ((int)_controller.HigherNote - (int)_controller.LowerNote + 1 == 61)
                info = string.Format(StaticResource.LOCALIZATION_MENU_INFO_MIDI_61_TOUCHES);
            else
            {
                info = string.Format(StaticResource.LOCALIZATION_MENU_INFO_MIDI_CUSTOM_TOUCHES, _controller.HigherNote - _controller.LowerNote, _controller.LowerNote, _controller.HigherNote);
                variablesList = new List<string>() { ((int)(_controller.HigherNote - _controller.LowerNote) + 1).ToString(), _controller.LowerNote.ToString(), _controller.HigherNote.ToString() };
            }

            ShowInfo(info, 3f, variablesList.ToArray());
            Save save = SaveManager.Save;
            save.SetControllerData(ControllerFactory.Instance.GetCurrentType(), MidiMaster.GetDeviceName(), _controller.LowerNote, _controller.HigherNote);
            SaveManager.SaveGame();
        }

        ChangeState(MenuState.Idle);
        ViewModel.UpdateRecommendation();
    }

    private void Controller_ConfigurationDestroyed(object sender, GameObjectEventArgs e) => ViewModel.CallGameObjectDestroyed(sender, e);

    private void GameSceneManager_SceneLoaded(object sender, SceneEventArgs e)
    {
        //Screen.orientation = ScreenOrientation.LandscapeRight;

        if (e.Scene.name == StaticResource.SCENE_MAIN_MENU)
        {
            Transition.SetPositionClose();
            StartCoroutine(Co_WaitForLoading());
        }
    }

    private void Transition_Closed(object sender, EventArgs e) => GameSceneManager.Instance.LoadScene(StaticResource.SCENE_MAIN_SCENE);

    private void ViewModel_SelectedLevelChanged(object sender, GenericEventArgs<Level> e) => _selectedLevel = e.Value;

    private void ViewModel_SelectedKeyChanged(object sender, GenericEventArgs<GameModeType> e) => _gameModeType = e.Value;

    private void ViewModel_SelectedIntervalChanged(object sender, GenericEventArgs<IntervalMode> e) => _intervalMode = e.Value;

    private void ViewModel_SelectedReplacementChanged(object sender, GenericEventArgs<bool> e) => _replacementMode = e.Value;

    private void ViewModel_SelectedAccidentalsChanged(object sender, GenericEventArgs<bool> e) => _withAccidental = e.Value;

    private void ViewModel_SelectedInversionChanged(object sender, GenericEventArgs<bool> e) => _withInversion = e.Value;

    private void ViewModel_SelectedGuessNameChanged(object sender, GenericEventArgs<bool> e) => _guessName = e.Value;

    private void MidiMaster_DeviceConnected(string deviceName)
    {
        Debug.Log("MidiMaster_DeviceConnected");
        if(Controller is MultipleController multipleCtrl)
        {
            multipleCtrl.EnableMIDI();
            UpdateFooter();
        }

        var save = SaveManager.Save;
        save.AddDeviceData(deviceName);

        var controllerData = save.GetControllerData(deviceName);
        if(controllerData != null)
        {
            if(Controller is MultipleController multipleController)
                multipleController.SetControllerData(controllerData);
            else if(Controller is MidiController midiController)
                midiController.SetControllerData(controllerData);

            ViewModel.UpdateRecommendation();
        }
        else // if device not exists, open configuration
            ViewModel_OpenMidiConfiguration(this, EventArgs.Empty);

        ViewModel.UpdateMidiConnection();
    }

    private void MidiMaster_DeviceDisconnected(string deviceName)
    {
        if (Controller is MultipleController multipleCtrl)
        {
            multipleCtrl.DisableMIDI();
            UpdateFooter();
        }
    }

    private void ViewModel_QuitClicked(object sender, EventArgs e)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UpdateFooter()
    {
        _controllerLabel = _controller.Label;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControllerLabel)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMidiConfigurationVisible)));
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
        var gameMode = GameModeManager.GetGameMode(_gameModeType, _intervalMode, _selectedLevel, _guessName, _withAccidental, _withInversion);
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), gameMode);
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.ReplacementMode), _replacementMode);

        var save = SaveManager.Save;
        save.SetLastGameMode(gameMode, _replacementMode);
        SaveManager.SaveGame();

        Transition.Close();
    }

    private void ViewModel_OpenMidiConfiguration(object sender, EventArgs e)
    {
        if (CurrentState == MenuState.Idle)
            ChangeState(MenuState.Configuration);
    }

    public void UI_ButtonClick()
    {
        SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_CLICK, 1f);
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
