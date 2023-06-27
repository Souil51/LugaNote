using Assets.Scripts.Game;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
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
    #endregion

    #region Unity methods
    private void Awake()
    {
        GameSceneManager.Instance.SceneLoaded += GameSceneManager_SceneLoaded;

        _controller = ControllerFactory.Instance.GetController();
        _controller.Configuration += controller_Configuration;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _controller.Configure(MainCanvas);
        }
    }
    #endregion

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
        _controller.Configure(MainCanvas);
    }

    #region Events callbacks
    private void controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        Debug.Log("Configuration ENDED");

        if (e.Result)
        {
            string info = "";

            if (_controller.HigherNote - _controller.LowerNote == 88 - 1)
                info = string.Format(Strings.MENU_MIDI_88_TOUCHES);
            else if (_controller.HigherNote - _controller.LowerNote == 61 - 1)
                info = string.Format(Strings.MENU_MIDI_61_TOUCHES);
            else
                info = string.Format(Strings.MENU_MIDI_CUSTOM_TOUCHES, _controller.HigherNote - _controller.LowerNote, _controller.LowerNote, _controller.HigherNote);

            ShowInfo(info);
        }
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
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), GameMode.Trebble);
        Transition.Close();
    }

    public void ChangeScene_Bass()
    {
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), GameMode.Bass);
        Transition.Close();
    }

    public void ChangeScene_TrebbleBass()
    {
        GameSceneManager.Instance.SetValue(Enums.GetEnumDescription(SceneSessionKey.GameMode), GameMode.TrebbleBass);
        Transition.Close();
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
