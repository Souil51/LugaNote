using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// All music stuff and calculation
/// </summary>
public class MusicHelper
{
    public static PianoNote HigherNote => PianoNote.C8;
    public static PianoNote LowerNote => PianoNote.A0;

    public static PianoNote HigherNote_66Touches => PianoNote.C7;
    public static PianoNote LowerNote_66Touches => PianoNote.C2;

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

    // These not can be Sharp but will not be often Sharp (B to C or E to F)
    private static List<PianoNote> _notNaturallySharpableNotes = new List<PianoNote>() 
    { 
        PianoNote.E1,
        PianoNote.E2,
        PianoNote.E3,
        PianoNote.E4,
        PianoNote.E5,
        PianoNote.E6,
        PianoNote.E7,
        PianoNote.B0,
        PianoNote.B1,
        PianoNote.B2,
        PianoNote.B3,
        PianoNote.B4,
        PianoNote.B5,
        PianoNote.B6,
        PianoNote.B7,
    };

    public static List<PianoNote> NotNaturallySharpableNotes => _notNaturallySharpableNotes;

    // These not can be Sharp but will not be often Sharp (C to B or F to E)
    private static List<PianoNote> _notNaturallyFlatableNotes = new List<PianoNote>()
    {
        PianoNote.F1,
        PianoNote.F2,
        PianoNote.F3,
        PianoNote.F4,
        PianoNote.F5,
        PianoNote.F6,
        PianoNote.F7,
        PianoNote.C1,
        PianoNote.C2,
        PianoNote.C3,
        PianoNote.C4,
        PianoNote.C5,
        PianoNote.C6,
        PianoNote.C7,
    };

    public static List<PianoNote> Notes3rdOctave => _notes3rdOctave;
    private static List<PianoNote> _notes3rdOctave = new List<PianoNote>()
    {
        PianoNote.E3,
        PianoNote.F3,
        PianoNote.FSharp3,
        PianoNote.G3,
        PianoNote.GSharp3,
        PianoNote.A3,
        PianoNote.ASharp3,
        PianoNote.B3
    };

    public static List<PianoNote> Notes4thOctave => _notes4thOctave;
    private static List<PianoNote> _notes4thOctave = new List<PianoNote>()
    {
        PianoNote.C4,
        PianoNote.CSharp4,
        PianoNote.D4,
        PianoNote.DSharp4,
        PianoNote.E4,
        PianoNote.F4,
        PianoNote.FSharp4,
        PianoNote.G4,
        PianoNote.GSharp4,
        PianoNote.A4,
        PianoNote.ASharp4,
        PianoNote.B4,
    };

    public static List<PianoNote> Notes5thOctave => _notes5thOctave;
    private static List<PianoNote> _notes5thOctave = new List<PianoNote>()
    {
        PianoNote.C5,
        PianoNote.CSharp5,
        PianoNote.D5,
        PianoNote.DSharp5,
        PianoNote.E5,
        PianoNote.F5,
        PianoNote.FSharp5,
        PianoNote.G5,
        PianoNote.GSharp5,
        PianoNote.A5,
        PianoNote.ASharp5,
        PianoNote.B5,
    };

    public static List<PianoNote> Notes6thOctave => _notes6thOctave;
    private static List<PianoNote> _notes6thOctave = new List<PianoNote>()
    {
        PianoNote.C6,
        PianoNote.CSharp6,
        PianoNote.D6,
        PianoNote.DSharp6,
        PianoNote.E6,
        PianoNote.F6,
        PianoNote.FSharp6,
    };

    public static List<PianoNote> Notes3rdAnd4thOctave => Notes3rdOctave.Concat(Notes4thOctave).ToList();

    public static List<PianoNote> Notes5thAnd6thOctave => Notes5thOctave.Concat(Notes6thOctave).ToList();

    public static List<PianoNote> Notes3thTo6thOctave => Notes3rdOctave.Concat(Notes4thOctave).Concat(Notes5thOctave).Concat(Notes6thOctave).ToList();

    public static List<PianoNote> GetNotesForLevel(Level level)
    {
        switch (level)
        {
            case Level.C3_C4:
                return Notes3rdAnd4thOctave;
            case Level.C5:
                return Notes5thOctave;
            case Level.C5_C6:
                return Notes5thAnd6thOctave;
            case Level.C3_C6:
                return Notes3thTo6thOctave;
            case Level.C4:
            default:
                return Notes4thOctave;
        }
    }

    public static List<PianoNote> NotNaturallyFlatableNotes => _notNaturallyFlatableNotes;

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

    public static bool IsNoteNaturralyAlterable(PianoNote note, Alteration alteration)
    {
        bool result = true;

        if (alteration == Alteration.Sharp && NotNaturallySharpableNotes.Contains(note))
            result = false;
        else if (alteration == Alteration.Flat && NotNaturallyFlatableNotes.Contains(note))
            result = false;

        return result;
    }

    /// <summary>
    /// Compare 2 notes with alteration or not
    /// A sharp is equal to a flat of the next note
    /// </summary>
    /// <returns></returns>
    public static bool IsSameNote(PianoNote note1,  PianoNote note2)
    {
        if (note1 == note2)
            return true;

        if (MusicHelper.SharpNotes.Contains(note1))
        {
            
        }

        return false;
    }
}