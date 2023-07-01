using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    [Serializable]
    public class GameMode
    {
        private int _id;
        public int Id => _id;

        private GameModeType _gameModeType;
        public GameModeType GameModeType => _gameModeType;

        private IntervalMode _intervalMode;
        public IntervalMode IntervalMode => _intervalMode;


        public GameMode(int id, GameModeType gameModeType, IntervalMode intervalMode)
        {
            _id = id;
            _gameModeType = gameModeType;
            _intervalMode = intervalMode;
        }

        public override bool Equals(object obj)
        {
            if (obj is GameMode gameMode)
            {
                return GameModeType == gameMode.GameModeType && IntervalMode == gameMode.IntervalMode;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id, Id, _gameModeType, GameModeType, _intervalMode, IntervalMode);
        }
    }
}
