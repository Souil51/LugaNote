using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    public static string PREFAB_NOTE_LINE_SHARP => "note_line_sharp";
    public static string PREFAB_NOTE_NO_LINE_SHARP => "note_no_line_sharp";
    public static string PREFAB_NOTE_LINE_FLAT => "note_line_flat";
    public static string PREFAB_NOTE_NO_LINE_FLAT => "note_no_line_flat";
    public static string PREFAB_NOTE_LINE_NATURAL => "note_line_natural";
    public static string PREFAB_NOTE_NO_LINE_NATURAL => "note_no_line_natural";
    public static string PREFAB_LINE => "line";
    public static string PREFAB_EMPTY_NOTE_LINE => "empty_note_line";

    public static string PREFAB_NOTE_BUTTON => "NoteButton";
    public static string PREFAB_NOTE_BUTTON_SHARP => "NoteButtonSharp";

    /// <summary>
    /// Useful values
    /// </summary>
    public static Color COLOR_GOOD_GUESS => Color.green;
    public static Color COLOR_BAD_GUESS => Color.red;

    public static int PIANO_KEY_COUNT => 88;
}