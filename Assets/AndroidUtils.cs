using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AndroidUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //They will be called when unity receive callback from Android 
    public static UnityAction onAllowCallback;
    public static UnityAction onDenyCallback;
    public static UnityAction onDenyAndNeverAskAgainCallback;

   /*public static bool IsPermitted(AndroidPermission permission)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var androidUtils = new AndroidJavaClass("com.helagos.androidutilspermission.AndroidUtils"))
        {
            return androidUtils.GetStatic<AndroidJavaObject>("currentActivity").Call<bool>("hasPermission", GetPermissionStrr(permission));
        }
#endif
        return true;
    }*/
    /*public static void RequestPermission(AndroidPermission permission, UnityAction onAllow = null, UnityAction onDeny = null, UnityAction onDenyAndNeverAskAgain = null)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        onAllowCallback = onAllow;
        onDenyCallback = onDeny;
        onDenyAndNeverAskAgainCallback = onDenyAndNeverAskAgain;
        using (var androidUtils = new AndroidJavaClass("com.helagos.androidutilspermission.AndroidUtils"))
        {
            androidUtils.GetStatic<AndroidJavaObject>("currentActivity").Call("requestPermission", GetPermissionStrr(permission));
        }
#endif
    }*/
    /*private static string GetPermissionStrr(AndroidPermission permission)
    {
        return "android.permission." + permission.ToString();
    }*/
    /*public static void ShowToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic("currentActivity");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            new AndroidJavaClass("android.widget.Toast").CallStatic("makeText", currentActivity.Call("getApplicationContext"), new AndroidJavaObject("java.lang.String", message), 0).Call("show");
        }));
#endif
    }*/

    //this function will be called when the permission has been approved
    public void OnAllow(string value)
    {
        Debug.Log("AndroidsUtils OnAllow " + value);
        if (onAllowCallback != null)
            onAllowCallback();
        ResetAllCallBacks();
    }
    //this function will be called when the permission has been denied
    public void OnDeny(string value)
    {
        Debug.Log("AndroidsUtils OnDeny " + value);
        if (onDenyCallback != null)
            onDenyCallback();
        ResetAllCallBacks();
    }
    //this function will be called when the permission has been denied and user tick to checkbox never ask again
    public void OnDenyAndNeverAskAgain(string value)
    {
        Debug.Log("AndroidsUtils OnDenyAndNeverAskAgain " + value);
        if (onDenyAndNeverAskAgainCallback != null)
            onDenyAndNeverAskAgainCallback();
        ResetAllCallBacks();
    }
    private void ResetAllCallBacks()
    {
        onAllowCallback = null;
        onDenyCallback = null;
        onDenyAndNeverAskAgainCallback = null;
    }
}

public enum AndroidPermission
{
    ACCESS_COARSE_LOCATION,
    ACCESS_FINE_LOCATION,
    ADD_VOICEMAIL,
    BODY_SENSORS,
    CALL_PHONE,
    CAMERA,
    GET_ACCOUNTS,
    PROCESS_OUTGOING_CALLS,
    READ_CALENDAR,
    READ_CALL_LOG,
    READ_CONTACTS,
    READ_EXTERNAL_STORAGE,
    READ_PHONE_STATE,
    READ_SMS,
    RECEIVE_MMS,
    RECEIVE_SMS,
    RECEIVE_WAP_PUSH,
    RECORD_AUDIO,
    SEND_SMS,
    USE_SIP,
    WRITE_CALENDAR,
    WRITE_CALL_LOG,
    WRITE_CONTACTS,
    WRITE_EXTERNAL_STORAGE
}
