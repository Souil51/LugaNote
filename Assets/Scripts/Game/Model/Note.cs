using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

/// <summary>
/// Note script
/// Handle notes
/// </summary>
public class Note : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteGoodGuess;
    [SerializeField] private SpriteRenderer SpriteBadGuess;

    public delegate void DestroyEventHandler(object sender, EventArgs args);
    public event DestroyEventHandler DestroyEvent;

    private bool _isMoving = false;
    public bool IsMoving => _isMoving;

    private StaffLine _parent;
    public StaffLine Parent => _parent;

    private bool _isActive = true;
    public bool IsActive => _isActive;

    private Alteration _alteration;
    public Alteration Alteration => _alteration;

    // Get the real note with alteration
    // The parent is always a natural note
    public PianoNote PianoNote
    {
        get
        {
            if (Alteration == Alteration.Sharp)
                return Parent.Note < MusicHelper.HigherNote ? Parent.Note + 1 : MusicHelper.HigherNote;
            else if(Alteration == Alteration.Flat)
                return Parent.Note > MusicHelper.LowerNote ? Parent.Note - 1 : MusicHelper.LowerNote;
            else
                return Parent.Note;
        }
    }

    public PianoNote NaturalNote => MusicHelper.ConvertToNaturalNote(PianoNote);

    private SpriteRenderer _sprtRenderer;

    private Tween _movement;

    private long _creationTimestamp;
    public long CreationTimestamp => _creationTimestamp;

    private void Awake()
    {
        _sprtRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeNote(StaffLine parent, int emptyLinesBelow, int emptyLinesAbove, Alteration alteration)
    {
        _creationTimestamp = DateTime.Now.Ticks;
        _parent = parent;
        _alteration = alteration;

        // The staff display only 5 lines.
        // For lower or higher notes than that empty lines have to be displayed for better accuracy
        if (emptyLinesAbove > 0 || emptyLinesBelow > 0)
        {
            float distance = Parent.Parent.LineDistanceSpacing / transform.localScale.x;
            float offset = Parent.IsSpaceLine || Parent.IsVisible ? distance : 2f * distance;

            if (emptyLinesBelow > 0)
            {
                distance *= -1;
                offset *= -1;
            }

            for (int i = 0; i < (emptyLinesAbove > 0 ? emptyLinesAbove : emptyLinesBelow); i++)
            {
                var goLine = Instantiate(Resources.Load(StaticResource.PREFAB_EMPTY_NOTE_LINE)) as GameObject;
                goLine.transform.SetParent(transform);
                goLine.transform.localPosition = new Vector3(0, i * distance * 2 + offset, 0); // * 2 because the distance is for every line, visible or note
                goLine.transform.localScale *= transform.localScale.x;
            }
        }
    }

    public void InitializeNote(StaffLine parent, int emptyLinesBelow, int emptyLinesAbove)
    {
        InitializeNote(parent, emptyLinesBelow, emptyLinesAbove, Alteration.Natural);
    }

    /// <summary>
    /// Move the note from the current position to the parameter position
    /// </summary>
    /// <param name="position">end position</param>
    /// <param name="duration">duration of the move, default 2 seconds</param>
    /// /// <param name="ease">ease of the move, default OutSine</param>
    public void MoveTo(Vector2 position, float duration = 2f, Ease ease = Ease.Linear)
    {
        _movement = transform.DOMove(position, duration)
            .SetEase(ease)
            .OnStart(() => _isMoving = true)
            .OnComplete(() =>
            {
                _isMoving = false;
                Destroy();
            });
    }

    public void ChangeColor(Color color)
    {
        _sprtRenderer.color = color;
    }

    public void ShowGoodGuess()
    {
        ChangeColor(UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_GREEN));
        SpriteGoodGuess.gameObject.SetActive(true);
    }

    public void ShowBadGuess()
    {
        ChangeColor(UIHelper.GetColorFromHEX(StaticResource.COLOR_HEX_RED));
        SpriteBadGuess.gameObject.SetActive(true);
    }

    public void Destroy()
    {
        _movement.Kill();
        DestroyEvent?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public void SetInactive()
    {
        _isActive = false;
    }
}
