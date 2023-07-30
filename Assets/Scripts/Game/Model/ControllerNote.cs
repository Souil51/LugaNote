using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    public class ControllerNote
    {
        private bool _isReplaceableByDefault;
        public bool IsReplaceableByDefault => _isReplaceableByDefault;

        private PianoNote _note;
        public PianoNote Note => _note;

        public ControllerNote(PianoNote note, bool isReplaceable)
        {
            _note = note;
            _isReplaceableByDefault = isReplaceable;
        }
    }
}
