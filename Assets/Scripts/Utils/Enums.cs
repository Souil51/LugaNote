using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum TransitionPosition { Open_1 = 0, Open_2 = 1, Close = 2 };
public enum ControllerType { Keyboard = 0, MIDI = 1, Visual = 2, KeyboardAndVisual = 3, KeyboardAndMidi = 4, KeyboardVisualMidi = 5 }
public enum ScaleFactor { Screen = 0, Staff = 1 }
public enum Clef { Trebble = 0, Bass = 1 }
public enum GameModeType { Trebble = 0, Bass = 1, TrebbleBass = 2 }
public enum IntervalMode { Note = 0, Interval = 1, Chord = 2 }

public enum GameState { Loaded = 5, Starting = 0, Started = 1, Ended = 2, Paused = 3, Navigating = 4 }
public enum MenuState { Loaded = 0, Idle = 1, Configuration = 2, ViewScore = 3 }

public enum Level { C4 = 0, C5, C3_C4, C5_C6, C3_C6}

public enum MidiConfigurationType { Touches88 = 0, Touches61 = 1, Custom = 3 }

public enum SceneSessionKey 
{
    [Description("GameMode")]
    GameMode = 0,
    [Description("ReplacementMode")]
    ReplacementMode
}

/// <summary>
/// All the notes, natural and sharp
/// </summary>
public enum PianoNote
{
    A0 = 0,
    A0Sharp = 1,
    B0 = 2,
    C1 = 3,
    C1Sharp = 4,
    D1 = 5,
    D1Sharp = 6,
    E1 = 7,
    F1 = 8,
    F1Sharp = 9,
    G1 = 10,
    G1Sharp = 11,
    A1 = 12,
    A1Sharp = 13,
    B1 = 14,
    C2 = 15,
    C2Sharp = 16,
    D2 = 17,
    D2Sharp = 18,
    E2 = 19,
    F2 = 20,
    F2Sharp = 21,
    G2 = 22,
    G2Sharp = 23,
    A2 = 24,
    A2Sharp = 25,
    B2 = 26,
    C3 = 27,
    C3Sharp = 28,
    D3 = 29,
    D3Sharp = 30,
    E3 = 31,
    F3 = 32,
    F3Sharp = 33,
    G3 = 34,
    G3Sharp = 35,
    A3 = 36,
    A3Sharp = 37,
    B3 = 38,
    C4 = 39,
    C4Sharp = 40,
    D4 = 41,
    D4Sharp = 42,
    E4 = 43,
    F4 = 44,
    F4Sharp = 45,
    G4 = 46,
    G4Sharp = 47,
    A4 = 48,
    A4Sharp = 49,
    B4 = 50,
    C5 = 51,
    C5Sharp = 52,
    D5 = 53,
    D5Sharp = 54,
    E5 = 55,
    F5 = 56,
    F5Sharp = 57,
    G5 = 58,
    G5Sharp = 59,
    A5 = 60,
    A5Sharp = 61,
    B5 = 62,
    C6 = 63,
    C6Sharp = 64,
    D6 = 65,
    D6Sharp = 66,
    E6 = 67,
    F6 = 68,
    F6Sharp = 69,
    G6 = 70,
    G6Sharp = 71,
    A6 = 72,
    A6Sharp = 73,
    B6 = 74,
    C7 = 75,
    C7Sharp = 76,
    D7 = 77,
    D7Sharp = 78,
    E7 = 79,
    F7 = 80,
    F7Sharp = 81,
    G7 = 82,
    G7Sharp = 83,
    A7 = 84,
    A7Sharp = 85,
    B7 = 86,
    C8 = 87
}

public enum NoteCommonNameSharp
{
    [Description("La")]
    La = 0,
    [Description("La#")]
    LaSharp,
    [Description("Si")]
    Si,
    [Description("Do")]
    Do,
    [Description("Do#")]
    DoSharp,
    [Description("RÈ")]
    RÈ,
    [Description("RÈ#")]
    RÈSharp,
    [Description("Mi")]
    Mi,
    [Description("Fa")]
    Fa,
    [Description("Fa#")]
    FaSharp,
    [Description("Sol")]
    Sol,
    [Description("Sol#")]
    SolSharp
}

public enum NoteCommonNameFlat
{
    [Description("La")]
    La = 0,
    [Description("Sib")]
    LaFlat,
    [Description("Si")]
    Si,
    [Description("Do")]
    Do,
    [Description("RÈb")]
    DoFlat,
    [Description("RÈ")]
    RÈ,
    [Description("Mib")]
    RÈFlat,
    [Description("Mi")]
    Mi,
    [Description("Fa")]
    Fa,
    [Description("Solb")]
    FaFlat,
    [Description("Sol")]
    Sol,
    [Description("Lab")]
    SolFlat
}

public enum PianoScale
{
    A,
    Am,
    B,
    Bm,
    C,
    Cm,
    D,
    Dm,
    E,
    Em,
    F,
    Fm
}

public enum Alteration
{
    Natural = 0,
    Sharp = 1,
    Flat = 2
}

public static class Enums
{
    public static string GetEnumDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }

        return value.ToString();
    }
}
