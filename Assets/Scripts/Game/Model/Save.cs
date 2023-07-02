using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
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

        public GameModeData GetGameModeData(GameModeType gameModeType, IntervalMode intervalMode)
        {
            var gameMode = new GameMode(0, gameModeType, intervalMode);
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
    }
}
