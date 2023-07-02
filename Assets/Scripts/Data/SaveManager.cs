using Assets.Scripts.Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts.Data
{
    public class SaveManager
    {
        private static readonly bool m_bUseFile = true;
        private static readonly string m_szSaveFileName = "save.json";

        private static Save _save;
        public static Save Save 
        {
            get
            {
                if(_save == null)
                    _save = GetSave();

                return _save;
            }
            private set
            {
                _save = value;
            }
        }

        private static Save GetSave()
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

                save = JsonConvert.DeserializeObject<Save>(szJson);

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

            string jsonString = JsonConvert.SerializeObject(save);

            StreamWriter sw = new StreamWriter(szFilesPath);
            sw.Write(jsonString);
            sw.Close();
            sw.Dispose();
        }

        public static void SaveGame()
        {
            if (m_bUseFile)
                SaveGameToFile(Save);
        }

        public static void AddScore(GameMode gameMode, int score, DateTime date)
        {
            Save.AddScore(gameMode, score, date);
        }

        private static Save InitGameSave()
        {
            Save save = new Save();

            var lstModes = GameModeManager.GameModes;

            foreach (var gameMode in lstModes)
            {
                var gameModeData = new GameModeData(gameMode);
                save.AddGameModeData(gameModeData);
            }

            if (m_bUseFile)
                SaveGameToFile(save);

            return save;
        }
    }
}
