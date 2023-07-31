using Assets.Scripts.Game.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultipleController : MonoBehaviour, IController
{
    private PianoNote _higherNote;
    public PianoNote HigherNote => _higherNote;

    private PianoNote _lowerNote;
    public PianoNote LowerNote => _lowerNote;

    private List<ControllerNote> _notesDown = new List<ControllerNote>();
    public List<ControllerNote> NotesDown => _notesDown;

    private List<ControllerNote> _notesUp = new List<ControllerNote>();
    public List<ControllerNote> NotesUp => _notesUp;

    private List<ControllerNote> _notes = new List<ControllerNote>();
    public List<ControllerNote> Notes => _notes;

    public int C4Offset => 0;

    public PianoNote HigherNoteWithOffset => HigherNote + C4Offset;

    public PianoNote LowerNoteWithOffset => LowerNote + C4Offset;

    private List<ControllerNote> _notesWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesWithOffset => _notesWithOffset;

    private List<ControllerNote> _notesDownWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesDownWithOffset => _notesDownWithOffset;

    private List<ControllerNote> _notesUpWithOffset = new List<ControllerNote>();
    public List<ControllerNote> NotesUpWithOffset => _notesUpWithOffset;

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

    public bool IsConfigurable
    {
        get
        {
            var midiController = _controllers.Where(x => x.GetType() == typeof(MidiController)).FirstOrDefault();
            return midiController != null;
        }
    }

    private List<IController> _controllers = new List<IController>();
    private MidiConfigurationHelper _configurationHelper;

    public bool IsReplacementModeForced
    {
        get
        {
            bool replacementModePossible = _controllers.Any(x => !x.IsReplacementModeForced);
            return !replacementModePossible;
        }
    }

    public event NoteDownEventHandler NoteDown;
    public event ConfigurationEventHandled Configuration;
    public event ConfigurationDestroyedEventHandled ConfigurationDestroyed;

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
            NoteDown?.Invoke(this, new ControllerNoteEventArgs(_notesDown[0]));
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
            midiController.ConfigurationDestroyed += MidiController_ConfigurationDestroyed;
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

    private void MidiController_ConfigurationDestroyed(object sender, Assets.GameObjectEventArgs e)
    {
        ConfigurationDestroyed?.Invoke(sender, e);
    }

    public GameObject Configure()
    {
        var midiController = _controllers.Where(x => x.GetType() == typeof(MidiController)).FirstOrDefault();
        if (midiController != null)
        {
            return midiController.Configure();
        }

        return null;
    }

    private void UpdateNotesWithOffset()
    {
        _notesWithOffset = new List<ControllerNote>(Notes);
        _notesDownWithOffset = new List<ControllerNote>(NotesDown);
        if (C4Offset != 0)
        {
            _notesWithOffset = _notesWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, x.IsReplaceableByDefault)).ToList();
            _notesDownWithOffset = _notesDownWithOffset.Select(x => new ControllerNote(x.Note + C4Offset, x.IsReplaceableByDefault)).ToList();
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
