using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
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
    public static string PREFAB_TRANSITION => "transition";
    public static string PREFAB_VISUAL_KEYBOARD => "VisualKeyBoard";

    public static string SCENE_MAIN_MENU => "MainMenu";
    public static string SCENE_MAIN_SCENE => "MainScene";

    /// <summary>
    /// Useful values
    /// </summary>
    public static Color COLOR_GOOD_GUESS => Color.green;
    public static Color COLOR_BAD_GUESS => Color.red;

    public static string COLOR_HEX_DARKGREEN => "3BFF5B";

    public static int PIANO_KEY_COUNT => 88;

    public static string GET_PREFAB_NOTE(bool withLine, Alteration alteration)
    {
        string result = "";

        if (withLine)
        {
            if (alteration == Alteration.Natural)
                result = StaticResource.PREFAB_NOTE_LINE;
            else if (alteration == Alteration.Sharp)
                result = StaticResource.PREFAB_NOTE_LINE_SHARP;
            else
                result = StaticResource.PREFAB_NOTE_LINE_FLAT;
        }
        else
        {
            if (alteration == Alteration.Natural)
                result = StaticResource.PREFAB_NOTE_NO_LINE;
            else if (alteration == Alteration.Sharp)
                result = StaticResource.PREFAB_NOTE_NO_LINE_SHARP;
            else
                result = StaticResource.PREFAB_NOTE_NO_LINE_FLAT;
        }

        return result;
    }
}