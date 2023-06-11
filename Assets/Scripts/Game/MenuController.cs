using System.Collections;
using System.Collections.Generic;
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
        Transition.SetPositionOpen_1();

        Transition.Closed += Transition_Closed;
    }

    private void Transition_Closed(object sender, System.EventArgs e)
    {
        SceneManager.LoadScene("MainScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene()
    {
        Transition.Close();
    }
}
