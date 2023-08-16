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

        private List<ControllerType> _usedControllers;
        public  List<ControllerType> UsedControllers 
        {
            get => _usedControllers;
            set
            {
                _usedControllers = value;
                OnPropertyChanged();
                OnPropertyChanged("MidiUsed");
                OnPropertyChanged("VisualUsed");
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

        [JsonIgnore]
        public bool MidiUsed => _usedControllers != null && _usedControllers.Any(x => x == ControllerType.MIDI);

        [JsonIgnore]
        public bool VisualUsed => _usedControllers != null && _usedControllers.Any(x => x == ControllerType.Visual);

        public Score(int value, DateTime date, List<ControllerType> usedControllers)
        {
            Value = value;
            Date = date;
            UsedControllers = usedControllers;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
