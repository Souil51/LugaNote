using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHelper
{
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

    private static List<PianoNote> _naturalNotes = new List<PianoNote>
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
    /// All naturals notes
    /// </summary>
    public static List<PianoNote> NaturalNotes => _naturalNotes;

    /// <summary>
    /// Convert a sharp note to a natural note or return this same note if already natural
    /// </summary>
    /// <param name="sharp"></param>
    /// <returns>Natural note</returns>
    public static PianoNote ConvertToNaturalNote(PianoNote sharp)
    {
        if (NaturalNotes.Contains(sharp))
        {
            return sharp;
        }
        else
        {
            return sharp - 1;
        }
    }

    /// <summary>
    /// Convert a natural note to a sharp note or return this same note if already sharp
    /// </summary>
    /// <param name="natural"></param>
    /// <returns>Sharp note</returns>
    public static PianoNote ConvertToSharpNote(PianoNote natural)
    {
        if (SharpNotes.Contains(natural) || natural == PianoNote.C8)
        {
            return natural;
        }
        else
        {
            return natural + 1;
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
        var naturalNote = ConvertToNaturalNote(pianoNote);
        var firstNote = GetFirstPianoNoteForClef(clef);

        int idxFirstNote = NaturalNotes.IndexOf(firstNote);
        int idxNaturalNote = NaturalNotes.IndexOf(naturalNote);

        int difference = idxNaturalNote - idxFirstNote;

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
        var naturalNote = ConvertToNaturalNote(pianoNote);
        var firstNote = GetFirstPianoNoteForClef(clef);

        int idxFirstNote = NaturalNotes.IndexOf(firstNote);
        int idxNaturalNote = NaturalNotes.IndexOf(naturalNote);

        int difference = idxNaturalNote - idxFirstNote;

        // Debug.Log(NaturalNote + " - " + firstNote + " - " + idxFirstNote + " - " + idxNaturalNote + " - " + difference);

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

        for (int i = 0; i < 8; i++)
        {
            PianoNote currentC = firstC + (i * 12);
            int currentDifference = Mathf.Abs(currentC - middleNote);

            if (currentDifference < differenceFound)
            {
                differenceFound = currentDifference;
                minNote = currentC;
            }
        }

        return minNote;
    }

    public static string GetNoteCommonName(PianoNote pianoNote, bool flat = false)
    {
        if (!flat)
            return ((NoteCommonNameSharp)((int)pianoNote % 12)).GetEnumDescription();
        else
            return ((NoteCommonNameFlat)((int)pianoNote % 12)).GetEnumDescription();
    }
}