//
// MidiJack - MIDI Input Plugin for Unity
//
// Copyright (C) 2013-2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using Unity.VisualScripting;
using System.Reflection;
using Assets.Scripts.Game.Model;
using Newtonsoft.Json;
using System.IO;
using Assets.MidiJack;

namespace MidiJack
{
    public abstract class MidiDriver
    {
        #region Internal Data

        protected class ChannelState
        {
            // Note state array
            // X<0    : Released on this frame
            // X=0    : Off
            // 0<X<=1 : On (X represents velocity)
            // 1<X<=2 : Triggered on this frame
            //          (X-1 represents velocity)
            public float[] _noteArray;

            // Knob number to knob value mapping
            public Dictionary<int, float> _knobMap;

            public ChannelState()
            {
                _noteArray = new float[128];
                _knobMap = new Dictionary<int, float>();
            }
        }

        // Channel state array
        protected ChannelState[] _channelArray;

        // Last update frame number
        protected int _lastFrame;

        protected List<string> allDevices = new List<string>();
        protected List<BoundDevice> allDevicesBound = new List<BoundDevice>();

        protected static string _persistentPermissionFilePath = Application.persistentDataPath + "/persistentPermissions.json";

        #endregion

        #region Accessor Methods

        public float GetKey(MidiChannel channel, int noteNumber)
        {
            UpdateIfNeeded();
            var v = _channelArray[(int)channel]._noteArray[noteNumber];
            if (v > 1) return v - 1;
            if (v > 0) return v;
            return 0.0f;
        }

        public bool GetKeyDown(MidiChannel channel, int noteNumber)
        {
            UpdateIfNeeded();
            return _channelArray[(int)channel]._noteArray[noteNumber] > 1;
        }

        public bool GetKeyUp(MidiChannel channel, int noteNumber)
        {
            UpdateIfNeeded();
            return _channelArray[(int)channel]._noteArray[noteNumber] < 0;
        }

        public int[] GetKnobNumbers(MidiChannel channel)
        {
            UpdateIfNeeded();
            var cs = _channelArray[(int)channel];
            var numbers = new int[cs._knobMap.Count];
            cs._knobMap.Keys.CopyTo(numbers, 0);
            return numbers;
        }

        public float GetKnob(MidiChannel channel, int knobNumber, float defaultValue)
        {
            UpdateIfNeeded();
            var cs = _channelArray[(int)channel];
            if (cs._knobMap.ContainsKey(knobNumber)) return cs._knobMap[knobNumber];
            return defaultValue;
        }

        #endregion

        #region Event Delegates

        public delegate void NoteOnDelegate(MidiChannel channel, int note, float velocity);
        public delegate void NoteOffDelegate(MidiChannel channel, int note);
        public delegate void KnobDelegate(MidiChannel channel, int knobNumber, float knobValue);
        public delegate void DeviceConnectedDelegate(string deviceName);
        public delegate void DeviceDisconnectedDelegate(string deviceName);

        public NoteOnDelegate noteOnDelegate { get; set; }
        public NoteOffDelegate noteOffDelegate { get; set; }
        public KnobDelegate knobDelegate { get; set; }
        public DeviceConnectedDelegate deviceConnectedDelegate { get; set; }
        public DeviceDisconnectedDelegate deviceDisconnectedDelegate { get; set; }

        #endregion

        #region Editor Support

#if UNITY_EDITOR

        // Update timer
        protected const float _updateInterval = 1.0f / 30;
        protected float _lastUpdateTime;

        protected bool CheckUpdateInterval()
        {
            var current = Time.realtimeSinceStartup;
            if (current - _lastUpdateTime > _updateInterval || current < _lastUpdateTime) {
                _lastUpdateTime = current;
                return true;
            }
            return false;
        }

        // Total message count
        protected int _totalMessageCount;

        public int TotalMessageCount {
            get {
                UpdateIfNeeded();
                return _totalMessageCount;
            }
        }

        // Message history
        protected Queue<MidiMessage> _messageHistory;

        public Queue<MidiMessage> History {
            get { return _messageHistory; }
        }

#endif

        #endregion

        #region Public Methods

        protected MidiDriver()
        {
            _channelArray = new ChannelState[17];
            for (var i = 0; i < 17; i++)
                _channelArray[i] = new ChannelState();

            #if UNITY_EDITOR
            _messageHistory = new Queue<MidiMessage>();
            #endif
        }

        #endregion

        #region Private Methods

        void UpdateIfNeeded()
        {
            if (Application.isPlaying)
            {
                var frame = Time.frameCount;
                if (frame != _lastFrame) {
                    Update();
                    _lastFrame = frame;
                }
            }
            else
            {
                #if UNITY_EDITOR
                if (CheckUpdateInterval()) Update();
                #endif
            }
        }

        protected virtual void Update()
        {
            // Update the note state array.
            foreach (var cs in _channelArray)
            {
                for (var i = 0; i < 128; i++)
                {
                    var x = cs._noteArray[i];
                    if (x > 1)
                        cs._noteArray[i] = x - 1; // Key down -> Hold.
                    else if (x < 0)
                        cs._noteArray[i] = 0; // Key up -> Off.
                }
            }
        }

        protected virtual void DeviceCallback() { }

        protected virtual void Init() { }

        #endregion

        #region Platform specific

        #endregion

        protected static void AddAndSavePersistentPermissions(string deviceName)
        {
            string jsonString = "";
            if (!File.Exists(_persistentPermissionFilePath))
            {
                File.Create(_persistentPermissionFilePath);
                jsonString = JsonConvert.SerializeObject(new List<string>());

                using (StreamWriter sw = new StreamWriter(_persistentPermissionFilePath))
                {
                    sw.Write(jsonString);
                }
            }

            var persistentList = new List<string>();
            using (StreamReader sr = new StreamReader(_persistentPermissionFilePath))
            {
                string szJson = sr.ReadToEnd();
                if (!String.IsNullOrEmpty(szJson))
                    persistentList = JsonConvert.DeserializeObject<List<string>>(szJson);
            }

            persistentList.Add(deviceName);
            jsonString = JsonConvert.SerializeObject(persistentList);

            using (StreamWriter sw = new StreamWriter(_persistentPermissionFilePath))
            {
                sw.Write(jsonString);
            }
        }

        protected static List<string> GetPersistentPermissions()
        {
            List<string> persistentList = new List<string>();
            if (File.Exists(_persistentPermissionFilePath))
            {
                using (StreamReader sr = new StreamReader(_persistentPermissionFilePath))
                {
                    string szJson = sr.ReadToEnd();
                    if(!String.IsNullOrEmpty(szJson))
                        persistentList = JsonConvert.DeserializeObject<List<string>>(szJson);
                }
            }

            return persistentList;
        }

        
        #region Singleton Class Instance

        protected static MidiDriver _instance;

        public static MidiDriver Instance {
            get {
                if (_instance == null) {
#if UNITY_ANDROID && !UNITY_EDITOR
                    _instance = new AndroidMidiDriver();
#else
                    _instance = new UnityMidiDriver();
#endif
                    if (Application.isPlaying)
                    {
                        MidiStateUpdater.CreateGameObject(
                            new MidiStateUpdater.Callback(_instance.Update),
                            new MidiStateUpdater.DeviceCallback(_instance.DeviceCallback));

                        _instance.Init();
                    }
                }
                return _instance;
            }
        }

#endregion
    }

    public class BoundDevice
    {
        public string Name;
        public bool IsBound;
        public bool AndroidPermissionRequested;
        public bool JustConnected;

        public BoundDevice(string deviceName)
        {
            Name = deviceName;
            IsBound = false;
            AndroidPermissionRequested = false;
            JustConnected = false;
        }
    }
}
