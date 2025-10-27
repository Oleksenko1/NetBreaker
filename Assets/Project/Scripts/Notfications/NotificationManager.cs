using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject permissionRequestPanel;
    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button deniedBtn;
    private NotificationScheduler notificationScheduler;
    void Awake()
    {
        CreateNotificationChannel();

        notificationScheduler = new NotificationScheduler();
    }
    void Start()
    {
        permissionRequestPanel.SetActive(false);

        acceptBtn.onClick.AddListener(OnUserAcceptedExplanation);
        deniedBtn.onClick.AddListener(OnUserDeclinedExplanation);

        CheckAndRequestPermission();
    }
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
    void CheckAndRequestPermission()
    {
        var status = AndroidNotificationCenter.UserPermissionToPost;

        if (status == PermissionStatus.Allowed)
        {
            Debug.Log("Access is already allowed");
            return;
        }

        if (status == PermissionStatus.Denied)
        {
            // Check if we already showed our explanation
            if (PlayerPrefs.GetInt(PlayerPrefsValues.NotificationPermissionAsked.ToString(), 0) == 0)
            {
                ShowPermissionExplanation();
            }
            return;
        }

        if (status == PermissionStatus.NotRequested)
        {
            // Show explanation BEFORE system dialog
            ShowPermissionExplanation();
        }
    }
    void ShowPermissionExplanation()
    {
        if (permissionRequestPanel != null)
        {
            permissionRequestPanel.SetActive(true);
        }

        Debug.Log("Showing an explanation why we need notifications");
    }
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

        if (permissionRequestPanel != null)
        {
            permissionRequestPanel.SetActive(false);
        }
    }

    public void OnUserDeclinedExplanation()
    {
        // Mark that we asked for permission
        PlayerPrefs.SetInt(PlayerPrefsValues.NotificationPermissionAsked.ToString(), 1);
        PlayerPrefs.Save();

        if (permissionRequestPanel != null)
        {
            permissionRequestPanel.SetActive(false);
        }

        Debug.Log("User declined notifications");
    }

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

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            var status = AndroidNotificationCenter.UserPermissionToPost;
            Debug.Log($"Current access status: {status}");
        }
    }
}