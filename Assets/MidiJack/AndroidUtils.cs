using MidiJack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static MidiJack.MidiDriver;

// This script have to be on a GameObject named AndroidUtils in the scene when Android Permission request occurs
// The Java android_utils.jar classes will send message to Unity for the USB permission result
public class AndroidUtils : MonoBehaviour
{
    public static UnityAction<string, string> OnAllowCallback = null;
    public static UnityAction<string, string> OnDenyCallback = null;

    //this function will be called when the permission has been approved
    public void OnAllow(string value)
    {
        Debug.Log("AndroidsUtils OnAllow " + value);

        string deviceName = "";
        string androidDeviceName = "";

        string[] split = value.Split(';');

        if(split.Length > 0)
            deviceName = split[0];
        if(split.Length > 1)
            androidDeviceName = split[1];

        if (OnAllowCallback != null)
            OnAllowCallback(deviceName, androidDeviceName);
    }

    //this function will be called when the permission has been denied
    public void OnDeny(string value)
    {
        Debug.Log("AndroidsUtils OnDeny " + value);

        string deviceName = "";
        string androidDeviceName = "";

        string[] split = value.Split(';');

        if (split.Length > 0)
            deviceName = split[0];
        if (split.Length > 1)
            androidDeviceName = split[1];
        
        if (OnDenyCallback != null)
            OnDenyCallback(deviceName, androidDeviceName);
    }
}