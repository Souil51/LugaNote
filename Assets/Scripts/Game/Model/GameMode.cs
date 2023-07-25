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

        private bool _withRandomAlteration;
        public bool WithRandomAlteration => _withRandomAlteration;

        public GameMode(int id, GameModeType gameModeType, IntervalMode intervalMode, bool withRandomAlteration)
        {
            _id = id;
            _gameModeType = gameModeType;
            _intervalMode = intervalMode;
            _withRandomAlteration = withRandomAlteration;
        }

        public override bool Equals(object obj)
        {
            if (obj is GameMode gameMode)
            {
                return GameModeType == gameMode.GameModeType && IntervalMode == gameMode.IntervalMode && WithRandomAlteration == gameMode.WithRandomAlteration;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id, Id, _gameModeType, GameModeType, _intervalMode, IntervalMode, _withRandomAlteration, WithRandomAlteration);
        }
    }
}
