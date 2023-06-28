using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts.Data
{
    public class SaveManager
    {
        private static readonly bool m_bUseFile = false;
        private static readonly string m_szSaveFileName = "save.json";

        public static Save GetSave()
        {
            Save save;

            if (m_bUseFile)
            {
                string szFilesPath = GetSaveLocation();

                if (!File.Exists(szFilesPath))
                {
                    InitGameSave();
                }

                StreamReader sr = new StreamReader(szFilesPath);
                string szJson = sr.ReadToEnd();

                save = JsonUtility.FromJson<Save>(szJson);

                sr.Close();
                sr.Dispose();
            }
            else
            {
                save = InitGameSave();
            }

            return save;
        }

        private static string GetSaveLocation()
        {
            return Application.persistentDataPath + "/" + m_szSaveFileName;
        }

        private static void SaveGameToFile(Save save)
        {
            string szFilesPath = GetSaveLocation();

            string jsonString = JsonUtility.ToJson(save);

            StreamWriter sw = new StreamWriter(szFilesPath);
            sw.Write(jsonString);
            sw.Close();
            sw.Dispose();
        }

        public static void SaveGame(Save save)
        {
            if (m_bUseFile)
                SaveGameToFile(save);
        }

        private static Save InitGameSave()
        {
            Save save = new Save();

            var lstModes = GameModeManager.GameModes;

            foreach (var gameMode in lstModes)
            {
                var gameModeData = new GameModeData();
                save.AddGameModeData(gameModeData);
            }

            if (m_bUseFile)
                SaveGameToFile(save);

            return save;
        }
    }

    [Serializable]
    public class Save
    {
        public List<GameModeData> _datas;

        public Save()
        {
            _datas = new List<GameModeData>();
        }

        public void AddGameModeData(GameModeData modeData)
        {
            _datas.Add(modeData);
        }

        public GameModeData GetGameModeData(int nIndex)
        {
            if (nIndex >= _datas.Count)
                return new GameModeData();
            else
                return _datas[nIndex];
        }
    }

    [Serializable]
    public class GameModeData
    {
        private List<Score> _scores;

        public GameModeData()
        {
            _scores = new List<Score>();
        }
    }

    [Serializable]
    public class Score
    {
        private int _value;
        public int Value => _value;
    }
}
