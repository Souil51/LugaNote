using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;

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

    /// <summary>
    /// Move the note from the current position to the parameter position
    /// </summary>
    /// <param name="position">end position</param>
    /// <param name="duration">duration of the move, default 2 seconds</param>
    /// /// <param name="ease">ease of the move, default OutSine</param>
    public void MoveTo(Vector2 position, float duration = 2f, Ease ease = Ease.OutSine)
    {
        transform.DOMove(position, duration)
            .SetEase(ease)
            .OnStart(() => _isMoving = true)
            .OnComplete(() => _isMoving = false);
    }
}
