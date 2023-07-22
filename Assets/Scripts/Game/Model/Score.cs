using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    [Serializable]
    public class Score : INotifyPropertyChanged
    {
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
                OnPropertyChanged("DateString");
            }
        }

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
            Value = value;
            Date = date;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
