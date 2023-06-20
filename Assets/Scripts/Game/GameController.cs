using Assets.Scripts.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

/// <summary>
/// 
/// </summary>
public class GameController : MonoBehaviour, INotifyPropertyChanged
{
    [SerializeField] private Transition Transition;
    [SerializeField] private List<Staff> Staffs;
    [SerializeField] private ControllerType ControllerType; // Replace this with Dependancy Injection for Controller ?
    [SerializeField] private bool ReplacementMode; // Can a note replace the same note on other octave ?
    [SerializeField] private GameInputHandler InputHandler;
    [SerializeField] private TimeScaleManager TimeScaleManager;
    [SerializeField] private GameViewModel ViewModel;
    [SerializeField] private float YPositionFirstStaff;
    [SerializeField] private float YPositionSecondStaff;
    [SerializeField] private GameMode GameMode;

    public List<Staff> GameStaffs => Staffs;
    public bool GameReplacementMode => ReplacementMode;

    private IController Controller;

    public bool IsStopped => Time.timeScale != 0f;

    public bool IsGameEnded => TimeLeft <= 0;

    private float _timeLeft;
    public float TimeLeft
    {
        get => _timeLeft;
        private set
        {
            bool updateString = (int)value != (int)_timeLeft;
            _timeLeft = value;

            if(updateString)
                ViewModel.TimeLeft = ((int)_timeLeft).ToString();
        }
    }

    private int _points = 0;
    public int Points
    {
        get => _points;
        set
        {
            _points = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points)));
        }
    }

    private PianoNote ControllerHigherNoteWithOffset => Controller.HigherNote + Controller.C4Offset;
    public PianoNote ControllerLowerNoteWithOffset => Controller.LowerNote + Controller.C4Offset;

    private List<PianoNote> _controllerNotesWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesWithOffset => ControllerNotesWithOffset;

    private List<PianoNote> _controllerNotesDownWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesDownWithOffset => _controllerNotesDownWithOffset;

    private List<PianoNote> _controllerNotesUpWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesUpWithOffset => _controllerNotesUpWithOffset;

    private static GameController _instance;

    public event PropertyChangedEventHandler PropertyChanged;

    public static GameController Instance => _instance;

    private void OnEnable()
    {
        GameSceneManager.Instance.SceneLoaded += Instance_SceneLoaded;

        Transition.Opened += Transition_Opened;

        InputHandler.Guess += InputHandler_Guess;

        ViewModel.PlayAgain += ViewModel_PlayAgain;
        ViewModel.BackToMenu += ViewModel_BackToMenu;

        Transition.Closed += Transition_Closed;
        Transition.Opened += Transition_Opened1;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.SceneLoaded -= Instance_SceneLoaded;

        Transition.Opened -= Transition_Opened;

        InputHandler.Guess -= InputHandler_Guess;

        ViewModel.PlayAgain -= ViewModel_PlayAgain;
        ViewModel.BackToMenu -= ViewModel_BackToMenu;

        Transition.Closed -= Transition_Closed;
        Transition.Opened -= Transition_Opened1;

        Controller.Configuration -= Controller_Configuration;
    }

    private void Instance_SceneLoaded(object sender, SceneEventArgs e)
    {
        if (e.Scene.name == StaticResource.SCENE_MAIN_SCENE)
        {
            GameMode = GameSceneManager.Instance.GetValue<GameMode>(Enums.GetEnumDescription(SceneSessionKey.GameMode));

            Transition.SetPositionClose();
            StartCoroutine(Co_WaitForLoading());
        }
    }

    private void Awake()
    {
        if (GameController.Instance == null)
            GameController._instance = this;

        if (Staffs.Count == 0)
            throw new Exception("Staffs list is empty");

        Controller = ControllerFactory.GetControllerForType(ControllerType);
        Controller.Configuration += Controller_Configuration;

        // Applying offset to center keyboard on C4
        _controllerNotesWithOffset = Controller.Notes;
        _controllerNotesDownWithOffset = Controller.NotesDown;
        if (Controller.C4Offset != 0)
        {
            _controllerNotesWithOffset = _controllerNotesWithOffset.Select(x => x + Controller.C4Offset).ToList();
            _controllerNotesDownWithOffset = _controllerNotesDownWithOffset.Select(x => x + Controller.C4Offset).ToList();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Generate the staff lines
        for(int i = 0; i < Staffs.Count; i++)
        {
            var staff = Staffs[i];

            if (GameMode == GameMode.TrebbleBass)
                staff.transform.position = new Vector3(staff.transform.position.x, i == 0 ? YPositionFirstStaff : YPositionSecondStaff, staff.transform.position.z);
            else
            {
                staff.transform.position = new Vector3(staff.transform.position.x, 0f, staff.transform.position.z);

                if (GameMode == GameMode.Trebble && staff.StaffClef == Clef.Bass)
                    staff.gameObject.SetActive(false);
                else if (GameMode == GameMode.Bass && staff.StaffClef == Clef.Trebble)
                    staff.gameObject.SetActive(false);
            }
            
            staff.InitializeStaff();
        }

        ResetGame(false);

        ViewModel.InitializeViewModel();

        // StartConfiguringController(); // For testing
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeft -= Time.unscaledDeltaTime;

        if(TimeLeft <= 0) // Stop de game and show end screen
        {
            TimeScaleManager.PauseGame(0f);
            ViewModel.EndPanelVisible = !ViewModel.EndPanelVisible;
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            ViewModel.EndPanelVisible = !ViewModel.EndPanelVisible;
        }
    }

    private void InputHandler_Guess(object sender, GuessEventArgs e)
    {
        var firstnote = GetFirstNote();
        firstnote.SetInactive();
        firstnote.ChangeColor(e.Result ? StaticResource.COLOR_GOOD_GUESS : StaticResource.COLOR_BAD_GUESS);
        if (e.Result) Points++;
    }

    private void Transition_Opened(object sender, EventArgs e)
    {
        StartGame();
    }

    private void StartGame()
    {
        // Starting spawn note
        StartCoroutine(Co_SpawnNotes());
    }

    /// <summary>
    /// Controller configuration
    /// Pause the game (unpause when receiving end configuration event)
    /// </summary>
    private void StartConfiguringController()
    {
        TimeScaleManager.PauseGame(-1f);
        Controller.Configure();
    }

    /// <summary>
    /// End of configuration, unpause the game after 1 second (to not get keys immediatly)
    /// </summary>
    private void Controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        TimeScaleManager.PauseGame(1f);
    }

    /// <summary>
    /// Pause the game, setting Timescale to 0f and saving timescale before the pause
    /// </summary>
    public Note GetFirstNote()
    {
        var firstNote = Staffs.SelectMany(x => x.Notes).OrderBy(x => x.CreationTimestamp).Where(x => x.IsActive).FirstOrDefault();
        return firstNote;
    }

    private void ViewModel_BackToMenu(object sender, EventArgs e)
    {
        BackToMenu();
    }

    private void ViewModel_PlayAgain(object sender, EventArgs e)
    {
        ResetGame();
    }

    private void ResetGame(bool unpause = true)
    {
        Points = 0;
        TimeLeft = 10f;

        if(unpause)
            TimeScaleManager.UnpauseGameAfterSeconds(1f);
    }

    private void BackToMenu()
    {
        TimeLeft = 0f;

        foreach(var staff in Staffs)
        {
            staff.Notes.ForEach(x => x.Destroy());
        }

        ViewModel.HideAll();

        Transition.SetPositionOpen_2();
        Transition.Close();
    }

    private void Transition_Closed(object sender, EventArgs e)
    {
        GameSceneManager.Instance.LoadScene(StaticResource.SCENE_MAIN_MENU);
    }

    private void Transition_Opened1(object sender, EventArgs e)
    {
        ViewModel.ShowAll();
    }

    /// <summary>
    /// Coroutine : Spawn notes on staff every 0.5f (for timescale = 1f)
    /// </summary>
    /// <returns></returns>
    public IEnumerator Co_SpawnNotes()
    {
        while (true)
        {
            for(int i = 0; i < Staffs.Count; i++)
            {
                Staffs[i].SpawnNote(ControllerHigherNoteWithOffset, ControllerLowerNoteWithOffset);
                yield return new WaitForSeconds(0.5f / Staffs.Count);
            }
        }
    }

    private IEnumerator Co_WaitForLoading()
    {
        yield return new WaitForSecondsRealtime(.25f);
        Transition.Open_2();

    }
}
