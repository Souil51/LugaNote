﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public class Strings
    {
        public static string MENU_INFO_MIDI_88_TOUCHES => "MIDI device changed to : 88 touches";
        public static string MENU_INFO_MIDI_61_TOUCHES => "MIDI device changed to : 61 touches";
        public static string MENU_INFO_MIDI_CUSTOM_TOUCHES => "MIDI device changed to : {0} touches ({1} to {2})";

        public static string MENU_MIDI_88_TOUCHES => "MIDI controller (88 keys)";
        public static string MENU_MIDI_61_TOUCHES => "MIDI controller (61 keys)";
        public static string MENU_MIDI_CUSTOM_TOUCHES => "MIDI controller ({1} to {2})";
    }
}
