using Assets.Scripts.Game.Model;
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

    // Used to store pending notes when playing intervals/chords mode
    private List<ControllerNote> _noteBuffer = new List<ControllerNote>();
    private int maxNoteCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        maxNoteCount = (int)GameController.Instance.GameMode.IntervalMode + 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Guessing system
        var firstSingleNote = GameController.Instance.GetFirstNote();
        var firstNotes = GameController.Instance.GetFirstNotes();

        if (!GameController.Instance.IsGameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause?.Invoke(this, EventArgs.Empty);
            }

            if (firstNotes.Count > 0 && !GameController.Instance.IsPaused)
            {
                // Store notes pressed
                foreach (var note in GameController.Instance.Controller.NotesDownWithOffset)
                {
                    if (!_noteBuffer.Contains(note))
                    {
                        _noteBuffer.Add(note);
                    }
                }

                foreach (var note in GameController.Instance.Controller.NotesUpWithOffset)
                {
                    if (_noteBuffer.Contains(note))
                    {
                        _noteBuffer.Remove(note);
                    }
                }

                // If buffer > maxNoteCount of the interval mode : bad guess (happens when user press multiple key in the same frame)
                if(_noteBuffer.Count > maxNoteCount)
                {
                    Debug.Log("Too much note -> bad guess");
                    // Bad guess
                    SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS);
                    firstNotes.ForEach(x => x.ShowBadGuess());
                    Guess?.Invoke(this, new GuessEventArgs(false));
                    _noteBuffer.Clear();
                }

                // If buffer == maxNoteCount of the interval mode : check if it's a good or a bad guess
                if(_noteBuffer.Count == maxNoteCount)
                {
                    Debug.Log("Note count match");
                    // Good guess ?

                //}

                //if (GameController.Instance.Controller.NotesDownWithOffset.Count > 0)
                //{
                    Debug.Log(string.Join(", ", firstNotes.Select(x => x.PianoNote.ToString())));
                    Debug.Log(firstSingleNote);

                    // Debug.Log(GameController.Instance.Controller.NotesDownWithOffset[0]);
                    bool? guessValue = null;

                    //if (GameController.Instance.Controller.NotesDownWithOffset.Count == 1)
                    //{
                    var firstPianoNotes = firstNotes.Select(x => x.PianoNote).ToList();
                    var firstPianoNotesForReplace = firstNotes.Select(x => (PianoNote)((int)x.PianoNote % 12)).ToList();

                    bool oneBadNote = false;
                    foreach (var note in _noteBuffer)
                    {
                        bool replaceNote = note.IsReplaceableByDefault || GameController.Instance.GameReplacementMode;
                        var noteFound = firstNotes.FirstOrDefault(x => x.PianoNote == note.Note);

                        if (!oneBadNote)
                            oneBadNote = noteFound == null || (!(!replaceNote && note.Note == firstNotes[0].PianoNote) && !(replaceNote && (int)note.Note % 12 == (int)(firstNotes[0].PianoNote) % 12));
                    }

                    if (!oneBadNote) // if there is one bad note in the buffer -> it's a bad guess
                    {
                        firstNotes.ForEach(x => SoundManager.PlayNote(x.PianoNote));
                        firstNotes.ForEach(x => x.ShowGoodGuess());
                        guessValue = true;
                    }
                    else
                    {
                        SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS);
                        firstNotes.ForEach(x => x.ShowBadGuess());
                        guessValue = false;
                    }

                    //var firstControllerNote = GameController.Instance.Controller.NotesDownWithOffset[0];
                    //bool replaceNote = firstControllerNote.IsReplaceableByDefault || GameController.Instance.GameReplacementMode;

                    //// Normal mode : the note has to be the exact same note
                    //// Replacement mode : the note has to be the same note, no matter the octave (note % 12 == 0)
                    //if (
                    //        (
                    //            (!replaceNote && firstControllerNote.Note == firstNotes[0].PianoNote)
                    //            ||
                    //            (replaceNote && (int)firstControllerNote.Note % 12 == (int)(firstNotes[0].PianoNote) % 12)
                    //        )
                    //    )
                    //{
                    //    SoundManager.PlayNote(firstControllerNote.Note);
                    //    // Good guess
                    //    firstNotes.ForEach(x => x.ShowGoodGuess());
                    //    guessValue = true;
                    //}
                    //else
                    //{
                    //    SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS);
                    //    // Bad guess
                    //    firstNotes.ForEach(x => x.ShowBadGuess());
                    //    guessValue = false;
                    //}
                    // }

                    if (guessValue.HasValue)
                    {
                        Guess?.Invoke(this, new GuessEventArgs(guessValue.Value));
                    }

                    _noteBuffer.Clear();
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