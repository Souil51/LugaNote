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
using MidiJack;

namespace Assets.MidiJack
{
    // MidiDriver for Android
    // Main difference with the Unity one : devive listing and permissions prompt
    public class AndroidMidiDriver : MidiDriver
    {
        private MidiDroid midiDroid;

        public AndroidMidiDriver() : base()
        {

        }

        protected override void Init()
        {
            base.Init();

            //Debug.Log("ANDROID");
            midiDroid = new MidiDroid();
            midiDroid.Start();
            midiDroid.callback.DroidMidiEvent += HandleMidiMessage;
            AndroidUtils.OnAllowCallback += OnAndroidAllowUSB;
            AndroidUtils.OnDenyCallback += OnAndroidDenyUSB;

            var permissionsGranted = GetPersistentPermissions();
            foreach (string device in permissionsGranted)
            {
                //Debug.Log("Device already known : " + device);

                var boundDevice = new BoundDevice(device);
                boundDevice.AndroidPermissionRequested = true;
                allDevicesBound.Add(boundDevice);
            }
        }

        protected override void Update()
        {
            base.Update();

            foreach(var item in _listNoteOn)
            {
                if (noteOnDelegate != null)
                    noteOnDelegate(item.Item1, item.Item2, item.Item3);
            }

            foreach(var item in _listNoteOff)
            {
                if (noteOnDelegate != null)
                    noteOffDelegate(item.Item1, item.Item2);
            }

            _listNoteOn.Clear();
            _listNoteOff.Clear();
        }

        protected override void DeviceCallback()
        {
            base.DeviceCallback();

            GetAndroidDevices();

            // Unplug detection
            for (int i = 0; i < allDevicesBound.Count; i++)
            {
                if (!allDevices.Contains(allDevicesBound[i].Name)
                    && allDevicesBound[i].IsBound)
                {
                    //Debug.Log("Unplug detection " + allDevicesBound[i].Name);

                    if (deviceDisconnectedDelegate != null)
                        deviceDisconnectedDelegate(allDevicesBound[i].Name);

                    allDevicesBound[i].IsBound = false;
                }
            }

            // plug detection
            for (uint i = 0; i < allDevices.Count; i++)
            {
                var boundDevice = allDevicesBound.Where(x => x.Name == allDevices[(int)i]).FirstOrDefault();
                if (boundDevice != null && !boundDevice.IsBound)
                {
                    // int indexToOpen = _instance.midiDroid.IndexOfDeviceNamed(allDevices[(int)i]);
                    // _instance.midiDroid.TryOpenDeviceAt(indexToOpen);
                    boundDevice.IsBound = true;
                    string deviceName = allDevices[(int)i];
                    if(!boundDevice.AndroidPermissionRequested)
                    {
                        USBPermissionHandler.RequestUSBPermission(deviceName);
                    }
                    else
                    {
                        midiDroid.FindADevice();
                        //Debug.Log("Open android device " + allDevices[(int)i]);

                        if (deviceConnectedDelegate != null)
                            deviceConnectedDelegate(allDevices[(int)i]);
                    }
                }
            }
        }

        private List<Tuple<MidiChannel, byte, float>> _listNoteOn = new List<Tuple<MidiChannel, byte, float>>();
        private List<Tuple<MidiChannel, byte>> _listNoteOff = new List<Tuple<MidiChannel, byte>>();

        private void HandleMidiMessage(object sender, MidiMessage message)
        {
            // Split the first byte.
            var statusCode = message.status >> 4;
            var channelNumber = message.status & 0xf;

            // Note on message?
            if (statusCode == 9)
            {
                //Debug.LogFormat("Getting {0} On", message.data1);
                var velocity = 1.0f / 127 * message.data2 + 1;
                _channelArray[channelNumber]._noteArray[message.data1] = velocity;
                _channelArray[(int)MidiChannel.All]._noteArray[message.data1] = velocity;
                //if (noteOnDelegate != null)
                //    noteOnDelegate((MidiChannel)channelNumber, message.data1, velocity - 1);

                _listNoteOn.Add(new Tuple<MidiChannel, byte, float>((MidiChannel)channelNumber, message.data1, velocity - 1));
            }

            // Note off message?
            if (statusCode == 8 || (statusCode == 9 && message.data2 == 0))
            {
                //Debug.LogFormat("Getting {0} Off", message.data1);
                _channelArray[channelNumber]._noteArray[message.data1] = -1;
                _channelArray[(int)MidiChannel.All]._noteArray[message.data1] = -1;
                //if (noteOffDelegate != null)
                //    noteOffDelegate((MidiChannel)channelNumber, message.data1);

                _listNoteOff.Add(new Tuple<MidiChannel, byte>((MidiChannel)channelNumber, message.data1));
            }

            // CC message?
            if (statusCode == 0xb)
            {
                // Normalize the value.
                var level = 1.0f / 127 * message.data2;
                // Update the channel if it already exists, or add a new channel.
                _channelArray[channelNumber]._knobMap[message.data1] = level;
                // Do again for All-ch.
                _channelArray[(int)MidiChannel.All]._knobMap[message.data1] = level;
                if (knobDelegate != null)
                    knobDelegate((MidiChannel)channelNumber, message.data1, level);
            }
        }

        private void GetAndroidDevices()
        {
            if (midiDroid == null) return;

            allDevices = midiDroid.getDeviceList();
            foreach (string deviceName in allDevices)
            {
                //Debug.Log(deviceName);
                if (!allDevicesBound.Any(x => x.Name == deviceName))
                {
                    allDevicesBound.Add(new BoundDevice(deviceName));
                }
            }
        }

        public ulong DequeueIncomingData()
        {
            return 0;
        }

        public void OnAndroidAllowUSB(string deviceName, string androidDeviceName)
        {
            //Debug.Log("MidiDriver OnAndroidAllowUSB " + deviceName + " " + androidDeviceName);
            midiDroid.FindADevice();

            var deviceFound = allDevices.Where(x => x == deviceName).FirstOrDefault();
            var boundDeviceFound = allDevicesBound.Where(x => x.Name == deviceFound).FirstOrDefault();
            if (deviceFound != null && allDevicesBound.Any(x => x.Name == deviceFound))
                boundDeviceFound.IsBound = true;
            else
            {
                boundDeviceFound = new BoundDevice(deviceFound);
                boundDeviceFound.IsBound = true;
                allDevicesBound.Add(boundDeviceFound);
            }

            boundDeviceFound.AndroidPermissionRequested = true;
            AddAndSavePersistentPermissions(deviceName);

            //Debug.Log("Open android device " + deviceFound);

            if (deviceConnectedDelegate != null)
                deviceConnectedDelegate(deviceFound);
        }

        public void OnAndroidDenyUSB(string deviceName, string androidDeviceName)
        {
            var boundDevice = allDevicesBound.Where(x => x.Name == deviceName).FirstOrDefault();
            if (boundDevice != null)
            {
                //boundDevice.AndroidPermissionRequested = true;
            }

            //Debug.Log("MidiDriver OnAndroidDenyUSB " + deviceName + " " + androidDeviceName);
        }
    }
}
