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
                    new GameMode(1, GameModeType.Trebble, IntervalMode.Note),
                    new GameMode(2, GameModeType.Bass, IntervalMode.Note),
                    new GameMode(3, GameModeType.TrebbleBass, IntervalMode.Note),
                };
            }
        }

        public static GameMode GetGameMode(GameModeType gameModeType, IntervalMode intervalMode)
        {
            var gameMode = new GameMode(0, gameModeType, intervalMode);
            var existingGameMode = GameModes.Where(x => x.Equals(gameMode)).FirstOrDefault();

            return existingGameMode;
        }
    }
}
