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
    // MidiDriver for Unity (Windows) platform
    public class UnityMidiDriver : MidiDriver
    {
        [DllImport("MidiJackPlugin", EntryPoint = "MidiJackDequeueIncomingData")]
        public static extern ulong DequeueIncomingData();

        [DllImport("MidiJackPlugin", EntryPoint = "MidiJackCountEndpoints")]
        protected static extern int CountEndpoints();

        [DllImport("MidiJackPlugin", EntryPoint = "MidiJackOpenDevice")]
        protected static extern void OpenDevice(uint index);

        [DllImport("MidiJackPlugin")]
        protected static extern System.IntPtr MidiJackGetEndpointName(uint id);

        [DllImport("MidiJackPlugin", EntryPoint = "MidiJackCloseAllDevices")]
        protected static extern void CloseDevices();

        [DllImport("MidiJackPlugin", EntryPoint = "MidiJackCloseDevice")]
        protected static extern void CloseDevice(uint index);

        public UnityMidiDriver() : base()
        {

        }

        protected override void Update()
        {
            //Debug.Log("Update UnityMidiDriver " + Time.frameCount);

            base.Update();

            // Process the message queue.
            while (true)
            {
                // Pop from the queue.
                var data = DequeueIncomingData();
                if (data == 0) break;

                // Parse the message.
                var message = new MidiMessage(data);

                // Split the first byte.
                var statusCode = message.status >> 4;
                var channelNumber = message.status & 0xf;

                // Note on message?
                if (statusCode == 9)
                {
                    var velocity = 1.0f / 127 * message.data2 + 1;
                    _channelArray[channelNumber]._noteArray[message.data1] = velocity;
                    _channelArray[(int)MidiChannel.All]._noteArray[message.data1] = velocity;
                    // Debug.Log("note down Midi Driver");
                    if (noteOnDelegate != null)
                        noteOnDelegate((MidiChannel)channelNumber, message.data1, velocity - 1);
                }

                // Note off message?
                if (statusCode == 8 || (statusCode == 9 && message.data2 == 0))
                {
                    _channelArray[channelNumber]._noteArray[message.data1] = -1;
                    _channelArray[(int)MidiChannel.All]._noteArray[message.data1] = -1;
                    if (noteOffDelegate != null)
                        noteOffDelegate((MidiChannel)channelNumber, message.data1);
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

#if UNITY_EDITOR
                // Record the message.
                _totalMessageCount++;
                _messageHistory.Enqueue(message);
#endif
            }

#if UNITY_EDITOR
            // Truncate the history.
            while (_messageHistory.Count > 8)
                _messageHistory.Dequeue();
#endif
        }

        protected override void DeviceCallback()
        {
            base.DeviceCallback();

            GetDeviceNames();

            // Unplug detection
            for (int i = 0; i < allDevicesBound.Count; i++)
            {
                if (!allDevices.Contains(allDevicesBound[i].Name)
                    && allDevicesBound[i].IsBound)
                {
                    Debug.Log("Unplug detection " + allDevicesBound[i].Name);

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
                    OpenDevice(i);

                    // int indexToOpen = _instance.midiDroid.IndexOfDeviceNamed(allDevices[(int)i]);
                    // _instance.midiDroid.TryOpenDeviceAt(indexToOpen);

                    boundDevice.IsBound = true;
                    Debug.Log("Open device " + allDevices[(int)i]);

                    if (deviceConnectedDelegate != null)
                        deviceConnectedDelegate(allDevices[(int)i]);
                }
            }
        }

        protected static string GetEndpointName(uint id)
        {
            return Marshal.PtrToStringAnsi(MidiJackGetEndpointName(id));
        }

        protected void GetDeviceNames()
        {
            allDevices = new List<string>();
            var endpointCount = CountEndpoints();
            for (uint i = 0; i < endpointCount; i++)
            {
                string name = GetEndpointName(i);
                allDevices.Add(name);
                if (!allDevicesBound.Any(x => x.Name == name))
                {
                    allDevicesBound.Add(new BoundDevice(name));
                }
            }
        }
    }
}
