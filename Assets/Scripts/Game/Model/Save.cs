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

        public GameModeData GetGameModeData(int nIndex)
        {
            if (nIndex >= _datas.Count)
                return null;
            else
                return _datas[nIndex];
        }

        public void AddScore(GameModeData gameModeData, int score, DateTime date)
        {
            var saveGameModeData = _datas.FirstOrDefault(x => x.Equals(gameModeData));

            if (saveGameModeData != null)
            {
                saveGameModeData.AddScore(score, date);
            }
        }
    }
}
