using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    /// <summary>
    /// Score with index
    /// Used for indexed leader board
    /// </summary>
    public class LeaderboardScore : Score
    {
        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

        public LeaderboardScore(int index, int value, DateTime date) : base(value, date)
        {
            Index = index;
        }
    }
}
