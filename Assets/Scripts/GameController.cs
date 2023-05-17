using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Staff Staff;
    [SerializeField] private ControllerType ControllerType; // Replace this with Dependancy Injection for Controller ?
    [SerializeField] private bool ReplacementMode; // Can a note replace the same note on other octave ?

    private IController Controller;

    public bool IsStopped => Time.timeScale != 0f;

    private int _points = 0;

    private int _C4Offset = 0;

    private PianoNote ControllerHigherNoteWithOffset => Controller.HigherNote + _C4Offset;
    private PianoNote ControllerLowerNoteWithOffset => Controller.LowerNote + _C4Offset;

    private List<PianoNote> _controllerNotesWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesWithOffset => ControllerNotesWithOffset;

    private List<PianoNote> _controllerNotesDownWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesDownWithOffset => _controllerNotesDownWithOffset;

    private List<PianoNote> _controllerNotesUpWithOffset = new List<PianoNote>();
    public List<PianoNote> ControllerNotesUpWithOffset => _controllerNotesUpWithOffset;

    private void Awake()
    {
        switch (ControllerType)
        {
            case ControllerType.MIDI:
                {
                    Controller = gameObject.AddComponent(typeof(MidiController)) as MidiController;

                    // For MIDI keyboard with reduced note count, keyboard will be centered on C4
                    var middleC = StaticResource.GetMiddleCBetweenTwoNotes(Controller.HigherNote, Controller.LowerNote);
                    _C4Offset = PianoNote.C4 - middleC;
                }
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

        Staff.InitializeStaff();

        StartCoroutine(Co_SpawnNotes());


        GameObject test = new GameObject();
        var go = Instantiate(test, Vector3.zero, Quaternion.identity) as GameObject;
        var config = go.AddComponent<MidiConfigurationHelper>();
        config.Controller = Controller;
        config.ConfigurationEnded += Config_ConfigurationEnded;
    }

    private void Config_ConfigurationEnded(object sender, MidiConfigurationReturn e)
    {
        int i = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Guessing system
        var firstNote = Staff.Notes.Where(x => x.IsActive).FirstOrDefault();

        if (firstNote != null)
        {
            if(_controllerNotesDownWithOffset.Count > 0)
            {
                bool guessDone = false;
                if (
                        _controllerNotesDownWithOffset.Count == 1 
                        && 
                        (
                            (!ReplacementMode && _controllerNotesDownWithOffset[0] == firstNote.Parent.Note)
                            ||
                            ((int)(_controllerNotesDownWithOffset[0]) % 12 == (int)(firstNote.Parent.Note) % 12)
                        )
                    )
                {
                    // Good guess
                    Debug.Log("Good guess");
                    firstNote.ChangeColor(StaticResource.COLOR_GOOD_GUESS);
                    guessDone = true;
                }
                else
                {
                    // Bad guess
                    Debug.Log("Bad guess");
                    firstNote.ChangeColor(StaticResource.COLOR_BAD_GUESS);
                    guessDone = true;
                }

                if (guessDone)
                {
                    firstNote.SetInactive();
                    _points++;

                    firstNote = Staff.Notes.Where(x => x.IsActive).FirstOrDefault();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                firstNote.SetInactive();
                _points++;

                firstNote = Staff.Notes.Where(x => x.IsActive).FirstOrDefault();
            }

            if (firstNote != null)
            {
                var totalDistance = Staff.StartingPointPosition - Staff.EndingPointPosition;
                var distanceToEnd = firstNote.transform.position.x - Staff.EndingPointPosition;

                float newTimeScale = distanceToEnd / totalDistance;
                if (newTimeScale > 0.05f)
                    Time.timeScale = distanceToEnd / totalDistance;
                else
                    Time.timeScale = 0f;
            }
        }

        // Debug.Log("Timescale : " + Time.timeScale);
    }

    public IEnumerator Co_SpawnNotes()
    {
        while (true)
        {
            Staff.SpawnNote(ControllerHigherNoteWithOffset, ControllerLowerNoteWithOffset);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
