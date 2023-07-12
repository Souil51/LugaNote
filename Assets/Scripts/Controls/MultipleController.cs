using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class MultipleController : MonoBehaviour, IController
{
    private PianoNote _higherNote;
    public PianoNote HigherNote => _higherNote;

    private PianoNote _lowerNote;
    public PianoNote LowerNote => _lowerNote;

    private List<PianoNote> _notesDown = new List<PianoNote>();
    public List<PianoNote> NotesDown => _notesDown;

    private List<PianoNote> _notesUp = new List<PianoNote>();
    public List<PianoNote> NotesUp => _notesUp;

    private List<PianoNote> _notes = new List<PianoNote>();
    public List<PianoNote> Notes => _notes;

    public int C4Offset => 0;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;

    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<PianoNote> _notesWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesWithOffset => _notesWithOffset;

    private List<PianoNote> _notesDownWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<PianoNote> _notesUpWithOffset = new List<PianoNote>();
    public List<PianoNote> NotesUpWithOffset => _notesUpWithOffset;

    public string Label
    {
        get
        {
            return string.Join(", ", _controllers.Select(x => x.Label).ToList());
        }
    }

    public List<PianoNote> AvailableNotes
    {
        get
        {
            var notes = _controllers.SelectMany(x => x.AvailableNotes).ToList();
            return notes.Distinct().ToList();
        }
    }

    public bool IsControllerUIVisible
    {
        get
        {
            var visualController = _controllers.Where(x => x.GetType() == typeof(VisualController)).FirstOrDefault();
            if (visualController != null)
            {
                return visualController.IsControllerUIVisible;
            }
            return true;
        }
    }

    private bool _hasUI = false;
    public bool HasUI => _hasUI;

    private List<IController> _controllers = new List<IController>();
    private MidiConfigurationHelper _configurationHelper;

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;

    private void Awake()
    {
        UpdateNotesWithOffset();
        HideControllerUI();
    }

    private void Update()
    {
        _notesDown.Clear();
        _notesUp.Clear();
        _notes.Clear();

        _notesDown = _controllers.SelectMany(x => x.NotesDown).Distinct().ToList();
        _notesUp = _controllers.SelectMany(x => x.NotesUp).Distinct().ToList();
        _notes = _controllers.SelectMany(x => x.Notes).Distinct().ToList();

        UpdateNotesWithOffset();

        if (_notesDown.Count > 0)
            NoteDown?.Invoke(this, new NoteEventArgs(_notesDown[0]));
    }
    private void OnDisable()
    {
        var midiController = _controllers.Where(x => x.GetType() == typeof(MidiController)).FirstOrDefault();
        if (midiController != null)
        {
            midiController.Configuration -= MidiController_Configuration;
        }
    }

    public void InitializeController(params IController[] controllers)
    {
        _controllers = new List<IController>(controllers);

        _lowerNote = _controllers.Select(x => x.LowerNote).Max();
        _higherNote = _controllers.Select(x => x.HigherNote).Min();

        var midiController = _controllers.Where(x => x.GetType() == typeof(MidiController)).FirstOrDefault();
        if (midiController != null)
        {
            midiController.Configuration += MidiController_Configuration;
        }

        var visualController = _controllers.Where(x => x.GetType() == typeof(VisualController)).FirstOrDefault();
        if (visualController != null)
        {
            _hasUI = true;
        }
    }

    private void MidiController_Configuration(object sender, ConfigurationEventArgs e)
    {
        _lowerNote = _controllers.Select(x => x.LowerNote).Max();
        _higherNote = _controllers.Select(x => x.HigherNote).Min();

        Configuration?.Invoke(this, e);
    }

    public void Configure(Canvas canvas)
    {
        var midiController = _controllers.Where(x => x.GetType() == typeof(MidiController)).FirstOrDefault();
        if (midiController != null)
        {
            midiController.Configure(canvas);
        }
    }

    private void UpdateNotesWithOffset()
    {
        _notesWithOffset = new List<PianoNote>(Notes);
        _notesDownWithOffset = new List<PianoNote>(NotesDown);
        if (C4Offset != 0)
        {
            _notesWithOffset = _notesWithOffset.Select(x => x + C4Offset).ToList();
            _notesDownWithOffset = _notesDownWithOffset.Select(x => x + C4Offset).ToList();
        }
    }

    public void HideControllerUI()
    {
        foreach(var controller in _controllers)
        {
            controller.HideControllerUI();
        }
    }

    public void ShowControllerUI()
    {
        foreach (var controller in _controllers)
        {
            controller.ShowControllerUI();
        }
    }

}
