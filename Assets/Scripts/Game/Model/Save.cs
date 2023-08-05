using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    [Serializable]
    public class Save
    {
        public List<GameModeData> _datas;
        public ControllerSaveData _controllerData;
        public List<DeviceData> _devicesDatas;

        public Save()
        {
            _datas = new List<GameModeData>();
        }

        public void AddGameModeData(GameModeData modeData)
        {
            _datas.Add(modeData);
        }

        public GameModeData GetGameModeData(GameModeType gameModeType, IntervalMode intervalMode, Level level, bool withRandomAlteration)
        {
            var gameMode = new GameMode(0, gameModeType, intervalMode, level, withRandomAlteration);
            return _datas.FirstOrDefault(x => x.GameMode.Equals(gameMode));
        }

        public void AddScore(GameMode gameMode, int score, DateTime date)
        {
            var saveGameModeData = _datas.FirstOrDefault(x => x.GameMode.Equals(gameMode));

            if (saveGameModeData != null)
            {
                saveGameModeData.AddScore(score, date);
            }
        }

        public ControllerSaveData GetControllerData()
        {
            return _controllerData;
        }

        public void SetControllerData(ControllerType controllerType, PianoNote midiLowerNote, PianoNote midiHigherNote)
        {
            _controllerData = new ControllerSaveData(controllerType, midiLowerNote, midiHigherNote);
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
    }
}
