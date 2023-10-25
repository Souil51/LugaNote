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
    public static string PREFAB_NOTE_BUTTON_SMALL => "NoteButtonSmall";
    public static string PREFAB_TRANSITION => "transition";
    public static string PREFAB_VISUAL_KEYBOARD => "VisualKeyBoard";

    public static string PREFAB_MIDI_CONFIGURATION_PANEL => "ConfigurationPanel";
    public static string PREFAB_DOTTED_LINE => "DottedLine";
    public static string SCENE_MAIN_MENU => "MainMenu";
    public static string SCENE_MAIN_SCENE => "MainScene";

    public static string RESOURCES_SOUND_NOTE_BASE => "sounds/notes/key_";

    public static string RESOURCES_SOUND_BAD_GUESS => "bad_guess";
    public static string RESOURCES_SOUND_CLICK => "click_2_silenced";

    public static string LOCALIZATION_MIDI_CONFIGURATION_TITLE => "MainMenu/title_midi_configuration";
    public static string LOCALIZATION_MIDI_CONFIGURATION_NEW_TITLE => "MainMenu/title_midi_configuration_new";

    public static string LOCALIZATION_MENU_INFO_MIDI_88_TOUCHES => "MainMenu/menu_info_midi_88";
    public static string LOCALIZATION_MENU_INFO_MIDI_61_TOUCHES => "MainMenu/menu_info_midi_61";
    public static string LOCALIZATION_MENU_INFO_MIDI_CUSTOM_TOUCHES => "MainMenu/menu_info_midi_custom";

    public static string LOCALIZATION_MENU_MIDI_88_TOUCHES => "MainMenu/menu_midi_88";
    public static string LOCALIZATION_MENU_MIDI_61_TOUCHES => "MainMenu/menu_midi_61";
    public static string LOCALIZATION_MENU_MIDI_CUSTOM_TOUCHES => "MainMenu/menu_midi_custom";

    public static string LOCALIZATION_CONTROLLER_LABEL_KEYBOARD => "MainMenu/controller_label_keyboard";

    public static string LOCALIZATION_CONTROLLER_LABEL_VISUAL => "MainMenu/controller_label_visual";

    public static string LOCALIZATION_PLAYERPREFS_LOCALIZEKEY => "LocalizeKey";

    /// <summary>
    /// Useful values
    /// </summary>
    public static Color COLOR_GOOD_GUESS => Color.green;
    public static Color COLOR_BAD_GUESS => Color.red;

    public static string COLOR_HEX_DARKGREEN => "23A138";
    public static string COLOR_HEX_GREEN => "38ff59";

    public static string COLOR_HEX_DARKRED => "991A1A";
    public static string COLOR_HEX_RED => "FF3838";

    public static string COLOR_HEX_LIGHT_BLUE => "0984E3";

    public static string COLOR_HEX_ULTRALIGHT_GREEN => "B7FFB4";
    public static string COLOR_HEX_ULTRALIGHT_BLUE => "B9E2FF";
    public static string COLOR_HEX_ULTRALIGHT_RED => "FFEBEB";

    public static string COLOR_HEX_LIME => "91DD97";

    public static string COLOR_HEX_LIGHT_GRAY => "E3E3E3";

    public static int PIANO_KEY_COUNT => 88;

    public static string GET_PREFAB_NOTE(bool withLine, Accidental accidental)
    {
        string result = "";

        if (withLine)
        {
            if (accidental == Accidental.Natural)
                result = StaticResource.PREFAB_NOTE_LINE;
            else if (accidental == Accidental.Sharp)
                result = StaticResource.PREFAB_NOTE_LINE_SHARP;
            else
                result = StaticResource.PREFAB_NOTE_LINE_FLAT;
        }
        else
        {
            if (accidental == Accidental.Natural)
                result = StaticResource.PREFAB_NOTE_NO_LINE;
            else if (accidental == Accidental.Sharp)
                result = StaticResource.PREFAB_NOTE_NO_LINE_SHARP;
            else
                result = StaticResource.PREFAB_NOTE_NO_LINE_FLAT;
        }

        return result;
    }
}