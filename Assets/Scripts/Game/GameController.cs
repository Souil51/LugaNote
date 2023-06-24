using Assets.Scripts.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GameController : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [SerializeField] private Transition Transition;
    [SerializeField] private List<Staff> Staffs;
    [SerializeField] private ControllerType ControllerType; // Replace this with Dependancy Injection for Controller ?
    [SerializeField] private bool ReplacementMode; // Can a note replace the same note on other octave ?
    [SerializeField] private GameInputHandler InputHandler;
    [SerializeField] private TimeScaleManager TimeScaleManager;
    [SerializeField] private GameViewModel ViewModel;
    [SerializeField] private float YPositionFirstStaff;
    [SerializeField] private float YPositionSecondStaff;
    [SerializeField] private float OneStaffScale;
    [SerializeField] private float TwoStaffScale;
    [SerializeField] private GameMode GameMode;

    public GameState State
    {
        get { return _state; }
        private set 
        {
            _state = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
        }
    }

    private bool _isGameEnded = false;
    public bool IsGameEnded
    {
        get => _isGameEnded;
        private set
        {
            _isGameEnded = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGameEnded)));
        }
    }

    private bool _isGameStarted = false;
    public bool IsGameStarted
    {
        get => _isGameStarted;
        private set
        {
            _isGameStarted = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGameStarted)));
        }
    }

    private float _timeLeft;
    public float TimeLeft
    {
        get => _timeLeft;
        private set
        {
            bool updateString = (int)value != (int)_timeLeft || value > _timeLeft;
            _timeLeft = value;

            if(updateString)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeLeft)));
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

    public List<Staff> GameStaffs => Staffs;
    public bool IsStopped => Time.timeScale != 0f;
    public bool GameReplacementMode => ReplacementMode;
    public bool IsPaused => TimeScaleManager.IsPaused;

    private GameState _state = GameState.Loaded;
    private IController _controller;
    public IController Controller => _controller;
    private Coroutine _notesCoroutine = null;
    private static GameController _instance;
    
    public static GameController Instance => _instance;

    #region Unity methods
    private void OnEnable()
    {
        GameSceneManager.Instance.SceneLoaded += Instance_SceneLoaded;

        InputHandler.Guess      += InputHandler_Guess;
        InputHandler.Pause      += InputHandler_Pause;
        ViewModel.PlayAgain     += ViewModel_PlayAgain;
        ViewModel.BackToMenu    += ViewModel_BackToMenu;
        ViewModel.Resume        += ViewModel_Resume;
        Transition.Closed       += Transition_Closed;
        Transition.Opened       += Transition_Opened;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.SceneLoaded -= Instance_SceneLoaded;

        InputHandler.Guess          -= InputHandler_Guess;
        ViewModel.PlayAgain         -= ViewModel_PlayAgain;
        ViewModel.BackToMenu        -= ViewModel_BackToMenu;
        Transition.Closed           -= Transition_Closed;
        Transition.Opened           -= Transition_Opened;
        Controller.Configuration    -= Controller_Configuration;
    }

    private void Awake()
    {
        if (GameController.Instance == null)
            GameController._instance = this;

        if (Staffs.Count == 0)
            throw new Exception("Staffs list is empty");

        _controller = ControllerFactory.Instance.GetController();
        Controller.Configuration += Controller_Configuration;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Generate the staff lines
        for(int i = 0; i < Staffs.Count; i++)
        {
            var staff = Staffs[i];

            staff.transform.localScale = new Vector3(GameMode == GameMode.TrebbleBass ? TwoStaffScale : OneStaffScale, GameMode == GameMode.TrebbleBass ? TwoStaffScale : OneStaffScale, staff.transform.localScale.z);

            if (GameMode == GameMode.TrebbleBass)
                staff.transform.position = new Vector3(staff.transform.position.x, i == 0 ? YPositionFirstStaff : YPositionSecondStaff, staff.transform.position.z);
            else
            {
                staff.transform.position = new Vector3(staff.transform.position.x, 0f, staff.transform.position.z);

                if (GameMode == GameMode.Trebble && staff.StaffClef == Clef.Bass || GameMode == GameMode.Bass && staff.StaffClef == Clef.Trebble)
                    staff.gameObject.SetActive(false);
            }
            
            staff.InitializeStaff();
        }

        ViewModel.InitializeViewModel();

        // StartConfiguringController(); // For testing
    }

    // Update is called once per frame
    void Update()
    {
        if(State == GameState.Started)
            TimeLeft -= Time.unscaledDeltaTime;

        if(State == GameState.Started && TimeLeft <= 0 ) // Stop de game and show end screen
        {
            ChangeState(GameState.Ended);
        }
    }
    #endregion

    private void ChangeState(GameState newState)
    {
        bool changed = false;

        if ((State == GameState.Loaded || State == GameState.Ended || State == GameState.Paused)
            && newState == GameState.Starting)
        {
            foreach (var staff in Staffs)
            {
                staff.Notes.ForEach(x => x.Destroy());
            }

            IsGameEnded = false;
            IsGameStarted = false;

            Points = 0;
            TimeLeft = 5f;

            if (_notesCoroutine != null)
                StopCoroutine(_notesCoroutine);

            changed = true;
        }
        else if (State == GameState.Starting && newState == GameState.Started)
        {
            TimeScaleManager.UnpauseGame();

            IsGameStarted = true;
            _notesCoroutine = StartCoroutine(Co_SpawnNotes());

            changed = true;
        }
        else if (State == GameState.Started && newState == GameState.Ended)
        {
            TimeLeft = 0f;
            IsGameEnded = true;
            TimeScaleManager.PauseGame(0f);

            changed = true;
        }
        else if (State == GameState.Started && newState == GameState.Paused)
        {
            TimeScaleManager.PauseGame(0);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaused)));

            changed = true;
        }
        else if (State == GameState.Paused && newState == GameState.Started)
        {
            TimeScaleManager.UnpauseGame();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaused)));

            changed = true;
        }

        if (changed)
            State = newState;
    }

    #region Methods
    /// <summary>
    /// Pause the game, setting Timescale to 0f and saving timescale before the pause
    /// </summary>
    public Note GetFirstNote()
    {
        var firstNote = Staffs.SelectMany(x => x.Notes).OrderBy(x => x.CreationTimestamp).Where(x => x.IsActive).FirstOrDefault();
        return firstNote;
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

    private void NavigateToMenu()
    {
        ViewModel.HideAll();

        Transition.SetPositionOpen_2();
        Transition.Close();
    }
    #endregion

    #region Events callbacks
    private void Instance_SceneLoaded(object sender, SceneEventArgs e)
    {
        if (e.Scene.name == StaticResource.SCENE_MAIN_SCENE)
        {
            GameMode = GameSceneManager.Instance.GetValue<GameMode>(Enums.GetEnumDescription(SceneSessionKey.GameMode));

            Transition.SetPositionClose();
            StartCoroutine(Co_WaitForLoading());
        }
    }

    private void InputHandler_Guess(object sender, GuessEventArgs e)
    {
        var firstnote = GetFirstNote();
        firstnote.SetInactive();
        firstnote.ChangeColor(e.Result ? StaticResource.COLOR_GOOD_GUESS : StaticResource.COLOR_BAD_GUESS);
        if (e.Result) Points++;
    }

    private void InputHandler_Pause(object sender, EventArgs e)
    {
        if (State == GameState.Started)
            ChangeState(GameState.Paused);
        else if (State == GameState.Paused)
            ChangeState(GameState.Started);
    }

    /// <summary>
    /// End of configuration, unpause the game after 1 second (to not get keys immediatly)
    /// </summary>
    private void Controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        TimeScaleManager.PauseGame(1f);
    }

    private void ViewModel_BackToMenu(object sender, EventArgs e)
    {
        ChangeState(GameState.Starting);
        NavigateToMenu();
    }

    private void ViewModel_PlayAgain(object sender, EventArgs e)
    {
        if (State == GameState.Ended || State == GameState.Paused)
        {
            ChangeState(GameState.Starting);
            ChangeState(GameState.Started);
        }
    }

    private void ViewModel_Resume(object sender, EventArgs e)
    {
        if (State == GameState.Paused)
            ChangeState(GameState.Started);
    }

    private void Transition_Closed(object sender, EventArgs e)
    {
        GameSceneManager.Instance.LoadScene(StaticResource.SCENE_MAIN_MENU);
    }

    private void Transition_Opened(object sender, EventArgs e)
    {
        ViewModel.ShowAll();
        ChangeState(GameState.Starting);
        ChangeState(GameState.Started);
    }
    #endregion

    #region Coroutines
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
                Staffs[i].SpawnNote(Controller.HigherNoteWithOffset, Controller.LowerNoteWithOffset);
                yield return new WaitForSeconds(0.5f / Staffs.Count);
            }
        }
    }

    private IEnumerator Co_WaitForLoading()
    {
        yield return new WaitForSecondsRealtime(.25f);
        Transition.Open_2();
    }
    #endregion
}
