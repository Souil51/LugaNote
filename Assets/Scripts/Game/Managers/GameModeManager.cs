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
                                foreach(Inversion inversion in Enum.GetValues(typeof(Inversion)))
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        bool accidentals = i == 0;
                                        for (int j = 0; j < 2; j++)
                                        {
                                            bool inversions = j == 0;
                                            _gameModes.Add(new GameMode(index, gameModeType, interval, level, accidentals, inversions));

                                            index++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return _gameModes;
            }
        }

        public static GameMode GetGameMode(GameModeType gameModeType, IntervalMode intervalMode, Level level, bool withRandomAccidental, bool withInversion)
        {
            var gameMode = new GameMode(0, gameModeType, intervalMode, level, withRandomAccidental, withInversion);
            var existingGameMode = GameModes.Where(x => x.Equals(gameMode)).FirstOrDefault();

            return existingGameMode;
        }
    }
}
