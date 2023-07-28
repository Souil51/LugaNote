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

        private Level _level;
        public Level Level => _level;

        public GameMode(int id, GameModeType gameModeType, IntervalMode intervalMode, Level level, bool withRandomAlteration)
        {
            _id = id;
            _gameModeType = gameModeType;
            _intervalMode = intervalMode;
            _withRandomAlteration = withRandomAlteration;
            _level = level;
        }

        public override bool Equals(object obj)
        {
            if (obj is GameMode gameMode)
            {
                return GameModeType == gameMode.GameModeType && IntervalMode == gameMode.IntervalMode && Level == gameMode.Level && WithRandomAlteration == gameMode.WithRandomAlteration;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = HashCode.Combine(_id, Id, _gameModeType, GameModeType, _intervalMode, IntervalMode, _level, Level);
            return HashCode.Combine(hash, _withRandomAlteration, WithRandomAlteration);
        }
    }
}
