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

    public event EventHandler Pause;

    // Used to store pending notes when playing intervals/chords mode
    private readonly List<ControllerNote> _noteBuffer = new();
    private int maxNoteCount = 1;

    // When playing multiple note on a MIDI device, we won't count bad guess for notes played at the same time or note played at few ms interval
    private float bounceTimeLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.Controller.ResetInputs();
        maxNoteCount = (int)GameController.Instance.GameMode.IntervalMode + 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(bounceTimeLeft);
        if (bounceTimeLeft > 0)
        {
            _noteBuffer.Clear();
            bounceTimeLeft -= Time.unscaledDeltaTime;
            if(bounceTimeLeft < 0)
            {
                bounceTimeLeft = 0;
            }
        }

        //Debug.Log("Update GameInputManager " + Time.frameCount);

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
                if (bounceTimeLeft > 0) // We just make a bad guess, we won't handle note for a short time
                    return;

                var lowerFirstNote = firstNotes.Select(x => x.PianoNote).Min();
                bool bufferChanged = false;
                // Store notes pressed
                foreach (var note in GameController.Instance.Controller.NotesDownWithOffset)
                {
                    // Debug.Log("note down");
                    //Debug.Log("Adding " + note.Note + " note in buffer");
                    var noteToAdd = note;

                    // For interval "guess name" mode, the notes in NotesDown/UpWithOffset are A0 and (A0 + interval name)
                    // Just add to these notes the (int) of the lower firstnote allow to know if the guess is good
                    if (GameController.Instance.GameMode.GuessName && GameController.Instance.GameMode.IntervalMode == IntervalMode.Interval)
                        noteToAdd = new ControllerNote((PianoNote)((int)note.Note + (int)lowerFirstNote), note.IsReplaceableByDefault, note.ControllerType);

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
                        noteToAdd = new ControllerNote((PianoNote)((int)note.Note + (int)lowerFirstNote), note.IsReplaceableByDefault, note.ControllerType);

                    if (_noteBuffer.Select(x => x.Note).Contains(noteToAdd.Note))
                    {
                        _noteBuffer.Remove(noteToAdd);
                        bufferChanged = true;
                    }
                }

                // If new note in buffer, check if one note is wrong
                if (_noteBuffer.Count <= maxNoteCount && _noteBuffer.Count > 0 && bufferChanged)
                {
                    //Debug.Log("Low : " + firstNotes.Select(x => x.PianoNote).Min() + " - High : " + firstNotes.Select(x => x.PianoNote).Max());
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

                    // Security to handle weird controller behaviour if clicking fast on multiple button in guessName mode
                    if(GameController.Instance.GameMode.GuessName && _noteBuffer.Count != maxNoteCount)
                    {
                        oneBadNote = true;
                    }

                    if (oneBadNote) // If one bad note -> bad guess
                    {
                        SoundManager.PlaySound(StaticResource.RESOURCES_SOUND_BAD_GUESS, .4f);
                        firstNotes.ForEach(x => x.ShowBadGuess());
                        guessValue = false;

                        bounceTimeLeft = 0.25f;

                    }
                    else if (_noteBuffer.Count == maxNoteCount) // Else if buffer in full -> good guess
                    {
                        // PlaySound for all note that are not the last note played (buffered)
                        // To get the interval/chord sound when it's a good guess
                        var notesToPlay = new List<Note>();

                        foreach(var note in firstNotes)
                        {
                            var lastBufferedNote = _noteBuffer.Last();
                            bool replaceNote = lastBufferedNote.IsReplaceableByDefault || GameController.Instance.GameReplacementMode;

                            if ((!replaceNote && note.PianoNote != lastBufferedNote.Note)
                                || (replaceNote && note.PianoNoteForReplaceValue != lastBufferedNote.PianoNoteForReplaceValue))
                            {
                                notesToPlay.Add(note);
                            }
                        }

                        notesToPlay.ForEach(x => SoundManager.PlayNote(x.PianoNote));

                        // Good guess
                        firstNotes.ForEach(x => x.ShowGoodGuess());
                        guessValue = true;
                    }

                    if (guessValue.HasValue)
                    {
                        var controllersList = _noteBuffer.Select(x => x.ControllerType).ToList();

                        Guess?.Invoke(this, new GuessEventArgs(guessValue.Value, controllersList));
                        _noteBuffer.Clear();// Clear buffer for next note(s)
                    }
                } 
                else if(_noteBuffer.Count > maxNoteCount)
                {
                    _noteBuffer.Clear();
                }

                // For testing
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Guess?.Invoke(this, new GuessEventArgs(true, new List<ControllerType>() { ControllerType.Keyboard }));
                }

                GameController.Instance.Controller.ResetInputs();
            }
        }
    }
}

public class GuessEventArgs : EventArgs
{
    public bool Result;

    private readonly List<ControllerType> _usedControllers;
    public List<ControllerType> UsedControllers => _usedControllers;

    public GuessEventArgs(bool result, List<ControllerType> usedControllers)
    {
        Result = result;
        _usedControllers = usedControllers;
    }
}