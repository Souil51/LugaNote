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
    }
}
