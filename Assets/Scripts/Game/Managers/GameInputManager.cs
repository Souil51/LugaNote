using Assets.Scripts.Game.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
                bool bufferChanged = false;
                // Store notes pressed
                foreach (var note in GameController.Instance.Controller.NotesDownWithOffset)
                {
                    if (!_noteBuffer.Contains(note))
                    {
                        SoundManager.PlayNote(note.Note);
                        _noteBuffer.Add(note);
                        bufferChanged = true;
                    }
                }

                foreach (var note in GameController.Instance.Controller.NotesUpWithOffset)
                {
                    if (_noteBuffer.Contains(note))
                    {
                        _noteBuffer.Remove(note);
                        bufferChanged = true;
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

                if(_noteBuffer.Count < maxNoteCount && _noteBuffer.Count > 0 && bufferChanged)
                {
                    bool? guessValue = null;

                    var firstPianoNotes = firstNotes.Select(x => x.PianoNote).ToList();
                    var firstPianoNotesForReplace = firstNotes.Select(x => (PianoNote)((int)x.PianoNote % 12)).ToList();

                    bool oneBadNote = false;
                    bool clearBuffer = false;
                    foreach (var note in _noteBuffer)
                    {
                        bool replaceNote = note.IsReplaceableByDefault || GameController.Instance.GameReplacementMode;
                        var noteFound = firstNotes.FirstOrDefault(x => x.PianoNote == note.Note);

                        bool isBadNote = noteFound == null || ((replaceNote || note.Note != noteFound.PianoNote) && (!replaceNote || (int)note.Note % 12 != (int)(noteFound.PianoNote) % 12));

                        if(noteFound != null && !isBadNote && !noteFound.IsPressed)
                        {
                            noteFound.Press();
                        }
                        else
                        {
                            SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS);
                            firstNotes.ForEach(x => x.ShowBadGuess());
                            Guess?.Invoke(this, new GuessEventArgs(false));
                            clearBuffer = true;
                        }
                    }

                    if(clearBuffer)
                        _noteBuffer.Clear();
                }

                // If buffer == maxNoteCount of the interval mode : check if it's a good or a bad guess
                if(_noteBuffer.Count == maxNoteCount)
                {
                    Debug.Log("Note count match");
                    Debug.Log(string.Join(", ", firstNotes.Select(x => x.PianoNote.ToString())));
                    Debug.Log(firstSingleNote);

                    bool? guessValue = null;

                    var firstPianoNotes = firstNotes.Select(x => x.PianoNote).ToList();
                    var firstPianoNotesForReplace = firstNotes.Select(x => (PianoNote)((int)x.PianoNote % 12)).ToList();

                    bool oneBadNote = false;
                    foreach (var note in _noteBuffer)
                    {
                        bool replaceNote = note.IsReplaceableByDefault || GameController.Instance.GameReplacementMode;
                        var noteFound = firstNotes.FirstOrDefault(x => x.PianoNote == note.Note);

                        if (!oneBadNote)
                            oneBadNote = noteFound == null || ((replaceNote || note.Note != noteFound.PianoNote) && (!replaceNote || (int)note.Note % 12 != (int)(noteFound.PianoNote) % 12));
                    }

                    if (!oneBadNote) // if there is one bad note in the buffer -> it's a bad guess
                    {
                        // We play all notes, but not the one that has trigger _buffer check, because the sound of this note is already being played
                        firstNotes.Take(firstNotes.Count - 2).ToList().ForEach(x => SoundManager.PlayNote(x.PianoNote));
                        firstNotes.ForEach(x => x.ShowGoodGuess());
                        guessValue = true;
                    }
                    else
                    {
                        SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS);
                        firstNotes.ForEach(x => x.ShowBadGuess());
                        guessValue = false;
                    }

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