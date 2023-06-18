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
    public Canvas Menu;
    public Transition Transition;

    private void Awake()
    {
        // Transition.SetPositionOpen_1();
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        Transition.Closed += Transition_Closed;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        Transition.Closed -= Transition_Closed;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("SceneManager_sceneLoaded");

        if (arg0.name == StaticResource.SCENE_MAIN_MENU)
        {
            Transition.SetPositionClose();
            StartCoroutine(Co_WaitForLoading());
        }
    }

    private void Transition_Closed(object sender, System.EventArgs e)
    {
        SceneManager.LoadScene(StaticResource.SCENE_MAIN_SCENE);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene_Trebble()
    {
        SceneSessionManager.Instance.SetGameMode(GameMode.Trebble);
        Transition.Close();
    }

    public void ChangeScene_Bass()
    {
        SceneSessionManager.Instance.SetGameMode(GameMode.Bass);
        Transition.Close();
    }

    public void ChangeScene_TrebbleBass()
    {
        SceneSessionManager.Instance.SetGameMode(GameMode.TrebbleBass);
        Transition.Close();
    }

    private IEnumerator Co_WaitForLoading()
    {
        yield return new WaitForSecondsRealtime(.25f);
        Transition.Open_1();
    }
}
