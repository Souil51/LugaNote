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

    private void Awake()
    {
        StartCoroutine(Co_SpawnNotes());
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (ControllerType)
        {
            case ControllerType.MIDI:
                Controller = gameObject.AddComponent(typeof(MidiController)) as MidiController;
                break;
            case ControllerType.Keyboard:
            default:
                Controller = gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.Notes.Count > 0)
            Debug.Log(Controller.Notes[0]);

        var firstNote = Staff.Notes.Where(x => x.IsActive).FirstOrDefault();

        if (firstNote != null)
        {
            if(Controller.NotesDown.Count > 0)
            {
                bool guessDone = false;
                if (
                        Controller.NotesDown.Count == 1 
                        && 
                        (
                            (!ReplacementMode && Controller.NotesDown[0] == firstNote.Parent.Note)
                            ||
                            ((int)(Controller.NotesDown[0]) % 12 == (int)(firstNote.Parent.Note) % 12)
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
