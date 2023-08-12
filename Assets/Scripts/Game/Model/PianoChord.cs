using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    public class PianoChord
    {
        private PianoNote _baseNote;
        public PianoNote BaseNote => _baseNote;

        private List<PianoNote> _notes;
        public List<PianoNote> Notes => _notes;

        private Inversion _inversion;
        public Inversion Inversion => _inversion;

        private Tonality _tonality;
        public Tonality Tonality => _tonality;

        private PianoNote _minNote;
        public PianoNote MinNote => _minNote;

        private PianoNote _maxNote;
        public PianoNote MaxNote => _maxNote;

        private bool _withAccidental;
        public bool WithAccidental => _withAccidental;

        public PianoChord(PianoNote baseNote, Inversion inversion, Tonality tonality)
        {
            _baseNote = baseNote;
            _inversion = inversion;
            _tonality = tonality;
            _notes = new List<PianoNote>();

            GenerateNotes();
            _withAccidental = Notes.Any(x => MusicHelper.IsSharp(x));
        }

        private void GenerateNotes()
        {
            if(_inversion == Inversion.None)
            {
                _minNote = _baseNote;
                _notes.Add(_minNote);

                if (_tonality == Tonality.Major)
                    _notes.Add(_baseNote + 4);
                else
                    _notes.Add(_baseNote + 3);

                _maxNote = _baseNote + 7;
                _notes.Add(_maxNote);
            }
            else if(_inversion == Inversion.firstInverstion)
            {
                if (_tonality == Tonality.Major)
                    _minNote = _baseNote - 8;
                else
                    _minNote = _baseNote - 9;


                _notes.Add(_minNote);

                _notes.Add(_baseNote - 5);

                _maxNote = _baseNote;
                _notes.Add(_maxNote);
            }
            else if (_inversion == Inversion.secondInversion)
            {
                _minNote = _baseNote - 5;
                _notes.Add(_minNote);
                _notes.Add(_baseNote);

                if (_tonality == Tonality.Major)
                    _maxNote = _baseNote + 4;
                else
                    _maxNote = _baseNote + 3;

                _notes.Add(_maxNote);
            }
        }
    }
}
