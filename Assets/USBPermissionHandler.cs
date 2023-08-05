using UnityEngine;

public class USBPermissionHandler : MonoBehaviour
{
    private static AndroidJavaObject androidUtils = null;

    private void Start()
    {
        Debug.Log("Starting LocationPermissionHandler");
        // RequestUSBPermission();
    }

    private void OnDestroy()
    {
        if(androidUtils != null)
            androidUtils.Call("unregisterUSBReceiver");
    }

    public static void RequestUSBPermission(string deviceName)
    {
        Debug.Log("RequestUSBPermission call for device " + deviceName);

        if(androidUtils == null)
            androidUtils = new AndroidJavaObject("com.helagos.androidutilspermission.USBPermissionManager");

        // androidUtils.Call("registerUSBReceiver"); // called in requestUSBPermission the first time
        androidUtils.Call("requestUSBPermission", deviceName);
    }
}
