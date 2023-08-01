import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbManager;
import android.os.Bundle;
import android.app.PendingIntent;

public class USBPermissionRequest extends Activity {
    private static final int USB_PERMISSION_REQUEST_CODE = 1;
    private static final String ACTION_USB_PERMISSION = "com.your.package.USB_PERMISSION";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Get the USB device passed as an extra to this activity
        UsbDevice usbDevice = getIntent().getParcelableExtra(UsbManager.EXTRA_DEVICE);

        if (usbDevice != null) {
            // Request USB permission
            UsbManager usbManager = (UsbManager) getSystemService(Context.USB_SERVICE);
            if (usbManager != null) {
                PendingIntent permissionIntent = PendingIntent.getBroadcast(this, 0, new Intent(ACTION_USB_PERMISSION), 0);
                usbManager.requestPermission(usbDevice, permissionIntent);
            }
        }

        // Close the activity immediately after requesting permission
        finish();
    }
}
