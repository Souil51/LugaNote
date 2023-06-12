 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

/// <summary>
/// 
/// </summary>
public class GameController : GameControllerBase
{
    [SerializeField] private Transition Transition;
    [SerializeField] private List<Staff> Staffs;
    [SerializeField] private ControllerType ControllerType; // Replace this with Dependancy Injection for Controller ?
    [SerializeField] private bool ReplacementMode; // Can a note replace the same note on other octave ?

    public List<Staff> GameStaffs => Staffs;
    public bool GameReplacementMode => ReplacementMode;

    private IController Controller;

    public bool IsStopped => Time.timeScale != 0f;

    private int _points = 0;
    public int Points
    {
        get => _points;
        private set
        {
            _points = value;
            OnPropertyChanged();
        }
    }

    public string TimeLeftString => ((int)TimeLeft).ToString();

    private float _timeLeft = 0;
    public float TimeLeft
    {
        get => _timeLeft;
        private set
        {
            bool updateString = (int)value != (int)_timeLeft;

            _timeLeft = value;
            OnPropertyChanged();

            if(updateString)
                OnPropertyChanged("TimeLeftString");
        }
    }

    private int _C4Offset = 0;

    private PianoNote ControllerHigherNoteWithOffset => Controller.HigherNote + _C4Offset;
    public PianoNote ControllerLowerNoteWithOffset => Controller.LowerNote + _C4Offset;

    private List<PianoNote> _controllerNotesWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesWithOffset => ControllerNotesWithOffset;

    private List<PianoNote> _controllerNotesDownWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesDownWithOffset => _controllerNotesDownWithOffset;

    private List<PianoNote> _controllerNotesUpWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesUpWithOffset => _controllerNotesUpWithOffset;

    // Pause var
    private float _lastTimeScale;
    private float _pauseTimer;
    public bool IsPaused => _pauseTimer != 0;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("SceneManager_sceneLoaded");
        Transition.Opened += Transition_Opened;
        Transition.Open_2();
    }

    private void Awake()
    {
        Debug.Log("Awake");
        if (Staffs.Count == 0)
            throw new Exception("Staffs list is empty");

        InitialiserNotifyPropertyChanged();

        // Instantiating the controller
        // Replace this with dependancy injection later
        switch (ControllerType)
        {
            case ControllerType.MIDI:
                {
                    Controller = gameObject.AddComponent(typeof(MidiController)) as MidiController;
                    Controller.Configuration += Controller_Configuration;

                    // For MIDI keyboard with reduced note count, keyboard will be centered on C4
                    var middleC = MusicHelper.GetMiddleCBetweenTwoNotes(Controller.HigherNote, Controller.LowerNote);
                    _C4Offset = PianoNote.C4 - middleC;
                }
                break;
            case ControllerType.Visual:
                Controller = gameObject.AddComponent(typeof(VisualController)) as VisualController;
                break;
            case ControllerType.Keyboard:
            default:
                Controller = gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                break;
        }

        // Applying offset to center keyboard on C4
        _controllerNotesWithOffset = Controller.Notes;
        _controllerNotesDownWithOffset = Controller.NotesDown;
        if (_C4Offset != 0)
        {
            _controllerNotesWithOffset = _controllerNotesWithOffset.Select(x => x + _C4Offset).ToList();
            _controllerNotesDownWithOffset = _controllerNotesDownWithOffset.Select(x => x + _C4Offset).ToList();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        // Generate the staff lines
        Staffs.ForEach(x => x.InitializeStaff());

        Points = 0;
        TimeLeft = 60f;

        // For testing
        // StartConfiguringController();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPaused)
            UpdatePause();

        TimeLeft -= Time.unscaledDeltaTime;

        // Guessing system
        var firstNote = GetFirstNote();
        // Debug.Log("First note : " + MusicHelper.GetNoteCommonName(firstNote.PianoNote));

        if (firstNote != null)
        {
            if(_controllerNotesDownWithOffset.Count > 0)
            {
                bool? guess = null;
                // Normal mode : the note has to be the exact same note
                // Replacement mode : the note has to be the same note, no matter the octave (note % 12 == 0)
                if (
                        _controllerNotesDownWithOffset.Count == 1 
                        && 
                        (
                            (!ReplacementMode && _controllerNotesDownWithOffset[0] == firstNote.PianoNote)
                            ||
                            ((int)(_controllerNotesDownWithOffset[0]) % 12 == (int)(firstNote.PianoNote) % 12)
                        )
                    )
                {
                    // Good guess
                    // Debug.Log("Good guess");
                    firstNote.ChangeColor(StaticResource.COLOR_GOOD_GUESS);
                    guess = true;
                }
                else
                {
                    // Bad guess
                    // Debug.Log("Bad guess");
                    firstNote.ChangeColor(StaticResource.COLOR_BAD_GUESS);
                    guess = false;
                }

                if (guess.HasValue)
                {
                    // Debug.Log("Guess " + firstNote.Parent.Note);
                    firstNote.SetInactive();
                    firstNote = GetFirstNote();

                    if (guess.Value)
                        Points++;
                }
            }

            // For testing
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Debug.Log("Guess " + firstNote.Parent.Note);
                firstNote.ChangeColor(StaticResource.COLOR_GOOD_GUESS);
                firstNote.SetInactive();
                Points++;

                firstNote = GetFirstNote();
            }

            // Update the timescale to slow down notes while they are approching the start of the staff
            if (firstNote != null)
            {
                var firstNoteStaff = firstNote.Parent.Parent;
                // timescale based on the first Staff
                var totalDistance = firstNoteStaff.StartingPointPosition - firstNoteStaff.EndingPointPosition;
                var distanceToEnd = firstNote.transform.localPosition.x - firstNoteStaff.EndingPointPosition;

                float newTimeScale = distanceToEnd / totalDistance;
                if (newTimeScale > 0.05f) // deadzone
                    Time.timeScale = distanceToEnd / totalDistance;
                else
                    Time.timeScale = 0f;
            }
        }
    }

    private void StartGame()
    {
        // Starting spawn note
        StartCoroutine(Co_SpawnNotes());
    }

    private void Transition_Opened(object sender, EventArgs e)
    {
        StartGame();
    }

    /// <summary>
    /// Controller configuration
    /// Pause the game (unpause when receiving end configuration event)
    /// </summary>
    private void StartConfiguringController()
    {
        PauseGame(-1f);
        Controller.Configure();
    }

    /// <summary>
    /// End of configuration, unpause the game after 1 second (to not get keys immediatly)
    /// </summary>
    private void Controller_Configuration(object sender, ConfigurationEventArgs e)
    {
        PauseGame(1f);
    }

    /// <summary>
    /// Pause the game, setting Timescale to 0f and saving timescale before the pause
    /// </summary>
    private void PauseGame(float duration)
    {
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        _pauseTimer = duration;
    }

    /// <summary>
    /// Update the pause timer and unpause if the timer is ended
    /// </summary>
    private void UpdatePause()
    {
        if (_pauseTimer != 0) // Pause can be positive or negative (= unlimited pause)
        {
            if (_pauseTimer > 0)
            {
                _pauseTimer -= Time.unscaledDeltaTime;
                if (_pauseTimer < 0)
                {
                    UnpauseGame();
                }
            }

            return;
        }
    }


    /// <summary>
    /// Resume the game after a pause
    /// </summary>
    private void UnpauseGame()
    {
        _pauseTimer = 0f;
        Time.timeScale = _lastTimeScale;
    }

    public Note GetFirstNote()
    {
        var firstNote = Staffs.SelectMany(x => x.Notes).OrderBy(x => x.CreationTimestamp).Where(x => x.IsActive).FirstOrDefault();
        return firstNote;
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
}
