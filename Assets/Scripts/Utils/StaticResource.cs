using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main utility static class
/// This class contains many helping and static datas like note enum, first note displayed for every clef...
/// </summary>
public static class StaticResource
{
    /// <summary>
    /// Below, every string for prefabs, used to not use magics strings for instantiating prefabs
    /// </summary>
    public static string PREFAB_NOTE_LINE => "note_line";
    public static string PREFAB_NOTE_NO_LINE => "note_no_line";
    public static string PREFAB_LINE => "line";
    public static string PREFAB_EMPTY_NOTE_LINE => "empty_note_line";

    /// <summary>
    /// Useful values
    /// </summary>
    public static Color COLOR_GOOD_GUESS => Color.green;
    public static Color COLOR_BAD_GUESS => Color.red;

    public static int PIANO_KEY_COUNT => 88;

    public static PianoNote HigherNote => PianoNote.C8;
    public static PianoNote LowerNote => PianoNote.A0;

    /// <summary>
    /// Return the A-G notation for each Clef
    /// </summary>
    /// <param name="key">The clef</param>
    /// <returns></returns>
    public static PianoNote GetMainPianoNoteForClef(Clef key)
    {
        if (key == Clef.Trebble)
            return PianoNote.G4;
        else
            return PianoNote.F3;
    }

    /// <summary>
    /// Return the first A-G notation for each clef
    /// Based on 23 differents notes staff
    /// </summary>
    /// <param name="key">The clef</param>
    /// <returns></returns>
    public static PianoNote GetLastPianoNoteForClef(Clef key)
    {
        if (key == Clef.Trebble)
            return PianoNote.F6;
        else
            return PianoNote.A5;
    }
    /// <summary>
    /// Return the last A-G notation for each clef
    /// Based on 23 differents notes staff
    /// </summary>
    /// <param name="key">The clef</param>
    /// <returns></returns>

    public static PianoNote GetFirstPianoNoteForClef(Clef key)
    {
        if (key == Clef.Trebble)
            return PianoNote.E3;
        else
            return PianoNote.G1;
    }


    private static List<PianoNote> _sharpNotes = new List<PianoNote>
    {
        PianoNote.ASharp0,
        PianoNote.CSharp1,
        PianoNote.DSharp1,
        PianoNote.FSharp1,
        PianoNote.GSharp1,
        PianoNote.ASharp1,
        PianoNote.CSharp2,
        PianoNote.DSharp2,
        PianoNote.FSharp2,
        PianoNote.GSharp2,
        PianoNote.ASharp2,
        PianoNote.CSharp3,
        PianoNote.DSharp3,
        PianoNote.FSharp3,
        PianoNote.GSharp3,
        PianoNote.ASharp3,
        PianoNote.CSharp4,
        PianoNote.DSharp4,
        PianoNote.FSharp4,
        PianoNote.GSharp4,
        PianoNote.ASharp4,
        PianoNote.CSharp5,
        PianoNote.DSharp5,
        PianoNote.FSharp5,
        PianoNote.GSharp5,
        PianoNote.ASharp5,
        PianoNote.CSharp6,
        PianoNote.DSharp6,
        PianoNote.FSharp6,
        PianoNote.GSharp6,
        PianoNote.ASharp6,
        PianoNote.CSharp7,
        PianoNote.DSharp7,
        PianoNote.FSharp7,
        PianoNote.GSharp7,
        PianoNote.ASharp7
    };
    /// <summary>
    /// All sharp notes
    /// </summary>
    public static List<PianoNote> SharpNotes => _sharpNotes;

    private static List<PianoNote> _flatNotes = new List<PianoNote>
    {
        PianoNote.A0,
        PianoNote.B0,
        PianoNote.C1,
        PianoNote.D1,
        PianoNote.E1,
        PianoNote.F1,
        PianoNote.G1,
        PianoNote.A1,
        PianoNote.B1,
        PianoNote.C2,
        PianoNote.D2,
        PianoNote.E2,
        PianoNote.F2,
        PianoNote.G2,
        PianoNote.A2,
        PianoNote.B2,
        PianoNote.C3,
        PianoNote.D3,
        PianoNote.E3,
        PianoNote.F3,
        PianoNote.G3,
        PianoNote.A3,
        PianoNote.B3,
        PianoNote.C4,
        PianoNote.D4,
        PianoNote.E4,
        PianoNote.F4,
        PianoNote.G4,
        PianoNote.A4,
        PianoNote.B4,
        PianoNote.C5,
        PianoNote.D5,
        PianoNote.E5,
        PianoNote.F5,
        PianoNote.G5,
        PianoNote.A5,
        PianoNote.B5,
        PianoNote.C6,
        PianoNote.D6,
        PianoNote.E6,
        PianoNote.F6,
        PianoNote.G6,
        PianoNote.A6,
        PianoNote.B6,
        PianoNote.C7,
        PianoNote.D7,
        PianoNote.E7,
        PianoNote.F7,
        PianoNote.G7,
        PianoNote.A7,
        PianoNote.B7,
        PianoNote.C8
    };
    /// <summary>
    /// All flat notes
    /// </summary>
    public static List<PianoNote> FlatNotes => _flatNotes;

    /// <summary>
    /// Convert a sharp note to a flat note or return this same note if already flat
    /// </summary>
    /// <param name="sharp"></param>
    /// <returns>Flat note</returns>
    public static PianoNote ConvertToFlatNote(PianoNote sharp)
    {
        if (FlatNotes.Contains(sharp))
        {
            return sharp;
        }
        else
        {
            return sharp - 1;
        }
    }

    /// <summary>
    /// Convert a flat note to a sharp note or return this same note if already sharp
    /// </summary>
    /// <param name="flat"></param>
    /// <returns>Sharp note</returns>
    public static PianoNote ConvertToSharpNote(PianoNote flat)
    {
        if (SharpNotes.Contains(flat) || flat == PianoNote.C8)
        {
            return flat;
        }
        else
        {
            return flat + 1;
        }
    }

    /// <summary>
    /// For notes that are not on a displayed line, return the number of empty line to draw below a note
    /// </summary>
    /// <param name="clef"></param>
    /// <param name="pianoNote"></param>
    /// <returns></returns>
    public static int GetAdditionnalEmptyLineBelow(Clef clef, PianoNote pianoNote) 
    {
        var flatNote = ConvertToFlatNote(pianoNote);
        var firstNote = GetFirstPianoNoteForClef(clef);

        int idxFirstNote = FlatNotes.IndexOf(firstNote);
        int idxFlatNote = FlatNotes.IndexOf(flatNote);

        int difference = idxFlatNote - idxFirstNote;

        if (difference > 21)
            return 3;
        else if (difference > 19)
            return 2;
        else if (difference > 17)
            return 1;

        return 0;
    }

    /// <summary>
    /// For notes that are not on a displayed line, return the number of empty line to draw above a note
    /// </summary>
    /// <param name="clef"></param>
    /// <param name="pianoNote"></param>
    /// <returns></returns>
    public static int GetAdditionnalEmptyLineAbove(Clef clef, PianoNote pianoNote)
    {
        var flatNote = ConvertToFlatNote(pianoNote);
        var firstNote = GetFirstPianoNoteForClef(clef);

        int idxFirstNote = FlatNotes.IndexOf(firstNote);
        int idxFlatNote = FlatNotes.IndexOf(flatNote);

        int difference = idxFlatNote - idxFirstNote;

        if (difference < 1)
            return 3;
        else if (difference < 3)
            return 2;
        else if (difference < 5)
            return 1;

        return 0;
    }

    /// <summary>
    /// Return the C note in between 2 others notes
    /// The octave of the C have to be complete
    /// </summary>
    /// <param name="noteMin"></param>
    /// <param name="noteMax"></param>
    /// <returns>Closest C note in the middle of noteMin and noteMax</returns>
    public static PianoNote GetMiddleCBetweenTwoNotes(PianoNote higherNote, PianoNote lowerNote)
    {
        int middleOffset = (higherNote - lowerNote) / 2;
        PianoNote middleNote = lowerNote + middleOffset;

        PianoNote firstC = PianoNote.C1;
        PianoNote minNote = PianoNote.C8;
        int differenceFound = PianoNote.C8 - PianoNote.A0;

        for(int i = 0; i < 8; i++)
        {
            PianoNote currentC = firstC + (i * 12);
            int currentDifference = Mathf.Abs(currentC - middleNote);

            if(currentDifference < differenceFound)
            {
                differenceFound = currentDifference;
                minNote = currentC;
            }
        }

        return minNote;
    }
}

public enum ControllerType { Keyboard = 0, MIDI = 1 }

public enum Clef { Trebble = 0, Bass = 1}

/// <summary>
/// All the notes, flat and sharp
/// </summary>
public enum PianoNote
{
    A0 = 0,
    ASharp0 = 1,
    B0 = 2,
    C1 = 3,
    CSharp1 = 4,
    D1 = 5,
    DSharp1 = 6,
    E1 = 7,
    F1 = 8,
    FSharp1 = 9,
    G1 = 10,
    GSharp1 = 11,
    A1 = 12,
    ASharp1 = 13,
    B1 = 14,
    C2 = 15,
    CSharp2 = 16,
    D2 = 17,
    DSharp2 = 18,
    E2 = 19,
    F2 = 20,
    FSharp2 = 21,
    G2 = 22,
    GSharp2 = 23,
    A2 = 24,
    ASharp2 = 25,
    B2 = 26,
    C3 = 27,
    CSharp3 = 28,
    D3 = 29,
    DSharp3 = 30,
    E3 = 31,
    F3 = 32,
    FSharp3 = 33,
    G3 = 34,
    GSharp3 = 35,
    A3 = 36,
    ASharp3 = 37,
    B3 = 38,
    C4 = 39,
    CSharp4 = 40,
    D4 = 41,
    DSharp4 = 42,
    E4 = 43,
    F4 = 44,
    FSharp4 = 45,
    G4 = 46,
    GSharp4 = 47,
    A4 = 48,
    ASharp4 = 49,
    B4 = 50,
    C5 = 51,
    CSharp5 = 52,
    D5 = 53,
    DSharp5 = 54,
    E5 = 55,
    F5 = 56,
    FSharp5 = 57,
    G5 = 58,
    GSharp5 = 59,
    A5 = 60,
    ASharp5 = 61,
    B5 = 62,
    C6 = 63,
    CSharp6 = 64,
    D6 = 65,
    DSharp6 = 66,
    E6 = 67,
    F6 = 68,
    FSharp6 = 69,
    G6 = 70,
    GSharp6 = 71,
    A6 = 72,
    ASharp6 = 73,
    B6 = 74,
    C7 = 75,
    CSharp7 = 76,
    D7 = 77,
    DSharp7 = 78,
    E7 = 79,
    F7 = 80,
    FSharp7 = 81,
    G7 = 82,
    GSharp7 = 83,
    A7 = 84,
    ASharp7 = 85,
    B7 = 86,
    C8 = 87
}