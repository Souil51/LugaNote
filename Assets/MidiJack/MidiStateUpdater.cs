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
using System.Collections;
using UnityEngine;

namespace MidiJack
{
    public class MidiStateUpdater : MonoBehaviour
    {
        public delegate void Callback();
        public delegate void DeviceCallback();

        public static void CreateGameObject(Callback callback, DeviceCallback deviceCallback)
        {
            var go = new GameObject("MIDI Updater");

            GameObject.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideInHierarchy;

            var updater = go.AddComponent<MidiStateUpdater>();
            updater._callback = callback;
            updater._deviceCallback = deviceCallback;

            updater.StartDeviceCoroutine();
        }

        Callback _callback; // MIDI info callback, called every frames to get notes
        DeviceCallback _deviceCallback;// MIDI device callback, called every seconds to detect device connection/disconnection

        void Update()
        {
            _callback();
        }

        void StartDeviceCoroutine()
        {
            StartCoroutine(Co_DeviceCallback());
        }

        IEnumerator Co_DeviceCallback()
        {
            while (true)
            {
                _deviceCallback();
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
