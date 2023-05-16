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

        Staff.InitializeStaff(Controller.HigherNote, Controller.LowerNote);

        StartCoroutine(Co_SpawnNotes());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Applying offset to center keyboard on C4
        var notesOffset = Controller.Notes;
        var notesDownOffset = Controller.NotesDown;

        if(_C4Offset != 0)
        {
            notesOffset = notesOffset.Select(x => x + _C4Offset).ToList();
            notesDownOffset = notesDownOffset.Select(x => x + _C4Offset).ToList();
        }

        // Guessing system
        var firstNote = Staff.Notes.Where(x => x.IsActive).FirstOrDefault();

        if (firstNote != null)
        {
            if(notesDownOffset.Count > 0)
            {
                bool guessDone = false;
                if (
                        notesDownOffset.Count == 1 
                        && 
                        (
                            (!ReplacementMode && notesDownOffset[0] == firstNote.Parent.Note)
                            ||
                            ((int)(notesDownOffset[0]) % 12 == (int)(firstNote.Parent.Note) % 12)
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
            Staff.SpawnNote();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
