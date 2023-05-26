using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class Note : MonoBehaviour
{
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;

    private StaffLine _parent;
    public StaffLine Parent => _parent;

    private bool _isActive = true;
    public bool IsActive => _isActive;

    private SpriteRenderer _sprtRenderer;

    private Tween _movement;

    private long _creationTimestamp;
    public long CreationTimestamp => _creationTimestamp;

    public delegate void DestroyEventHandler(object sender, EventArgs args);
    public event DestroyEventHandler DestroyEvent;

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

    public void InitializeNote(StaffLine parent, int emptyLinesBelow, int emptyLinesAbove)
    {
        _creationTimestamp = DateTime.Now.Ticks;
        _parent = parent;

        // The staff display only 5 lines.
        // For lower or higher notes than that empty lines have to be displayed for better accuracy
        if(emptyLinesAbove > 0 || emptyLinesBelow > 0)
        {
            float distance = Parent.Parent.LineDistance / transform.localScale.x;
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
