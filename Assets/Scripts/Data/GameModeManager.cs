using Assets.Scripts.Game.Model;
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
                    new GameMode(1, GameModeType.Trebble, IntervalMode.Note, false),
                    new GameMode(2, GameModeType.Bass, IntervalMode.Note, false),
                    new GameMode(3, GameModeType.TrebbleBass, IntervalMode.Note, false),
                    new GameMode(4, GameModeType.Trebble, IntervalMode.Note, true),
                    new GameMode(5, GameModeType.Bass, IntervalMode.Note, true),
                    new GameMode(6, GameModeType.TrebbleBass, IntervalMode.Note, true)
                };
            }
        }

        public static GameMode GetGameMode(GameModeType gameModeType, IntervalMode intervalMode, bool withRandomAlteration)
        {
            var gameMode = new GameMode(0, gameModeType, intervalMode, withRandomAlteration);
            var existingGameMode = GameModes.Where(x => x.Equals(gameMode)).FirstOrDefault();

            return existingGameMode;
        }
    }
}
