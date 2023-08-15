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
                var lowerFirstNote = firstNotes.Select(x => x.PianoNote).Min();
                bool bufferChanged = false;
                // Store notes pressed
                foreach (var note in GameController.Instance.Controller.NotesDownWithOffset)
                {
                    Debug.Log("Adding " + note.Note + " note in buffer");
                    var noteToAdd = note;

                    // For interval "guess name" mode, the notes in NotesDown/UpWithOffset are A0 and (A0 + interval name)
                    // Just add to these notes the (int) of the lower firstnote allow to know if the guess is good
                    if (GameController.Instance.GameMode.GuessName && GameController.Instance.GameMode.IntervalMode == IntervalMode.Interval)
                        noteToAdd = new ControllerNote((PianoNote)((int)note.Note + (int)lowerFirstNote), note.IsReplaceableByDefault);

                    SoundManager.PlayNote(noteToAdd.Note);
                    bool containsNote = _noteBuffer.Select(x => x.Note).Contains(noteToAdd.Note);
                    if (!containsNote)
                    {
                        _noteBuffer.Add(noteToAdd);
                        bufferChanged = true;
                    }
                }

                // Remove note from buffer
                foreach (var note in GameController.Instance.Controller.NotesUpWithOffset)
                {
                    var noteToAdd = note;
                    if (GameController.Instance.GameMode.GuessName && GameController.Instance.GameMode.IntervalMode == IntervalMode.Interval)
                        noteToAdd = new ControllerNote((PianoNote)((int)note.Note + (int)lowerFirstNote), note.IsReplaceableByDefault);

                    if (_noteBuffer.Select(x => x.Note).Contains(noteToAdd.Note))
                    {
                        _noteBuffer.Remove(noteToAdd);
                        bufferChanged = true;
                    }
                }

                // If new note in buffer, check if one note is wrong
                if (_noteBuffer.Count <= maxNoteCount && _noteBuffer.Count > 0 && bufferChanged)
                {
                    Debug.Log("Low : " + firstNotes.Select(x => x.PianoNote).Min() + " - High : " + firstNotes.Select(x => x.PianoNote).Max());
                    // Debug.Log("Buffer if : " + _noteBuffer[0].Note + " - " + _noteBuffer[1].Note);

                    var firstPianoNotes = firstNotes.Select(x => x.PianoNote).ToList();
                    var firstPianoNotesForReplace = firstNotes.Select(x => x.PianoNoteForReplaceValue).ToList();

                    bool? guessValue = null;
                    bool oneBadNote = false;
                    foreach (var note in _noteBuffer)
                    {
                        bool replaceNote = note.IsReplaceableByDefault || GameController.Instance.GameReplacementMode;
                        var noteFound = firstNotes.FirstOrDefault(x => (!replaceNote && x.PianoNote == note.Note) || (replaceNote && x.PianoNoteForReplaceValue == note.PianoNoteForReplaceValue));

                        bool isBadNote = noteFound == null || ((replaceNote || note.Note != noteFound.PianoNote) && (!replaceNote || note.PianoNoteForReplaceValue != noteFound.PianoNoteForReplaceValue));

                        if (!oneBadNote)
                            oneBadNote = isBadNote;

                        if (noteFound != null && !isBadNote && !noteFound.IsPressed)
                        {
                            noteFound.Press(); // Indication that the note in pressed
                        }
                    }

                    if (oneBadNote) // If one bad note -> bad guess
                    {
                        SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS, .4f);
                        firstNotes.ForEach(x => x.ShowBadGuess());
                        guessValue = false;

                    }
                    else if (_noteBuffer.Count == maxNoteCount) // Else if buffer in full -> good guess
                    {
                        firstNotes.Where(x => x.PianoNote != _noteBuffer.Select(x => x.Note).Last()).ToList().ForEach(x => SoundManager.PlayNote(x.PianoNote));
                        firstNotes.ForEach(x => x.ShowGoodGuess());
                        guessValue = true;
                    }

                    if (guessValue.HasValue)
                    {
                        Guess?.Invoke(this, new GuessEventArgs(guessValue.Value));
                        _noteBuffer.Clear();// Clear buffer for next note(s)
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