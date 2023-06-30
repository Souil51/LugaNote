using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class GameModeManager
    {
        public static List<GameMode> GameModes
        {
            get
            {
                return new List<GameMode>()
                {
                    new GameMode(1, GameModeType.Trebble, IntervalMode.Note),
                    new GameMode(2, GameModeType.Bass, IntervalMode.Note),
                    new GameMode(3, GameModeType.TrebbleBass, IntervalMode.Note),
                };
            }
        }
    }

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
    }
}
