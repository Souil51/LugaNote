using Assets.Scripts.Game.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Save
{
    [Serializable]
    public class Save
    {
        public List<GameModeData> _datas;
        public List<ControllerSaveData> _controllersData;
        public List<DeviceData> _devicesDatas;
        public GameMode _lastGameMode;
        public bool _lastReplacementMode;

        public Save()
        {
            _datas = new List<GameModeData>();
        }

        public void AddGameModeData(GameModeData modeData)
        {
            _datas.Add(modeData);
        }

        public GameModeData GetGameModeData(GameModeType gameModeType, IntervalMode intervalMode, Level level, bool guessName, bool withRandomAccidental, bool withInversion)
        {
            var gameMode = new GameMode(0, gameModeType, intervalMode, level, guessName, withRandomAccidental, withInversion);
            return _datas.FirstOrDefault(x => x.GameMode.Equals(gameMode));
        }

        public void AddScore(GameMode gameMode, int score, DateTime date)
        {
            var saveGameModeData = _datas.FirstOrDefault(x => x.GameMode.Equals(gameMode));

            if (saveGameModeData != null)
            {
                saveGameModeData.AddScore(score, date);
            }
            else
            {
                var datas = new GameModeData(gameMode);
                datas.AddScore(score, date);
                AddGameModeData(datas);
            }
        }

        public List<ControllerSaveData> GetControllersData()
        {
            return _controllersData;
        }

        public ControllerSaveData GetControllerData(string deviceName)
        {
            return _controllersData.Where(x => x.DeviceName == deviceName).FirstOrDefault();
        }

        public void SetControllerData(ControllerType controllerType, string deviceName, PianoNote midiLowerNote, PianoNote midiHigherNote)
        {
            var existingData = GetControllerData(deviceName);
            if(existingData != null)
            {
                existingData.ControllerType = controllerType;
                existingData.MidiLowerNote = midiLowerNote;
                existingData.MidiHigherNote = midiHigherNote;
            }
            else
            {
                var newData = new ControllerSaveData(controllerType, deviceName, midiLowerNote, midiHigherNote);
                _controllersData.Add(newData);
            }
        }

        public List<DeviceData> GetDevicesDatas()
        {
            return _devicesDatas;
        }

        public DeviceData GetDeviceData(string name)
        {
            return _devicesDatas?.FirstOrDefault(x => x.Name == name);
        }

        public DeviceData AddDeviceData(string name)
        {
            DeviceData newDevice = _devicesDatas?.FirstOrDefault(x => x.Name == name);

            if (newDevice == null)
            {
                newDevice = new DeviceData(name);
                _devicesDatas?.Add(newDevice);
            }

            return newDevice;
        }

        public void UpdateDeviceData(string name, bool androidPermissionRequested, bool androidPermissionResult)
        {
            var device = _devicesDatas?.FirstOrDefault(x => x.Name == name);

            if (device != null)
            {
                device.AndroidPermissionRequested = androidPermissionRequested;
                device.AndroidPermission = androidPermissionResult;
            }
        }

        public void SetLastGameMode(GameMode gameMode, bool replacementMode)
        {
            this._lastGameMode = gameMode;
            this._lastReplacementMode = replacementMode;
        }
    }
}
