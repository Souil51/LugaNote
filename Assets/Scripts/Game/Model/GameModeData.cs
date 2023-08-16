using Assets.Scripts.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    [Serializable]
    public class GameModeData
    {
        private GameMode _gameMode;
        public GameMode GameMode => _gameMode;

        private List<Score> _scores;
        public List<Score> Scores => _scores;

        public GameModeData(GameMode gameMode)
        {
            _gameMode = gameMode;
            _scores = new List<Score>();
        }

        public void AddScore(int score, DateTime date, List<ControllerType> usedControllers)
        {
            _scores.Add(new Score(score, date, usedControllers));
        }

        public override bool Equals(object obj)
        {
            if(obj is GameModeData gameModeData)
            {
                return this.GameMode.Equals(gameModeData.GameMode);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_gameMode, GameMode, _scores, Scores);
        }
    }
}
