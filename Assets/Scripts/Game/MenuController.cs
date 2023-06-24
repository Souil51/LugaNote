using Assets.Scripts.Game;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu script
/// Handle main menu UI events and scene transition
/// </summary>
public class MenuController : MonoBehaviour
{
    [SerializeField] private MidiConfigurationHelper Configuration;
    [SerializeField] private Canvas MainCanvas;

    public Canvas Menu;
    public Transition Transition;

    private IController _controller;

    private void Awake()
    {
        // Transition.SetPositionOpen_1();
        GameSceneManager.Instance.SceneLoaded += Instance_SceneLoaded;

        _controller = ControllerFactory.Instance.GetController();
        _controller.Configuration += _controller_Configuration;
    }

    private void OnEnable()
    {
        Transition.Closed += Transition_Closed;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.SceneLoaded -= Instance_SceneLoaded;
        Transition.Closed -= Transition_Closed;
        _controller.Configuration -= _controller_Configuration;
    }
    private void Instance_SceneLoaded(object sender, SceneEventArgs e)
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _controller.Configure(MainCanvas);
        }
    }

    private void _controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        Debug.Log("Configuration ENDED");
    }

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

    private IEnumerator Co_WaitForLoading()
    {
        yield return new WaitForSecondsRealtime(.25f);
        Transition.Open_1();
    }
}
