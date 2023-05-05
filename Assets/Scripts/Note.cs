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

    public void MoveTo(Vector2 position)
    {
        transform.DOMove(position, 2f)
            .SetEase(Ease.OutSine)
            .OnStart(() => _isMoving = true)
            .OnComplete(() => _isMoving = false);
    }
}
