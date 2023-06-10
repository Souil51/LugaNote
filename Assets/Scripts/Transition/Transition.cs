using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    public GameObject LeftPart;
    public GameObject RightPart;

    public float LeftPartYStarting;
    public float RightPartYStarting;

    private void Awake()
    {
        LeftPart.transform.localPosition = new Vector3(0, LeftPartYStarting, 0);
        RightPart.transform.localPosition = new Vector3(0, RightPartYStarting, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        //LeftPart.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.InOutCubic);
        //RightPart.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.InOutCubic);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LeftPart.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.InOutCubic);
            RightPart.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.InOutCubic);

            // LeftPart.transform.DOLocalMoveY(-1f * LeftPartYStarting, 1f).SetEase(Ease.InOutCubic);
            // RightPart.transform.DOLocalMoveY(-1f * RightPartYStarting, 1f).SetEase(Ease.InOutCubic);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LeftPart.transform.DOLocalMoveY(-1f * LeftPartYStarting, 1f).SetEase(Ease.InOutCubic);
            RightPart.transform.DOLocalMoveY(-1f * RightPartYStarting, 1f).SetEase(Ease.InOutCubic);
        }
    }
}
