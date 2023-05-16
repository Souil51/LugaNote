using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    PianoNote HigherNote { get; }
    PianoNote LowerNote { get; }

    List<PianoNote> NotesDown { get; }
    List<PianoNote> NotesUp { get; }
    List<PianoNote> Notes { get; }
}
