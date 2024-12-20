using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public delegate void ValueChangedEventHandler(object sender, GenericEventArgs<bool> e);
    public event ValueChangedEventHandler ValueChanged;

    [SerializeField] Image Frame;
    [SerializeField] Image BGFading;
    [SerializeField] GameObject SlideButton;
    [SerializeField] float LerpDuration;
    [SerializeField] float LeftMargin;
    [SerializeField] float RightMargin;
    [SerializeField] float ActionDistance;

    private RectTransform FrameRect;
    private RectTransform btRect;

    private (float, float) FrameXBoundary;
    private Vector2 touchPos;

    public int SwitchState { get; private set; } = 0;
    public bool Value => SwitchState == 1;

    private int ButtonSwitchState = 0;
    private float timeElapsed = 0;


    public void InitialState(int _switchState)
    {
        if (_switchState == 1)
        {
            SwitchOn();
        }
        else
        {
            SwitchOff();
        }
    }

    public void InitialState(bool value)
    {
        InitialState(value ? 1 : 0);
    }

    void Awake()
    {
        FrameRect = (RectTransform)Frame.transform;
        btRect = (RectTransform)SlideButton.transform;

        FrameXBoundary = (Frame.transform.localPosition.x - (FrameRect.rect.width / 2),
        Frame.transform.localPosition.x + (FrameRect.rect.width / 2));

        timeElapsed = LerpDuration;
    }

    void SwitchOn()
    {
        // Debug.Log("Switch ON");
        // Debug.Log(SwitchState);
        // Debug.Log(ButtonSwitchState);
        if (SwitchState == 0)
        {
            SwitchState = 1;
            timeElapsed = 0;
            ValueChanged?.Invoke(this, new GenericEventArgs<bool>(true));
        }
    }

    void SwitchOff()
    {
        // Debug.Log("Switch OFF");
        // Debug.Log(SwitchState);
        // Debug.Log(ButtonSwitchState);
        if (SwitchState == 1)
        {
            SwitchState = 0;
            timeElapsed = LerpDuration;
            ValueChanged?.Invoke(this, new GenericEventArgs<bool>(false));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (!pointerDown)
        //{
        //    if (SwitchState == 1) // switch off
        //    {
        //        SwitchState = 0;
        //    }
        //    else // switch on
        //    {
        //        SwitchState = 1;
        //    }
        //}
    }
    // OnPointerDown and OnPointerUp serves as rescaling slide button
    public void OnPointerDown(PointerEventData eventData)
    {
        //if (Input.touchCount > 0)
        //{
        //    touchPos = Input.GetTouch(0).position;
        //    pointerDown = true;
        //}
        //else if(Input.GetMouseButtonDown(0))
        //{
        //    touchPos = Input.mousePosition;
        //    pointerDown = true;
        //}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //pointerDown = false;
    }

    void Update()
    {
        //if (pointerDown)
        //{
        //    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        //    {
        //        Vector2 TempPos = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        //        if (TempPos.x - touchPos.x >= ActionDistance)
        //        {
        //            if (SwitchState != 1)
        //            {
        //                SwitchOn();
        //            }
        //        }

        //        if (TempPos.x - touchPos.x <= -ActionDistance)
        //        {
        //            if (SwitchState != 0)
        //            {
        //                SwitchOff();
        //            }
        //        }
        //    }
        //}

        if (SwitchState != ButtonSwitchState)
        {
            SlideMotion(SwitchState);
        }
    }
    public void Click()
    {
        if (SwitchState != 1)
        {
            SwitchOn();
        } 
        else
        {
            SwitchOff();
        }
    }

    // timeElapsed serves as position(percentage position) of button and also a time in slide duration
    void SlideMotion(int _btstate)
    {
        // motion start
        // set button pos state mide state

        if (_btstate == 1) // on state, 1
        {
            ButtonSwitchState = -1;
            SlideButton.transform.localPosition = Vector3.Lerp(
                new Vector3(FrameXBoundary.Item1 + (btRect.rect.width / 2) + LeftMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z), 
                new Vector3(FrameXBoundary.Item2 - (btRect.rect.width / 2) - RightMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z), 
                timeElapsed / LerpDuration);
            BGFading.color = new Color(BGFading.color.r, BGFading.color.g, BGFading.color.b, timeElapsed);
            timeElapsed += Time.unscaledDeltaTime;
            // Debug.Log(timeElapsed + " | " + Time.unscaledDeltaTime);
            if (timeElapsed / LerpDuration >= 1) // if reach button state 1
            {
                ButtonSwitchState = 1;
                // button margin re assigning, in case of delta time inaccurate, see differemce of third parameter of Learp function
                SlideButton.transform.localPosition = Vector3.Lerp(
                    new Vector3(FrameXBoundary.Item1 + (btRect.rect.width / 2) + LeftMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z), 
                    new Vector3(FrameXBoundary.Item2 - (btRect.rect.width / 2) - RightMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z), 
                    1);
                BGFading.color = new Color(BGFading.color.r, BGFading.color.g, BGFading.color.b, 1);
            }
        }
        else // off state, 0
        {
            ButtonSwitchState = -2;
            SlideButton.transform.localPosition = Vector3.Lerp(
                new Vector3(FrameXBoundary.Item1 + (btRect.rect.width / 2) + LeftMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z),
                new Vector3(FrameXBoundary.Item2 - (btRect.rect.width / 2) - RightMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z),
                timeElapsed / LerpDuration);
            BGFading.color = new Color(BGFading.color.r, BGFading.color.g, BGFading.color.b, timeElapsed / LerpDuration);
            timeElapsed -= Time.unscaledDeltaTime;
            // Debug.Log(timeElapsed + " | " + Time.unscaledDeltaTime);
            if (timeElapsed / LerpDuration <= 0) // if reach 0 button state
            {
                ButtonSwitchState = 0;

                SlideButton.transform.localPosition = Vector3.Lerp(
                    new Vector3(FrameXBoundary.Item1 + (btRect.rect.width / 2) + LeftMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z),
                    new Vector3(FrameXBoundary.Item2 - (btRect.rect.width / 2) - RightMargin, SlideButton.transform.localPosition.y, SlideButton.transform.localPosition.z),
                    0);
                BGFading.color = new Color(BGFading.color.r, BGFading.color.g, BGFading.color.b, 0);
            }
        }
    }

}