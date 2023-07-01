using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    [Serializable]
    public class Score
    {
        private int _value;
        public int Value => _value;

        private DateTime _date;
        public DateTime Date => _date;

        [JsonIgnore]
        public string DateString
        {
            get
            {
                if (Date > DateTime.Now.AddYears(-1))
                    return Date.ToString("dd/MM");
                else
                    return Date.ToString("MM/yyyy");
            }
        }

        public Score(int value, DateTime date)
        {
            _value = value;
            _date = date;
        }
    }
}
