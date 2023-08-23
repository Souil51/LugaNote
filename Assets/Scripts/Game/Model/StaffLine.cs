using DG.Tweening;
using Guid = System.Guid;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UIElements;

/// <summary>
/// StaffLine script handle note spawning
/// </summary>
public class StaffLine : MonoBehaviour
{
    private readonly List<Note> _notes = new();
    public List<Note> Notes => _notes;

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
            if (_accidental == Accidental.Natural)
                return _naturalNote;
            else if (_accidental == Accidental.Sharp)
                return _naturalNote + 1 > MusicHelper.HigherNote ? MusicHelper.HigherNote : _naturalNote + 1;
            else
                return _naturalNote - 1 < MusicHelper.LowerNote ? MusicHelper.LowerNote : _naturalNote - 1;
        }
    }

    public PianoNote NaturalNote => MusicHelper.ConvertToNaturalNote(Note);

    private Accidental _accidental;
    public Accidental Accidental => _accidental;

    

    private SpriteRenderer _sprtRenderer;

    private Staff _parent;
    public Staff Parent => _parent;

    public float Width => _sprtRenderer.size.x * transform.localScale.x;

    private void Awake()
    {
        _sprtRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeLine(Staff parent, PianoNote note, bool visible, bool spaceLine)
    {
        this._parent = parent;
        this._isSpaceLine = spaceLine;
        this._isVisible = visible;

        // A Staff line PianoNote CANNOT be altered, this stuff have to be handle with the accidental
        if (MusicHelper.IsSharp(note))
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

    public void SetAccidental(Accidental accidental) => this._accidental = accidental;

    public void ResetAccidental() => this.SetAccidental(Accidental.Natural);

    public void SpawnRandomNote(float scale, float fromX, float toX, Guid groupId = default) => this.SpawnRandomNote(scale, fromX, toX, false, false, Accidental.Natural, groupId);

    public void SpawnRandomNoteWithAccidental(float scale, float fromX, float toX, Accidental accidental, Guid groupId = default) => this.SpawnRandomNote(scale, fromX, toX, false, true, accidental, groupId);

    public void SpawnRandomNoteWithRandomAccidental(float scale, float fromX, float toX, Guid groupId = default) => this.SpawnRandomNote(scale, fromX, toX, true, false, Accidental.Natural, groupId);

    public Note SpawnNote(PianoNote note, float scale, float fromX, float toX, Guid groupId)
    {
        bool isSharp = MusicHelper.IsSharp(note);
        var accidental = isSharp ? Accidental.Sharp : Accidental.Natural;
        string resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, accidental);

        return InstantiateNote(resourceToLoad, scale, fromX, toX, accidental, groupId);
    }

    /// <summary>
    /// Instantiate a note on the line starting position to the line ending position
    /// </summary>
    private Note SpawnRandomNote(float scale,  float fromX, float toX, bool withRandomAccidental, bool forceAccidental, Accidental forcedAccidental, Guid groupId)
    {
        var accidental = Accidental.Natural;

        string resourceToLoad;
        if (forceAccidental)
        {
            resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, forcedAccidental);
            accidental = forcedAccidental;
        }
        else if (!withRandomAccidental)
        {
            resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, this.Accidental);
            accidental = this.Accidental;
        }
        else
        {
            bool sharpable = MusicHelper.IsNaturallySharpable(this.NaturalNote);
            bool flatable = MusicHelper.IsNaturallyFlatable(this.NaturalNote);

            if (sharpable && flatable) // If sharpable or flatable -> random in the Accidental Enum range
            {
                int rand = Random.Range(0, 3);
                accidental = (Accidental)rand;
            }
            else if (sharpable || flatable) // Else, random to know if we use accidental or natural not
            {
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    accidental = sharpable ? Accidental.Sharp : Accidental.Flat;
                }
            }

            resourceToLoad = StaticResource.GET_PREFAB_NOTE(!IsVisible && !IsSpaceLine, accidental);
        }

        return InstantiateNote(resourceToLoad, scale, fromX, toX, accidental, groupId);
    }

    private Note InstantiateNote(string resourceToLoad, float scale, float fromX, float toX, Accidental accidental, Guid groupId)
    {
        GameObject go = (GameObject)Instantiate(Resources.Load(resourceToLoad));
        go.transform.position = new Vector3(fromX, transform.position.y, transform.position.z);
        go.transform.localScale *= scale;

        int numberOfEmptyLineAbove = MusicHelper.GetAdditionnalEmptyLineAbove(Parent.StaffClef, this.Note);
        int numberOfEmptyLineBelow = MusicHelper.GetAdditionnalEmptyLineBelow(Parent.StaffClef, this.Note);

        var noteComponent = go.GetComponent<Note>();

        // Initialize note with this line accidental
        if (groupId == default)
            noteComponent.InitializeNote(this, numberOfEmptyLineBelow, numberOfEmptyLineAbove, accidental);
        else
            noteComponent.InitializeNote(this, numberOfEmptyLineBelow, numberOfEmptyLineAbove, accidental, groupId);

        noteComponent.MoveTo(new Vector3(toX, transform.position.y, transform.position.z));

        noteComponent.DestroyEvent += Note_DestroyEvent;

        _notes.Add(noteComponent);

        return noteComponent;
    }

    private void Note_DestroyEvent(object sender, System.EventArgs args) => _notes.Remove(sender as Note);
}
