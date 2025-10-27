using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject permissionRequestPanel;
    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button deniedBtn;
    void Awake()
    {
        CreateNotificationChannel();
    }
    void Start()
    {
        permissionRequestPanel.SetActive(false);

        acceptBtn.onClick.AddListener(OnUserAcceptedExplanation);
        deniedBtn.onClick.AddListener(OnUserDeclinedExplanation);

        CheckAndRequestPermission();

        SendImmediateTestNotification();
    }

    /// <summary>
    /// Creates a notification channel for Android. Required for Android 8.0+
    /// </summary>
    void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "session_notifications",
            Name = "Session Notifications",
            Importance = Importance.High,
            Description = "Reminder to return to the game",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    /// <summary>
    /// Checks current permission status and requests if needed
    /// </summary>
    void CheckAndRequestPermission()
    {
        var status = AndroidNotificationCenter.UserPermissionToPost;

        // Permission already granted - nothing to do
        if (status == PermissionStatus.Allowed)
        {
            Debug.Log("Access is already allowed");
            return;
        }

        // User previously denied permission
        if (status == PermissionStatus.Denied)
        {
            // Check if we already showed our explanation
            if (PlayerPrefs.GetInt(PlayerPrefsValues.NotificationPermissionAsked.ToString(), 0) == 0)
            {
                ShowPermissionExplanation();
            }
            return;
        }

        // Permission not yet requested
        if (status == PermissionStatus.NotRequested)
        {
            // Show explanation BEFORE system dialog
            ShowPermissionExplanation();
        }
    }

    /// <summary>
    /// Shows custom UI panel explaining why notifications are needed
    /// </summary>
    void ShowPermissionExplanation()
    {
        if (permissionRequestPanel != null)
        {
            permissionRequestPanel.SetActive(true);
        }

        Debug.Log("Showing an explanation why we need notifications");
    }

    /// <summary>
    /// Called when user clicks "Allow" button on custom UI panel
    /// </summary>
    public void OnUserAcceptedExplanation()
    {
        // Mark that we asked for permission
        PlayerPrefs.SetInt(PlayerPrefsValues.NotificationPermissionAsked.ToString(), 1);
        PlayerPrefs.Save();

        var status = AndroidNotificationCenter.UserPermissionToPost;

        // Trigger system permission dialog if not yet requested
        if (status == PermissionStatus.NotRequested)
        {
            RequestNotificationPermissionNative();
        }
        // Open app settings if permission was previously denied
        else if (status == PermissionStatus.Denied)
        {
            OpenAppSettings();
        }

        // Hide explanation panel
        if (permissionRequestPanel != null)
        {
            permissionRequestPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Called when user clicks "Not Now" button on custom UI panel
    /// </summary>
    public void OnUserDeclinedExplanation()
    {
        // Mark that we asked for permission
        PlayerPrefs.SetInt(PlayerPrefsValues.NotificationPermissionAsked.ToString(), 1);
        PlayerPrefs.Save();

        // Hide explanation panel
        if (permissionRequestPanel != null)
        {
            permissionRequestPanel.SetActive(false);
        }

        Debug.Log("User declined notifications");
    }

    /// <summary>
    /// Requests notification permission using native Android API (for Android 13+)
    /// </summary>
    void RequestNotificationPermissionNative()
    {
        try
        {
            Debug.Log("Requesting notification permission via native Android API");

#if UNITY_ANDROID
            // Get Android API level
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                int sdkInt = version.GetStatic<int>("SDK_INT");

                // Android 13+ (API 33+) requires POST_NOTIFICATIONS permission
                if (sdkInt >= 33)
                {
                    using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        // Request permission using ActivityCompat
                        string[] permissions = new string[] { "android.permission.POST_NOTIFICATIONS" };

                        currentActivity.Call("requestPermissions", permissions, 1001);

                        Debug.Log("Permission dialog should appear");
                    }
                }
                else
                {
                    Debug.Log("Android version below 13, permission not needed");
                }
            }
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to request permission: {e.Message}");
        }
    }

    /// <summary>
    /// Opens Android app settings where user can manually enable notifications
    /// </summary>
    void OpenAppSettings()
    {
        try
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var intent = new AndroidJavaObject("android.content.Intent"))
            {
                intent.Call<AndroidJavaObject>("setAction", "android.settings.APP_NOTIFICATION_SETTINGS");
                intent.Call<AndroidJavaObject>("putExtra", "android.provider.extra.APP_PACKAGE",
                    currentActivity.Call<string>("getPackageName"));

                currentActivity.Call("startActivity", intent);

                Debug.Log("Opening device settings");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to open settings: {e.Message}");
        }
    }

    /// <summary>
    /// Checks permission status when app returns to foreground
    /// Useful to detect if user enabled notifications in settings
    /// </summary>
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            var status = AndroidNotificationCenter.UserPermissionToPost;
            Debug.Log($"Current access status: {status}");
        }
    }
    /// <summary>
    /// Sends a test notification immediately (5 seconds after call)
    /// </summary>
    public void SendImmediateTestNotification()
    {
        // Check permission first
        var status = AndroidNotificationCenter.UserPermissionToPost;
        if (status != PermissionStatus.Allowed)
        {
            Debug.LogWarning("Cannot send test notification - permission not granted");
            return;
        }

        const int TEST_NOTIFICATION_ID = 9999;

        // Cancel any previous test notification
        AndroidNotificationCenter.CancelScheduledNotification(TEST_NOTIFICATION_ID);

        // Create test notification
        var testNotification = new AndroidNotification
        {
            Title = "TEST: Notification Working!",
            Text = "Your notification system is set up correctly!",
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            FireTime = System.DateTime.Now.AddSeconds(1),
            ShowTimestamp = true
        };

        // Schedule test notification
        AndroidNotificationCenter.SendNotification(testNotification, "session_notifications");

        Debug.Log($"TEST notification scheduled for: {testNotification.FireTime}");
    }
}