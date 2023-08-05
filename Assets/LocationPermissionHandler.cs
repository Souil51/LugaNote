using UnityEngine;

public class LocationPermissionHandler : MonoBehaviour
{
    private const string USB_PERMISSION = "android.permission.USB_PERMISSION";

    private AndroidJavaObject currentActivity;
    private USBPermissionReceiver usbPermissionReceiver;

    private void Start()
    {
        Debug.Log("Starting LocationPermissionHandler");
        currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");





        /*Debug.Log("Instantiating AndroidUtils");
        AndroidJavaObject androidUtils = new AndroidJavaObject("com.helagos.androidutilspermission.AndroidUtils");
        androidUtils.Call("requestPermission", "android.hardware.usb.action.USB_PERMISSION");
        Debug.Log("RequestPermission called");*/




        // Create an instance of the USBReceiver class
        AndroidJavaObject usbReceiverInstance = new AndroidJavaObject("com.helagos.androidutilspermission.USBReceiver");

        // Register the BroadcastReceiver to listen for USB permission result
        AndroidJavaObject filter = new AndroidJavaObject("android.content.IntentFilter", "android.hardware.usb.action.USB_PERMISSION");
        // currentActivity.Call("registerReceiver", usbReceiverInstance, filter);


        AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", "com.helagos.androidutilspermission.USB_PERMISSION_RESULT");
        USBPermissionReceiver usbPermissionReceiver = new USBPermissionReceiver(this);

        /*AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        Debug.Log("Get Application Context " + context);
        context.Call("registerReceiver", usbPermissionReceiver, intentFilter);*/
        //currentActivity.Call("registerReceiver", usbPermissionReceiver, intentFilter);
        /*AndroidJavaClass broadcastReceiverHelperClass = new AndroidJavaClass("com.helagos.androidutilspermission.BroadcastReceiverHelper");
        broadcastReceiverHelperClass.CallStatic("registerReceiver", currentActivity, usbPermissionReceiver, intentFilter);*/

        // Request USB permission
        RequestUSBPermission();
    }

    private void RequestUSBPermission()
    {
        Debug.Log("RequestUSBPermission call");

        AndroidJavaObject androidUtils = new AndroidJavaObject("com.helagos.androidutilspermission.USBPermissionManager");
        // androidUtils.Call("registerUSBReceiver");
        androidUtils.Call("requestUSBPermission");

        /*AndroidJavaObject usbManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "usb");

        AndroidJavaObject usbDeviceList = usbManager.Call<AndroidJavaObject>("getDeviceList");
        if (usbDeviceList != null && !usbDeviceList.Call<bool>("isEmpty"))
        {
            Debug.Log("usbDeviceList");
            AndroidJavaObject usbDeviceIterator = usbDeviceList.Call<AndroidJavaObject>("values").Call<AndroidJavaObject>("iterator"); ;
            while (usbDeviceIterator.Call<bool>("hasNext"))
            {
                AndroidJavaObject device = usbDeviceIterator.Call<AndroidJavaObject>("next");
                bool hasPermission = usbManager.Call<bool>("hasPermission", device);

                if (true || !hasPermission)
                {
                    Debug.Log("!hasPermission");
                    // Create a PendingIntent for USB permission request
                    // AndroidJavaObject permissionIntent = new AndroidJavaObject("android.app.PendingIntent", 0, new AndroidJavaObject("android.content.Intent", "android.hardware.usb.action.USB_PERMISSION"));
                    // AndroidJavaObject permissionIntent = new AndroidJavaObject("android.app.PendingIntent", currentActivity.Call<AndroidJavaObject>("getApplicationContext"), 0, new AndroidJavaObject("android.content.Intent"));
                    // AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                    // AndroidJavaObject permissionIntent = intent.CallStatic<AndroidJavaObject>("getBroadcast", currentActivity, 0, intent, 0);
                    AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.hardware.usb.action.USB_PERMISSION");
                    AndroidJavaObject pendingIntentClass = new AndroidJavaClass("android.app.PendingIntent");
                    AndroidJavaObject permissionIntent = pendingIntentClass.CallStatic<AndroidJavaObject>("getBroadcast", currentActivity, 0, intent, 0);

                    // Request USB permission
                    usbManager.Call("requestPermission", device, permissionIntent);
                }
            }
        }*/
    }

    public void OnUSBPermissionResult(bool granted)
    {
        if (granted)
        {
            Debug.Log("USB permission granted.");
            // Proceed with USB-related functionality.
        }
        else
        {
            Debug.Log("USB permission denied.");
            // Handle the situation when permission is denied.
        }
    }
}
