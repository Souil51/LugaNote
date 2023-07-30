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

        if (!GameController.Instance.IsGameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause?.Invoke(this, EventArgs.Empty);
            }

            if (firstNote != null && !GameController.Instance.IsPaused)
            {
                if (GameController.Instance.Controller.NotesDownWithOffset.Count > 0)
                {
                    Debug.Log(firstNote.PianoNote);
                    Debug.Log(GameController.Instance.Controller.NotesDownWithOffset[0]);
                    bool? guessValue = null;

                    if (GameController.Instance.Controller.NotesDownWithOffset.Count == 1)
                    {
                        var firstControllerNote = GameController.Instance.Controller.NotesDownWithOffset[0];
                        bool replaceNote = firstControllerNote.IsReplaceableByDefault || GameController.Instance.GameReplacementMode;

                        // Normal mode : the note has to be the exact same note
                        // Replacement mode : the note has to be the same note, no matter the octave (note % 12 == 0)
                        if (
                                (
                                    (!replaceNote && firstControllerNote.Note == firstNote.PianoNote)
                                    ||
                                    (replaceNote && (int)firstControllerNote.Note % 12 == (int)(firstNote.PianoNote) % 12)
                                )
                            )
                        {
                            SoundManager.PlayNote(firstControllerNote.Note);
                            // Good guess
                            firstNote.ShowGoodGuess();
                            guessValue = true;
                        }
                        else
                        {
                            SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS);
                            // Bad guess
                            firstNote.ShowBadGuess();
                            guessValue = false;
                        }
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