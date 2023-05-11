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

    public delegate void DestroyEventHandler(object sender, EventArgs args);
    public event DestroyEventHandler DestroyEvent;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeNote(StaffLine parent)
    {
        _parent = parent;
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
