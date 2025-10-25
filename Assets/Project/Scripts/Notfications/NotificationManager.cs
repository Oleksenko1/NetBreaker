using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject permissionRequestPanel;
    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button deniedBtn;
    void Start()
    {
        acceptBtn.onClick.AddListener(OnUserAcceptedExplanation);
        deniedBtn.onClick.AddListener(OnUserDeclinedExplanation);

        CreateNotificationChannel();
        CheckAndRequestPermission();
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
            TriggerSystemPermissionRequest();
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
    /// Triggers Android system permission dialog by sending a test notification
    /// </summary>
    void TriggerSystemPermissionRequest()
    {
        // Send test notification to trigger system dialog
        var notification = new AndroidNotification
        {
            Title = "Test",
            Text = "Checking for the access",
            SmallIcon = "icon_0",
            FireTime = System.DateTime.Now.AddSeconds(1)
        };

        AndroidNotificationCenter.SendNotification(notification, "session_notifications");

        Debug.Log("System dialog should appear");
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
}