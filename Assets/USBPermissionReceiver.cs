using UnityEngine;

public class USBPermissionReceiver : AndroidJavaProxy
{
    private LocationPermissionHandler permissionHandler;

    public USBPermissionReceiver(LocationPermissionHandler handler) : base("android.content.BroadcastReceiver")
    {
        permissionHandler = handler;
    }

    public void onReceive(AndroidJavaObject context, AndroidJavaObject intent)
    {
        Debug.Log("onReceive");
        string action = intent.Call<string>("getAction");

        if ("android.hardware.usb.action.USB_PERMISSION".Equals(action))
        {
            bool granted = intent.Call<bool>("getBooleanExtra", "permission", false);
            permissionHandler.OnUSBPermissionResult(granted);
        }
    }
}
