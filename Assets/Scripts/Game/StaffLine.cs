using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// StaffLine script handle note spawning
/// </summary>
public class StaffLine : MonoBehaviour
{
    private List<Note> _notes = new List<Note>();
    public List<Note> Notes => _notes;
    private int _id;


    private bool _isVisible;
    public bool IsVisible => _isVisible;

    private bool _isSpaceLine;
    public bool IsSpaceLine => _isSpaceLine;

    private PianoNote _naturalNote;
    public PianoNote Note
    {
        get
        {
            if (_alteration == Alteration.Natural)
                return _naturalNote;
            else if (_alteration == Alteration.Sharp)
                return _naturalNote + 1 > MusicHelper.HigherNote ? MusicHelper.HigherNote : _naturalNote + 1;
            else
                return _naturalNote - 1 < MusicHelper.LowerNote ? MusicHelper.LowerNote : _naturalNote - 1;
        }
    }

    public PianoNote NaturalNote => MusicHelper.ConvertToNaturalNote(Note);

    private Alteration _alteration;
    public Alteration Alteration => _alteration;

    

    private SpriteRenderer _sprtRenderer;

    private Staff _parent;
    public Staff Parent => _parent;

    public float Width => _sprtRenderer.size.x * transform.localScale.x;

    private void Awake()
    {
        _sprtRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeLine(Staff parent, int id, PianoNote note, bool visible, bool spaceLine)
    {
        this._parent = parent;
        this._isSpaceLine = spaceLine;
        this._isVisible = visible;
        this._id = id;

        // A Staff line PianoNote CANNOT be altered, this stuff have to be handle with the alteration
        if (MusicHelper.SharpNotes.Contains(note))
            note--;
        this._naturalNote = note;

        if(!_isVisible || _isSpaceLine)
        {
            _sprtRenderer.sprite = null;
        }
    }

    public void SetAlteration(Alteration alteration)
    {
        this._alteration = Alteration;
    }

    public void ResetAlteration()
    {
        this.SetAlteration(Alteration.Natural);
    }

    /// <summary>
    /// Instantiate a note on the line starting position to the line ending position
    /// </summary>
    public void SpawnNote(float scale,  float fromX, float toX)
    {
        string resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, this.Alteration);

        GameObject go = (GameObject)Instantiate(Resources.Load(resourceToLoad));
        go.transform.position = new Vector3(fromX, transform.position.y, transform.position.z);
        go.transform.localScale *= scale;

        int numberOfEmptyLineAbove = MusicHelper.GetAdditionnalEmptyLineAbove(Parent.StaffClef, this.Note);
        int numberOfEmptyLineBelow = MusicHelper.GetAdditionnalEmptyLineBelow(Parent.StaffClef, this.Note);

        var note = go.GetComponent<Note>();

        // Initialize note with this line alteration
        note.InitializeNote(this, numberOfEmptyLineBelow, numberOfEmptyLineAbove, this.Alteration);
        note.MoveTo(new Vector3(toX, transform.position.y, transform.position.z));

        // Debug.Log("Spawn note : " + note.AlteredNote);

        note.DestroyEvent += Note_DestroyEvent;

        _notes.Add(note);
    }

    private void Note_DestroyEvent(object sender, System.EventArgs args)
    {
        _notes.Remove(sender as Note);
    }
}
