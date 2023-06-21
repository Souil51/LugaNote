using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum TransitionPosition { Open_1 = 0, Open_2 = 1, Close = 2 };
public enum ControllerType { Keyboard = 0, MIDI = 1, Visual = 2 }
public enum ScaleFactor { Screen = 0, Staff = 1 }
public enum Clef { Trebble = 0, Bass = 1 }
public enum GameMode { Trebble = 0, Bass = 1, TrebbleBass = 2 }

public enum GameState { Loaded = 5, Starting = 0, Started = 1, Ended = 2, Paused = 3, Navigating = 4 }

public enum SceneSessionKey 
{
    [Description("GameMode")]
    GameMode = 0
}
/// <summary>
/// All the notes, natural and sharp
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
