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
                    new GameMode(1),
                    new GameMode(2),
                    new GameMode(3),
                };
            }
        }
    }

    public class GameMode
    {
        private int _id;
        public int Id => _id;

        public GameMode(int id)
        {
            _id = id;
        }
    }
}
