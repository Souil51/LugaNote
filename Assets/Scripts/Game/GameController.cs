using Assets.Scripts.Data;
using Assets.Scripts.Game;
using Assets.Scripts.Game.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Game.Save;

/// <summary>
/// 
/// </summary>
public class GameController : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    #region Properties
    [SerializeField] private Transition Transition;
    [SerializeField] private List<Staff> Staffs;
    [SerializeField] private ControllerType ControllerType; // Replace this with Dependancy Injection for Controller ?
    [SerializeField] private GameInputManager InputHandler;
    [SerializeField] private TimeScaleManager TimeScaleManager;
    [SerializeField] private GameViewModel ViewModel;
    [SerializeField] private float YPositionFirstStaff;
    [SerializeField] private float YPositionSecondStaff;
    [SerializeField] private float OneStaffScale;
    [SerializeField] private float TwoStaffScale;
    [SerializeField] private float StaffSeparatorYPosition;
    
    private GameMode _gameMode;
    public GameMode GameMode => _gameMode;

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

    private bool _gameReplacementMode = true;
    public bool GameReplacementMode => _gameReplacementMode;
    public bool IsPaused => TimeScaleManager.IsPaused;

    public bool HasControllerUI => _controller.HasUI;

    private GameState _state = GameState.Loaded;
    private IController _controller;
    public IController Controller => _controller;
    private Coroutine _notesCoroutine = null;
    private static GameController _instance;
    
    public static GameController Instance => _instance;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        GameSceneManager.Instance.SceneLoaded += GameSceneManager_SceneLoaded;

        InputHandler.Guess      += InputHandler_Guess;
        InputHandler.Pause      += InputHandler_Pause;
        ViewModel.PlayAgain     += ViewModel_PlayAgain;
        ViewModel.NavigateToMenu    += ViewModel_NavigateToMenu;
        ViewModel.Resume        += ViewModel_Resume;
        ViewModel.OpenMenu  += ViewModel_OpenMenu;
        ViewModel.ToggleVisualKeysVisibility += ViewModel_ToggleVisualKeysVisibility;
        Transition.Closed       += Transition_Closed;
        Transition.Opened       += Transition_Opened;
    }

    private void OnDisable()
    {
        GameSceneManager.Instance.SceneLoaded -= GameSceneManager_SceneLoaded;

        InputHandler.Guess          -= InputHandler_Guess;
        ViewModel.PlayAgain         -= ViewModel_PlayAgain;
        ViewModel.NavigateToMenu        -= ViewModel_NavigateToMenu;
        ViewModel.Resume            -= ViewModel_Resume;
        ViewModel.OpenMenu      -= ViewModel_OpenMenu;
        ViewModel.ToggleVisualKeysVisibility -= ViewModel_ToggleVisualKeysVisibility;
        Transition.Closed           -= Transition_Closed;
        Transition.Opened           -= Transition_Opened;
    }

    private void Awake()
    {
        if (GameController.Instance == null)
            GameController._instance = this;

        if (Staffs.Count == 0)
            throw new Exception("Staffs list is empty");

        _controller = ControllerFactory.Instance.GetController();
    }

    // Start is called before the first frame update
    void Start()
    {
        // For testing
        if (GameMode == null)
        {
            _gameMode = new GameMode(1, GameModeType.Trebble, IntervalMode.Note, Level.C3_C6, false);
        }

        // Generate the staff lines
        for (int i = 0; i < Staffs.Count; i++)
        {
            var staff = Staffs[i];
                
            staff.transform.localScale = new Vector3(GameMode.GameModeType == GameModeType.TrebbleBass ? TwoStaffScale : OneStaffScale, GameMode.GameModeType == GameModeType.TrebbleBass ? TwoStaffScale : OneStaffScale, staff.transform.localScale.z);

            if (GameMode.GameModeType == GameModeType.TrebbleBass)
            {
                staff.transform.position = new Vector3(staff.transform.position.x, i == 0 ? YPositionFirstStaff : YPositionSecondStaff, staff.transform.position.z);

                var go = Instantiate(Resources.Load(StaticResource.PREFAB_DOTTED_LINE)) as GameObject;
                go.transform.position = new Vector3(0, StaffSeparatorYPosition, 0);
            }
            else
            {
                staff.transform.position = new Vector3(staff.transform.position.x, 0f, staff.transform.position.z);

                if (GameMode.GameModeType == GameModeType.Trebble && staff.StaffClef == Clef.Bass || GameMode.GameModeType == GameModeType.Bass && staff.StaffClef == Clef.Trebble)
                    staff.gameObject.SetActive(false);
            }
            
            if(staff.gameObject.activeSelf)
                staff.InitializeStaff();
        }

        ViewModel.InitializeViewModel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Break();
        }

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
            TimeLeft = 60f;

            if (_notesCoroutine != null)
                StopCoroutine(_notesCoroutine);

            changed = true;
        }
        else if (State == GameState.Starting && newState == GameState.Started)
        {
            _controller.ShowControllerUI();

            TimeScaleManager.UnpauseGame();

            IsGameStarted = true;
            _notesCoroutine = StartCoroutine(Co_SpawnNotes());

            changed = true;
        }
        else if (State == GameState.Started && newState == GameState.Ended)
        {
            _controller.HideControllerUI();

            TimeLeft = 0f;
            IsGameEnded = true;
            TimeScaleManager.PauseGame(0f);

            SaveManager.AddScore(GameMode, Points, DateTime.Now);
            SaveManager.SaveGame();
            changed = true;
        }
        else if (State == GameState.Started && newState == GameState.Paused)
        {
            _controller.HideControllerUI();

            TimeScaleManager.PauseGame(0);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaused)));

            changed = true;
        }
        else if (State == GameState.Paused && newState == GameState.Started)
        {
            _controller.ShowControllerUI();

            TimeScaleManager.UnpauseGame();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaused)));

            changed = true;
        }
        else if(newState == GameState.Navigating)
        {
            TimeScaleManager.UnpauseGame();
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

    public List<Note> GetFirstNotes()
    {
        var firstNotes = new List<Note>();
        var nearestNotesGroupId = Staffs.SelectMany(x => x.Notes).OrderBy(x => x.CreationTimestamp).Where(x => x.IsActive).FirstOrDefault()?.GroupId;
        if(nearestNotesGroupId != Guid.Empty)
        {
            firstNotes = Staffs.SelectMany(x => x.Notes).Where(x => x.GroupId == nearestNotesGroupId.Value).ToList();
        }
        return firstNotes;
    }

    private void NavigateToMenu()
    {
        ViewModel.HideAll();

        Transition.SetPositionOpen_2();
        Transition.Close();
    }
    #endregion

    #region Events callbacks
    private void GameSceneManager_SceneLoaded(object sender, SceneEventArgs e)
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;

        if (e.Scene.name == StaticResource.SCENE_MAIN_SCENE)
        {
            _gameMode = GameSceneManager.Instance.GetValue<GameMode>(Enums.GetEnumDescription(SceneSessionKey.GameMode));
            _gameReplacementMode = GameSceneManager.Instance.GetValue<bool>(Enums.GetEnumDescription(SceneSessionKey.ReplacementMode));

            Transition.SetPositionClose();
            StartCoroutine(Co_WaitForLoading());
        }
    }

    private void InputHandler_Guess(object sender, GuessEventArgs e)
    {
        var firstnotes = GetFirstNotes();

        if (e.Result)
        {
            Points++;

            // Mode a note from the good guess to the score
            var notePoint = Instantiate(Resources.Load(StaticResource.PREFAB_NOTE_NO_LINE)) as GameObject;
            foreach(var note in firstnotes)
            {
                notePoint.transform.position = note.transform.position;
                // var pos = new Vector3(ScreenManager.ScreenWidth / 2f, ScreenManager.ScreenHeight / 2f, 1);
                var pos = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.95f));

                notePoint.transform.DOScale(0.1f, 1f).SetUpdate(true);

                notePoint.transform.DOMove(pos, 1f).SetUpdate(true).OnKill(() =>
                {
                    Destroy(notePoint);
                });
            }
        };

        firstnotes.ForEach(x => x.SetInactive());
        // firstnote.ChangeColor(e.Result ? StaticResource.COLOR_GOOD_GUESS : StaticResource.COLOR_BAD_GUESS);
    }

    private void InputHandler_Pause(object sender, EventArgs e)
    {
        if (State == GameState.Started)
            ChangeState(GameState.Paused);
        else if (State == GameState.Paused)
            ChangeState(GameState.Started);
    }

    private void ViewModel_NavigateToMenu(object sender, EventArgs e)
    {
        ChangeState(GameState.Starting);
        ChangeState(GameState.Navigating);
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

    private void ViewModel_OpenMenu(object sender, EventArgs e)
    {
        if (State == GameState.Started)
            ChangeState(GameState.Paused);
    }

    private void ViewModel_ToggleVisualKeysVisibility(object sender, EventArgs e)
    {
        if(_controller.IsControllerUIVisible)
            _controller.HideControllerUI();
        else
            _controller.ShowControllerUI();
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

    // TEST
    // For testing all notes in order with alterations
    private List<PianoNote> _listeTEST = new List<PianoNote>()
    {
        PianoNote.C4,
        PianoNote.D4,
        PianoNote.E4,
        PianoNote.F4,
        PianoNote.G4,
        PianoNote.A4,
        PianoNote.B4,
    };

    // TEST

    /// <summary>
    /// Coroutine : Spawn notes on staff every 0.5f (for timescale = 1f)
    /// </summary>
    public IEnumerator Co_SpawnNotes()
    {
        var staffs = Staffs.Where(x => x.gameObject.activeSelf).ToList();

        while (true)
        {
            switch (GameMode.IntervalMode) 
            {
                case IntervalMode.Note:
                    {
                        for (int i = 0; i < staffs.Count; i++)
                        {
                            // For testing all notes in order with alterations
                            /*for(int j = 0; j < _listeTEST.Count; j++)
                            {
                                if (_listeTEST[j] == PianoNote.C4)
                                {
                                    for(int k = 0; k < 2; k++)
                                    {
                                        if(k == 0)
                                            Staffs.Where(x => x.gameObject.activeSelf).ToList()[i].SpawnNote(new List<PianoNote>() { (PianoNote)((int)_listeTEST[j]) }, Alteration.Natural);
                                        else
                                            Staffs.Where(x => x.gameObject.activeSelf).ToList()[i].SpawnNote(new List<PianoNote>() { (PianoNote)((int)_listeTEST[j]) }, Alteration.Sharp);

                                        yield return new WaitForSeconds(0.5f / Staffs.Count);
                                    }
                                }
                                else if (_listeTEST[j] == PianoNote.B4)
                                {
                                    for (int k = 0; k < 2; k++)
                                    {
                                        if(k == 0)
                                            Staffs.Where(x => x.gameObject.activeSelf).ToList()[i].SpawnNote(new List<PianoNote>() { (PianoNote)((int)_listeTEST[j]) }, Alteration.Flat);
                                        else if(k == 1)
                                            Staffs.Where(x => x.gameObject.activeSelf).ToList()[i].SpawnNote(new List<PianoNote>() { (PianoNote)((int)_listeTEST[j]) }, Alteration.Natural);
                                        
                                        yield return new WaitForSeconds(0.5f / Staffs.Count);
                                    }
                                }
                                else
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        if(k == 0)
                                            Staffs.Where(x => x.gameObject.activeSelf).ToList()[i].SpawnNote(new List<PianoNote>() { (PianoNote)((int)_listeTEST[j]) }, Alteration.Flat);
                                        else if(k == 1)
                                            Staffs.Where(x => x.gameObject.activeSelf).ToList()[i].SpawnNote(new List<PianoNote>() { (PianoNote)((int)_listeTEST[j]) }, Alteration.Natural);
                                        else
                                            Staffs.Where(x => x.gameObject.activeSelf).ToList()[i].SpawnNote(new List<PianoNote>() { (PianoNote)((int)_listeTEST[j]) }, Alteration.Sharp);

                                        yield return new WaitForSeconds(0.5f / Staffs.Count);
                                    }
                                }
                            }*/

                            var noteList = MusicHelper.GetNotesForLevel(this.GameMode.Level);

                            // staffs[i].SpawnNote(Controller.AvailableNotes, GameMode.WithRandomAlteration);
                            staffs[i].SpawnNote(noteList, GameMode.WithRandomAlteration);
                            yield return new WaitForSeconds(0.5f / Staffs.Count);
                        }
                    }
                    break;
                case IntervalMode.Interval:
                    {
                        int noteCount = MusicHelper.GetNotesCountForInterval(this.GameMode.IntervalMode);
                        var noteList = MusicHelper.GetNotesForLevel(this.GameMode.Level);
                        
                        for (int i = 0; i < staffs.Count; i++)
                        {
                            staffs[i].SpawnMultipleNotes(noteCount, noteList, GameMode.WithRandomAlteration);
                            yield return new WaitForSeconds(0.5f / Staffs.Count);
                        }
                    }
                    break;
                case IntervalMode.Chord:
                    {
                        int noteCount = MusicHelper.GetNotesCountForInterval(this.GameMode.IntervalMode);
                        var noteList = MusicHelper.GetNotesForLevel(this.GameMode.Level);

                        for (int i = 0; i < staffs.Count; i++)
                        {
                            staffs[i].SpawnMultipleNotes(noteCount, noteList, GameMode.WithRandomAlteration);
                            yield return new WaitForSeconds(0.5f / Staffs.Count);
                        }
                    }
                    break;
                default:
                    yield return new WaitForSeconds(0.5f / Staffs.Count);
                    break;
            }
        }
    }

    /// <summary>
    /// Wait 0.25s before opening the transition, to wait all loading
    /// </summary>
    private IEnumerator Co_WaitForLoading()
    {
        yield return new WaitForSecondsRealtime(.25f);
        Transition.Open_2();
    }
    #endregion
}
