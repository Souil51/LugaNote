using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[ExecuteInEditMode]
public class Transition : MonoBehaviour
{
    public delegate void ClosedEventHandler(object sender, EventArgs e);
    public event ClosedEventHandler Closed;

    public delegate void OpenedEventHandler(object sender, EventArgs e);
    public event OpenedEventHandler Opened;

    [SerializeField] private float Duration;

    [SerializeField] private TransitionPosition InitialPosition;

    [SerializeField] private GameObject LeftPart;
    [SerializeField] private GameObject RightPart;

    [SerializeField] private float LeftPartYPosition_Open_1;
    [SerializeField] private float RightPartYPosition_Open_1;

    [SerializeField] private float LeftPartYPosition_Close;
    [SerializeField] private float RightPartYPosition_Close;

    [SerializeField] private float LeftPartYPosition_Open_2;
    [SerializeField] private float RightPartYPosition_Open_2;

    public TransitionPosition CurrentPosition => InitialPosition;

    private void Awake()
    {
        if(InitialPosition == TransitionPosition.Close)
            SetPositionClose();
        else if(InitialPosition == TransitionPosition.Open_1)
            SetPositionOpen_1();
        else
            SetPositionOpen_1();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPositionClose()
    {
        LeftPart.transform.localPosition = new Vector3(LeftPart.transform.localPosition.x, 0f, 0f);
        RightPart.transform.localPosition = new Vector3(RightPart.transform.localPosition.x, 0f, 0f);
    }

    public void SetPositionOpen_1()
    {
        LeftPart.transform.localPosition = new Vector3(LeftPart.transform.localPosition.x, -1f * LeftPartYPosition_Open_1, 0f);
        RightPart.transform.localPosition = new Vector3(RightPart.transform.localPosition.x, -1f * RightPartYPosition_Open_1, 0f);
    }
    public void SetPositionOpen_2()
    {
        LeftPart.transform.localPosition = new Vector3(LeftPart.transform.localPosition.x, -1f * LeftPartYPosition_Open_2, 0f);
        RightPart.transform.localPosition = new Vector3(RightPart.transform.localPosition.x, -1f * RightPartYPosition_Open_2, 0f);
    }

    public void Close()
    {
        LeftPart.transform.DOLocalMoveY(LeftPartYPosition_Close, Duration).SetUpdate(true).SetEase(Ease.InOutCubic);
        RightPart.transform.DOLocalMoveY(RightPartYPosition_Close, Duration).SetUpdate(true).SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                Closed?.Invoke(this, EventArgs.Empty);
            });
    }

    public void Open_1()
    {
        LeftPart.transform.DOLocalMoveY(-1f * LeftPartYPosition_Open_1, Duration).SetUpdate(true).SetEase(Ease.InOutCubic);
        RightPart.transform.DOLocalMoveY(-1f * RightPartYPosition_Open_1, Duration).SetUpdate(true).SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                Opened?.Invoke(this, EventArgs.Empty);
            });
    }

    public void Open_2()
    {
        LeftPart.transform.DOLocalMoveY(-1f * LeftPartYPosition_Open_2, Duration).SetUpdate(true).SetEase(Ease.InOutCubic);
        RightPart.transform.DOLocalMoveY(-1f * RightPartYPosition_Open_2, Duration).SetUpdate(true).SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                Opened?.Invoke(this, EventArgs.Empty);
            });
    }
}
