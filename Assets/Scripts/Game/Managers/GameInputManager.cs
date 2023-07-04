using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public delegate void GuessEventHandler(object sender, GuessEventArgs e);
    public event GuessEventHandler Guess;

    public delegate void PauseEventHandler(object sender, EventArgs e);
    public event PauseEventHandler Pause;

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

        if(!GameController.Instance.IsGameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause?.Invoke(this, EventArgs.Empty);
            }

            if (firstNote != null && !GameController.Instance.IsPaused)
            {
                if (GameController.Instance.Controller.NotesDownWithOffset.Count > 0)
                {
                    SoundManager.PlayNote(GameController.Instance.Controller.NotesDownWithOffset[0]);
                    Debug.Log(GameController.Instance.Controller.NotesDownWithOffset[0]);
                    bool? guess = null;
                    bool? guessValue = null;
                    // Normal mode : the note has to be the exact same note
                    // Replacement mode : the note has to be the same note, no matter the octave (note % 12 == 0)
                    if (
                            GameController.Instance.Controller.NotesDownWithOffset.Count == 1
                            &&
                            (
                                (!GameController.Instance.GameReplacementMode && GameController.Instance.Controller.NotesDownWithOffset[0] == firstNote.PianoNote)
                                ||
                                ((int)(GameController.Instance.Controller.NotesDownWithOffset[0]) % 12 == (int)(firstNote.PianoNote) % 12)
                            )
                        )
                    {
                        // Good guess
                        // Debug.Log("Good guess");
                        firstNote.ChangeColor(StaticResource.COLOR_GOOD_GUESS);
                        guessValue = true;
                    }
                    else
                    {
                        // Bad guess
                        // Debug.Log("Bad guess");
                        firstNote.ChangeColor(StaticResource.COLOR_BAD_GUESS);
                        guessValue = false;
                    }

                    if (guessValue.HasValue)
                    {
                        Guess?.Invoke(this, new GuessEventArgs(guessValue.Value));
                    }
                }

                // For testing
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Guess?.Invoke(this, new GuessEventArgs(true));
                }
            }
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