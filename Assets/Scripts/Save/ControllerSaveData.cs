using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Save
{
    [Serializable]
    public class ControllerSaveData
    {
        public ControllerType ControllerType;
        public PianoNote MidiLowerNote;
        public PianoNote MidiHigherNote;

        public ControllerSaveData(ControllerType controllerType, PianoNote midiLowerNote, PianoNote midiHigherNote)
        {
            ControllerType = controllerType;
            MidiLowerNote = midiLowerNote;
            MidiHigherNote = midiHigherNote;
        }
    }
}
