using System;
using UnityEngine;

namespace MidiJack
{
    public class MidiDroidCallback: AndroidJavaProxy
    {
        public delegate void RawMidiDelegate(object sender, MidiMessage m);
        public event RawMidiDelegate DroidMidiEvent;

        public MidiDroidCallback() : base("mmmlabs.com.mididroid.MidiCallback") { }

        // Got this from a french google translate of a chinese blog post (the only google result with the error "no proxy method") and it works ???
        public override AndroidJavaObject Invoke(string methodName, object[] args)
        {
            var deviceIndex = (int)args[0]; // Type = Int32 
            var status = (sbyte)args[1];     // Type = SByte (dépendant du périphérique ?) 
            var data1 = (sbyte)args[2]; // Type = SByte
            var data2 = (sbyte)args[3];      // Type = SByte

            //Debug.Log($"COPO : {methodName}, devID={deviceIndex}, status={status}, d1={data1}, d2={data2}");

            midiJackMessage(deviceIndex, (byte)status, (byte)data1, (byte)data2);
            return null;
        }

        public void midiJackMessage(int deviceIndex, byte status, byte data1, byte data2)
        {
            if(DroidMidiEvent != null)
            {
                DroidMidiEvent(this, new MidiMessage((uint)deviceIndex, status, data1, data2));
            }
        }
    }
}