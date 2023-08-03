using MidiJack;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class NoteIndicatorGroup_old : MonoBehaviour
{
    public GameObject prefab;
    public TextMeshProUGUI text;

    void Start()
    {
        for (var i = 0; i < 128; i++)
        {
            var go = Instantiate<GameObject>(prefab);
            go.transform.position = new Vector3(i % 12, i / 12, 0);
            go.GetComponent<NoteIndicator>().noteNumber = i;

#if UNITY_ANDROID && !UNITY_EDITOR
            text.text = "ANDROID";
#endif
        }

        MidiMaster.noteOnDelegate += NoteOn;

        // Permission.RequestUserPermission() += HandlePermissionResult;
        // Register a BroadcastReceiver to listen for USB device attachment
        /*AndroidJavaObject filter = new AndroidJavaObject("android.content.IntentFilter", "android.hardware.usb.action.USB_DEVICE_ATTACHED");
        USBReceiver receiver = new USBReceiver(this);
        AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        context.Call("registerReceiver", receiver, filter);*/
    }

    void NoteOn(MidiChannel channel, int note, float velocity)
    {
        AskPermission();
    }

    private void AskPermission()
    {
#if UNITY_ANDROID
        Debug.Log("ASKING FOR USB PERMISSION");
        // CheckForUSBPermission();
        // TestRequestPermission();

        RequestFineLocationPermission();
#endif
    }

    private void HandlePermissionResult(string permission, bool granted)
    {
        if (permission == FINE_LOCATION_PERMISSION)
        {
            if (granted)
            {
                Debug.Log("Fine Location permission granted.");
                // You can now proceed with location-related functionality.
            }
            else
            {
                Debug.Log("Fine Location permission denied.");
                // Handle the situation when permission is denied.
            }
        }
    }

    private const string FINE_LOCATION_PERMISSION = "android.permission.ACCESS_FINE_LOCATION";

    public void RequestFineLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(FINE_LOCATION_PERMISSION))
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission(FINE_LOCATION_PERMISSION);
        }
        else
        {
            Debug.Log("Aleardy granted");
        }
    }

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
    }

    /*private const string USB_PERMISSION = "android.permission.USB_PERMISSION";

    public void CheckForUSBPermission()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Android platform");
            // Get the USB manager instance
            AndroidJavaObject usbManager = new AndroidJavaObject("android.hardware.usb.UsbManager");

            // Get the list of connected USB devices
            AndroidJavaObject usbDeviceList = usbManager.Call<AndroidJavaObject>("getDeviceList");

            // Check if any devices are connected
            if (usbDeviceList != null && usbDeviceList.Call<bool>("isEmpty") == false)
            {
                Debug.Log("usbDeviceList is not null and not empty");
                // Create a list of USB devices
                List<AndroidJavaObject> deviceList = new List<AndroidJavaObject>();
                AndroidJavaObject usbDeviceIterator = usbDeviceList.Call<AndroidJavaObject>("values");
                while (usbDeviceIterator.Call<bool>("hasNext"))
                {
                    Debug.Log("USB Iterator");
                    AndroidJavaObject device = usbDeviceIterator.Call<AndroidJavaObject>("next");
                    deviceList.Add(device);
                }

                // Request permission for each connected USB device
                foreach (AndroidJavaObject device in deviceList)
                {
                    Debug.Log("Device loop");
                    string deviceString = device.Call<string>("toString");
                    Debug.Log("USB Device: " + deviceString);

                    // Check if permission is already granted
                    if (!usbManager.Call<bool>("hasPermission", device))
                    {
                        // Create a pending intent to request USB permission
                        AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                            .GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject permissionIntent = new AndroidJavaObject("android.app.PendingIntent",
                            0, new AndroidJavaObject("android.content.Intent"));

                        // Request permission
                        usbManager.Call("requestPermission", device, permissionIntent);
                    }
                }
            }
        }
    }*/

    /*private const string USB_PERMISSION = "android.permission.USB_PERMISSION";

    private bool hasRequestedPermission = false;

    public void RequestUSBPermission(AndroidJavaObject device)
    {
        // Create a pending intent to request USB permission
        AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject permissionIntent = new AndroidJavaObject("android.app.PendingIntent",
            0, new AndroidJavaObject("android.content.Intent"));

        // Request permission
        AndroidJavaObject usbManager = context.Call<AndroidJavaObject>("getSystemService", "usb");
        usbManager.Call("requestPermission", device, permissionIntent);
    }*/

    public void TestRequestPermission()
    {
        if (!AndroidUtils.IsPermitted(AndroidPermission.ACCESS_FINE_LOCATION))//test request permission
            AndroidUtils.RequestPermission(
            AndroidPermission.ACCESS_FINE_LOCATION,
            () => { },
            () => { },
            () => { }
            );
    }
}

/*public class USBReceiver : AndroidJavaProxy
{
    private NoteIndicatorGroup usbPermissionHandler;

    public USBReceiver(NoteIndicatorGroup handler) : base("android.content.BroadcastReceiver")
    {
        usbPermissionHandler = handler;
    }

    public void onReceive(AndroidJavaObject context, AndroidJavaObject intent)
    {
        Debug.Log("onReceive");
        string action = intent.Call<string>("getAction");

        if ("android.hardware.usb.action.USB_DEVICE_ATTACHED".Equals(action))
        {
            AndroidJavaObject usbDevice = intent.Call<AndroidJavaObject>("getParcelableExtra", "android.hardware.usb.extra.USB_DEVICE");
            if (usbDevice != null)
            {
                usbPermissionHandler.RequestUSBPermission(usbDevice);
            }
        }
    }
}*/
