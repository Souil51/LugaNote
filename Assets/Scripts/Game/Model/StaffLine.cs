using DG.Tweening;
using Guid = System.Guid;
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

    public ScaleFactor ScaleFactor = ScaleFactor.Screen;

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

        float xScale = 1f;
        if (ScaleFactor == ScaleFactor.Screen)
            xScale = 9.5f * ScreenManager.ScreenRatio;
        else if(ScaleFactor == ScaleFactor.Staff)
            xScale = 19f * _parent.transform.localScale.x * ScreenManager.ScreenRatio;

        gameObject.transform.localScale = new Vector3(xScale, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
    }

    public void SetAlteration(Alteration alteration)
    {
        this._alteration = Alteration;
    }

    public void ResetAlteration()
    {
        this.SetAlteration(Alteration.Natural);
    }

    public void SpawnNoteWithRandomAlteration(float scale, float fromX, float toX)
    {
        this.SpawnNote(scale, fromX, toX, true, false, Alteration.Natural, Guid.Empty);
    }

    public void SpawnNote(float scale, float fromX, float toX)
    {
        this.SpawnNote(scale, fromX, toX, false, false, Alteration.Natural, Guid.Empty);
    }

    public void SpawnNoteWithAlteration(float scale, float fromX, float toX, Alteration alteration)
    {
        this.SpawnNote(scale, fromX, toX, false, true, alteration, Guid.Empty);
    }

    public void SpawnNoteWithRandomAlterationWithGroup(float scale, float fromX, float toX, Guid groupId)
    {
        this.SpawnNote(scale, fromX, toX, true, false, Alteration.Natural, groupId);
    }

    public void SpawnNoteWithGroup(float scale, float fromX, float toX, Guid groupId)
    {
        this.SpawnNote(scale, fromX, toX, false, false, Alteration.Natural, groupId);
    }

    public void SpawnNoteWithAlterationWithGroup(float scale, float fromX, float toX, Alteration alteration, Guid groupId)
    {
        this.SpawnNote(scale, fromX, toX, false, true, alteration, groupId);
    }

    /// <summary>
    /// Instantiate a note on the line starting position to the line ending position
    /// </summary>
    private Note SpawnNote(float scale,  float fromX, float toX, bool withRandomAlteration, bool forceAlteration, Alteration forcedAlteration, Guid groupId)
    {
        var alteration = Alteration.Natural;

        string resourceToLoad = "";

        if (forceAlteration)
        {
            resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, forcedAlteration);
            alteration = forcedAlteration;
        }
        else if (!withRandomAlteration)
        {
            resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, this.Alteration);
            alteration = this.Alteration;
        }
        else
        {
            bool sharpable = !MusicHelper.NotNaturallySharpableNotes.Contains(this.NaturalNote);
            bool flatable = !MusicHelper.NotNaturallyFlatableNotes.Contains(this.NaturalNote);

            if (sharpable && flatable) // If sharpable or flatable -> random in the Alteration Enum range
            {
                int rand = Random.Range(0, 3);
                alteration = (Alteration)rand;
            }
            else if (sharpable || flatable) // Else, random to know if we use alteration or natural not
            {
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    alteration = sharpable ? Alteration.Sharp : Alteration.Flat;
                }
            }

            resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, alteration);
        }

        GameObject go = (GameObject)Instantiate(Resources.Load(resourceToLoad));
        go.transform.position = new Vector3(fromX, transform.position.y, transform.position.z);
        go.transform.localScale *= scale;

        int numberOfEmptyLineAbove = MusicHelper.GetAdditionnalEmptyLineAbove(Parent.StaffClef, this.Note);
        int numberOfEmptyLineBelow = MusicHelper.GetAdditionnalEmptyLineBelow(Parent.StaffClef, this.Note);

        var note = go.GetComponent<Note>();

        // Initialize note with this line alteration
        if(groupId == Guid.Empty)
            note.InitializeNote(this, numberOfEmptyLineBelow, numberOfEmptyLineAbove, alteration);
        else
            note.InitializeNote(this, numberOfEmptyLineBelow, numberOfEmptyLineAbove, alteration, groupId);

        note.MoveTo(new Vector3(toX, transform.position.y, transform.position.z));

        note.DestroyEvent += Note_DestroyEvent;

        _notes.Add(note);

        return note;
    }

    private void Note_DestroyEvent(object sender, System.EventArgs args)
    {
        _notes.Remove(sender as Note);
    }
}
