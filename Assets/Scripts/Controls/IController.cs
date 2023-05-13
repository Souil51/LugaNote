using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    List<PianoNote> NotesDown { get; }
    List<PianoNote> NotesUp { get; }
    List<PianoNote> Notes { get; }
}
