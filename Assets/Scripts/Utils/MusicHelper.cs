using Assets.Scripts.Game.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

/// <summary>
/// All music stuff and calculation
/// </summary>
public class MusicHelper
{
    #region Lists and Hashset
    private static List<PianoNote> _sharpNotes = new List<PianoNote>
    {
        PianoNote.A0Sharp,
        PianoNote.C1Sharp,
        PianoNote.D1Sharp,
        PianoNote.F1Sharp,
        PianoNote.G1Sharp,
        PianoNote.A1Sharp,
        PianoNote.C2Sharp,
        PianoNote.D2Sharp,
        PianoNote.F2Sharp,
        PianoNote.G2Sharp,
        PianoNote.A2Sharp,
        PianoNote.C3Sharp,
        PianoNote.D3Sharp,
        PianoNote.F3Sharp,
        PianoNote.G3Sharp,
        PianoNote.A3Sharp,
        PianoNote.C4Sharp,
        PianoNote.D4Sharp,
        PianoNote.F4Sharp,
        PianoNote.G4Sharp,
        PianoNote.A4Sharp,
        PianoNote.C5Sharp,
        PianoNote.D5Sharp,
        PianoNote.F5Sharp,
        PianoNote.G5Sharp,
        PianoNote.A5Sharp,
        PianoNote.C6Sharp,
        PianoNote.D6Sharp,
        PianoNote.F6Sharp,
        PianoNote.G6Sharp,
        PianoNote.A6Sharp,
        PianoNote.C7Sharp,
        PianoNote.D7Sharp,
        PianoNote.F7Sharp,
        PianoNote.G7Sharp,
        PianoNote.A7Sharp
    };

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

    private static HashSet<int> _notNaturallySharpableIndices = new HashSet<int>()
    {
        2, 7, 14, 17, 26, 31, 38, 43, 50, 55, 62, 67, 74, 79, 86
    };

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

    private static HashSet<int> _notNaturallyFlatableIndices = new HashSet<int>()
    {
        3, 8, 15, 20, 27, 32, 39, 44, 51, 56, 63, 68, 75, 80
    };

    private static List<PianoNote> _notes3rdOctave = new List<PianoNote>()
    {
        PianoNote.E3,
        PianoNote.F3,
        PianoNote.F3Sharp,
        PianoNote.G3,
        PianoNote.G3Sharp,
        PianoNote.A3,
        PianoNote.A3Sharp,
        PianoNote.B3
    };

    private static List<PianoNote> _notes4thOctave = new List<PianoNote>()
    {
        PianoNote.C4,
        PianoNote.C4Sharp,
        PianoNote.D4,
        PianoNote.D4Sharp,
        PianoNote.E4,
        PianoNote.F4,
        PianoNote.F4Sharp,
        PianoNote.G4,
        PianoNote.G4Sharp,
        PianoNote.A4,
        PianoNote.A4Sharp,
        PianoNote.B4,
    };

    private static List<PianoNote> _notes5thOctave = new List<PianoNote>()
    {
        PianoNote.C5,
        PianoNote.C5Sharp,
        PianoNote.D5,
        PianoNote.D5Sharp,
        PianoNote.E5,
        PianoNote.F5,
        PianoNote.F5Sharp,
        PianoNote.G5,
        PianoNote.G5Sharp,
        PianoNote.A5,
        PianoNote.A5Sharp,
        PianoNote.B5,
    };

    private static List<PianoNote> _notes6thOctave = new List<PianoNote>()
    {
        PianoNote.C6,
        PianoNote.C6Sharp,
        PianoNote.D6,
        PianoNote.D6Sharp,
        PianoNote.E6,
        PianoNote.F6,
        PianoNote.F6Sharp,
    };

    private static List<PianoNote> _bassNotes3rdOctave = new List<PianoNote>()
    {
        PianoNote.G1,
        PianoNote.G1Sharp,
        PianoNote.A1,
        PianoNote.A1Sharp,
        PianoNote.B1,
        PianoNote.C2,
        PianoNote.C2Sharp,
        PianoNote.D2,
        PianoNote.D2Sharp
    };

    private static List<PianoNote> _bassNotes4thOctave = new List<PianoNote>()
    {
        PianoNote.E2,
        PianoNote.F2,
        PianoNote.F2Sharp,
        PianoNote.G2,
        PianoNote.G2Sharp,
        PianoNote.A2,
        PianoNote.A2Sharp,
        PianoNote.B2,
        PianoNote.C3,
        PianoNote.C3Sharp,
        PianoNote.D3,
        PianoNote.D3Sharp,
    };

    private static List<PianoNote> _bassNotes5thOctave = new List<PianoNote>()
    {
        PianoNote.E3,
        PianoNote.F3,
        PianoNote.F3Sharp,
        PianoNote.G3,
        PianoNote.G3Sharp,
        PianoNote.A3,
        PianoNote.A3Sharp,
        PianoNote.B3,
        PianoNote.C4,
        PianoNote.C4Sharp,
        PianoNote.D4,
        PianoNote.D4Sharp,
    };

    private static List<PianoNote> _bassNotes6thOctave = new List<PianoNote>()
    {
        PianoNote.E4,
        PianoNote.F4,
        PianoNote.F4Sharp,
        PianoNote.G4,
        PianoNote.G4Sharp,
        PianoNote.A5,
        PianoNote.A5Sharp,
    };

    public static HashSet<int> SharpIndices => new HashSet<int>
    {
        1, 4, 6, 9,
        11, 13, 16, 18,
        21, 23, 25, 28,
        30, 33, 35, 37,
        40, 42, 45, 47, 49,
        52, 54, 57, 59,
        61, 64, 66, 69,
        71, 73, 76, 78,
        81, 83, 85
    };

    public static HashSet<int> NaturalIndices => new HashSet<int>
    {
        2, 3, 5, 7, 8,
        10, 12, 14, 15, 17, 19,
        20, 22, 24, 26, 27, 29,
        31, 32, 34, 36, 38, 39,
        41, 43, 44, 46, 48,
        50, 51, 53, 55, 56, 58,
        60, 62, 63, 65, 67, 68,
        70, 72, 74, 75, 77, 79,
        80, 82, 84, 86, 87
    };


    /***** Chords generation in GetMajor/MinorChords methods *****/
    private static List<PianoChord> MajorChords = new List<PianoChord>() { };

    private static List<PianoChord> MinorChords = new List<PianoChord>() { };

    #endregion

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
        if (key == Clef.Treble)
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
        if (key == Clef.Treble)
            return PianoNote.F6;
        else
            return PianoNote.A4;
    }
    /// <summary>
    /// Return the last A-G notation for each clef
    /// Based on 23 differents notes staff
    /// </summary>
    /// <param name="key">The clef</param>
    /// <returns></returns>

    public static PianoNote GetFirstPianoNoteForClef(Clef key)
    {
        if (key == Clef.Treble)
            return PianoNote.E3;
        else
            return PianoNote.G1;
    }

    /// <summary>
    /// All sharp notes
    /// </summary>
    public static List<PianoNote> SharpNotes => _sharpNotes;

    /// <summary>
    /// All naturals notes
    /// </summary>
    public static List<PianoNote> NaturalNotes => _naturalNotes;

    public static List<PianoNote> NotNaturallySharpableNotes => _notNaturallySharpableNotes;

    public static bool IsNaturallySharpable(PianoNote note)
    {
        return !_notNaturallySharpableIndices.Contains((int)note);
    }

    public static bool IsNaturallyFlatable(PianoNote note)
    {
        return !_notNaturallyFlatableIndices.Contains((int)note);
    }

    public static List<PianoNote> Notes3rdOctave => _notes3rdOctave;
    
    public static List<PianoNote> Notes4thOctave => _notes4thOctave;
    
    public static List<PianoNote> Notes5thOctave => _notes5thOctave;
    
    public static List<PianoNote> Notes6thOctave => _notes6thOctave;
    
    public static List<PianoNote> Notes3rdAnd4thOctave => Notes3rdOctave.Concat(Notes4thOctave).ToList();

    public static List<PianoNote> Notes5thAnd6thOctave => Notes5thOctave.Concat(Notes6thOctave).ToList();

    public static List<PianoNote> Notes3thTo6thOctave => Notes3rdOctave.Concat(Notes4thOctave).Concat(Notes5thOctave).Concat(Notes6thOctave).ToList();

    public static List<PianoNote> BassNotes3rdOctave => _bassNotes3rdOctave;

    public static List<PianoNote> BassNotes4thOctave => _bassNotes4thOctave;

    public static List<PianoNote> BassNotes5thOctave => _bassNotes5thOctave;

    public static List<PianoNote> BassNotes6thOctave => _bassNotes6thOctave;

    public static List<PianoNote> BassNotes3rdAnd4thOctave => BassNotes3rdOctave.Concat(BassNotes4thOctave).ToList();

    public static List<PianoNote> BassNotes5thAnd6thOctave => BassNotes5thOctave.Concat(BassNotes6thOctave).ToList();

    public static List<PianoNote> BassNotes3thTo6thOctave => BassNotes3rdOctave.Concat(BassNotes4thOctave).Concat(BassNotes5thOctave).Concat(BassNotes6thOctave).ToList();

    public static List<PianoNote> GetNotesForLevel(Level level, Clef clef)
    {
        switch (level)
        {
            case Level.C3_C4:
                return clef == Clef.Treble ? Notes3rdAnd4thOctave : BassNotes3rdAnd4thOctave;
            case Level.C5:
                return clef == Clef.Treble ? Notes5thOctave : BassNotes5thOctave;
            case Level.C5_C6:
                return clef == Clef.Treble ? Notes5thAnd6thOctave : BassNotes5thAnd6thOctave;
            case Level.C3_C6:
                return clef == Clef.Treble ? Notes3thTo6thOctave : BassNotes3thTo6thOctave;
            case Level.C4:
            default:
                return clef == Clef.Treble ? Notes4thOctave : BassNotes4thOctave;
        }
    }

    public static int GetNotesCountForInterval(IntervalMode intervalMode)
    {
        switch (intervalMode)
        {
            default:
            case IntervalMode.Note:
                return 1;
            case IntervalMode.Interval:
                return 2;
            case IntervalMode.Chord:
                return 3;
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
        if (IsNatural(sharp))
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
        if (IsSharp(natural) || natural == PianoNote.C8)
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
            return ((SharpPianoNote)((int)pianoNote % 12)).GetEnumDescription();
        else
            return ((FlatPianoNote)((int)pianoNote % 12)).GetEnumDescription();
    }

    public static bool IsNoteNaturralyAlterable(PianoNote note, Accidental accidental)
    {
        bool result = true;

        if (accidental == Accidental.Sharp && !IsNaturallySharpable(note))
            result = false;
        else if (accidental == Accidental.Flat && !IsNaturallyFlatable(note))
            result = false;

        return result;
    }

    /// <summary>
    /// Compare 2 notes with accidental or not
    /// A sharp is equal to a flat of the next note
    /// </summary>
    /// <returns></returns>
    public static bool IsSameNote(PianoNote note1,  PianoNote note2)
    {
        if (note1 == note2)
            return true;

        if (IsSharp(note1))
        {
            
        }

        return false;
    }

    public static List<PianoChord> GetMajorChords()
    {
        if(MajorChords.Count == 0)
        {
            PianoNote[] allEnumValues = (PianoNote[])Enum.GetValues(typeof(PianoNote));
            foreach (var note in allEnumValues)
            {
                Inversion[] inversions = (Inversion[])Enum.GetValues(typeof(Inversion));
                foreach(var inversion in inversions)
                {
                    var chord = new PianoChord(note, inversion, Tonality.Major);
                    if (chord.MinNote >= LowerNote && chord.MaxNote <= HigherNote)
                    {
                        MajorChords.Add(chord);
                    }
                }
            }
        }

        return MajorChords;
    }

    public static List<PianoChord> GetMinorChords()
    {
        if (MinorChords.Count == 0)
        {
            PianoNote[] allEnumValues = (PianoNote[])Enum.GetValues(typeof(PianoNote));
            foreach (var note in allEnumValues)
            {
                Inversion[] inversions = (Inversion[])Enum.GetValues(typeof(Inversion));
                foreach (var inversion in inversions)
                {
                    var chord = new PianoChord(note, inversion, Tonality.Minor);
                    if (chord.MinNote >= LowerNote && chord.MaxNote <= HigherNote)
                    {
                        MinorChords.Add(chord);
                    }
                }
            }
        }

        return MinorChords;
    }

    public static List<PianoChord> GetMajorChords(PianoNote min, PianoNote max, bool withAccidentals = true, bool withInversion = false)
    {
        var chords = GetMajorChords();
        return chords.Where(x => x.MinNote >= min && x.MaxNote <= max && (withAccidentals || !x.WithAccidental) && (withInversion || x.Inversion == Inversion.None)).ToList();
    }

    public static List<PianoChord> GetMinorChords(PianoNote min, PianoNote max, bool withAccidentals = true, bool withInversion = false)
    {
        var chords = GetMinorChords();
        return chords.Where(x => x.MinNote >= min && x.MaxNote <= max && (withAccidentals || !x.WithAccidental) && (withInversion || x.Inversion == Inversion.None)).ToList();
    }

    public static bool IsSharp(PianoNote note)
    {
        return SharpIndices.Contains((int)note);
    }

    public static bool IsNatural(PianoNote note)
    {
        return !IsSharp(note);
    }
}