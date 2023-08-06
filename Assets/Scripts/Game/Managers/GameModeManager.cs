using Assets.Scripts.Game.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class GameModeManager
    {
        private static List<GameMode> _gameModes = new List<GameMode>();
        public static List<GameMode> GameModes
        {
            get
            {
                if (_gameModes.Count == 0)
                {
                    int index = 1;
                    foreach (GameModeType gameModeType in Enum.GetValues(typeof(GameModeType)))
                    {
                        foreach (IntervalMode interval in Enum.GetValues(typeof(IntervalMode)))
                        {
                            foreach (Level level in Enum.GetValues(typeof(Level)))
                            {
                                for(int i = 0; i < 2; i++)
                                {
                                    bool alterations = i == 0;

                                    _gameModes.Add(new GameMode(index, gameModeType, interval, level, alterations));

                                    index++;
                                }
                            }
                        }
                    }
                }

                return _gameModes;
            }
        }

        public static GameMode GetGameMode(GameModeType gameModeType, IntervalMode intervalMode, Level level, bool withRandomAlteration)
        {
            var gameMode = new GameMode(0, gameModeType, intervalMode, level, withRandomAlteration);
            var existingGameMode = GameModes.Where(x => x.Equals(gameMode)).FirstOrDefault();

            return existingGameMode;
        }
    }
}
