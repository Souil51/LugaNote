using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameInputHandler : MonoBehaviour
{
    public delegate void GuessEventHandler(object sender, GuessEventArgs e);
    public event GuessEventHandler Guess;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Guessing system
        var firstNote = GameController.Instance.GetFirstNote();
        // Debug.Log("First note : " + MusicHelper.GetNoteCommonName(firstNote.PianoNote));

        if (firstNote != null)
        {
            if (GameController.Instance.ControllerNotesDownWithOffset.Count > 0)
            {
                bool? guess = null;
                // Normal mode : the note has to be the exact same note
                // Replacement mode : the note has to be the same note, no matter the octave (note % 12 == 0)
                if (
                        GameController.Instance.ControllerNotesDownWithOffset.Count == 1
                        &&
                        (
                            (!GameController.Instance.GameReplacementMode && GameController.Instance.ControllerNotesDownWithOffset[0] == firstNote.PianoNote)
                            ||
                            ((int)(GameController.Instance.ControllerNotesDownWithOffset[0]) % 12 == (int)(firstNote.PianoNote) % 12)
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
                    Guess?.Invoke(this, new GuessEventArgs(guess.Value));

                    // Debug.Log("Guess " + firstNote.Parent.Note);
                    //firstNote.SetInactive();
                    //firstNote = _controller.GetFirstNote();

                    //if (guess.Value)
                    //    Points++;
                }
            }

            // For testing
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Guess?.Invoke(this, new GuessEventArgs(true));

                // Debug.Log("Guess " + firstNote.Parent.Note);
                //firstNote.ChangeColor(StaticResource.COLOR_GOOD_GUESS);
                //firstNote.SetInactive();
                //Points++;

                // firstNote = GameController.Instance.GetFirstNote();
            }

            // Recall the method to get the potential next first note (because the current is disabled ?)
            // firstNote = GameController.Instance.GetFirstNote();
            // Update the timescale to slow down notes while they are approching the start of the staff
            //if (firstNote != null)
            //{
            //    var firstNoteStaff = firstNote.Parent.Parent;
            //    // timescale based on the first Staff
            //    var totalDistance = firstNoteStaff.StartingPointPosition - firstNoteStaff.EndingPointPosition;
            //    var distanceToEnd = firstNote.transform.localPosition.x - firstNoteStaff.EndingPointPosition;

            //    float newTimeScale = distanceToEnd / totalDistance;
            //    if (newTimeScale > 0.05f) // deadzone
            //        Time.timeScale = distanceToEnd / totalDistance;
            //    else
            //        Time.timeScale = 0f;
            //}
        }
    }
}

public class GuessEventArgs : EventArgs
{
    public bool Result;

    public GuessEventArgs(bool result)
    {
        Result = result;
    }
}