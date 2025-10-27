using Unity.Notifications.Android;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class NotificationScheduler : MonoBehaviour
{
    private const string CHANNEL_ID = "session_notifications";
    private const int NOTIFICATION_ID = 2001;
    private CancellationTokenSource _cancellationTokenSource;

    void Start()
    {
        StartNotificationLoop().Forget();
    }

    private async UniTaskVoid StartNotificationLoop()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (HasNotificationPermission())
                {
                    ScheduleNotificationInMaxHours();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Notification scheduling loop cancelled");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in notification loop: {e.Message}");
        }
    }

    private void ScheduleNotificationInMaxHours()
    {
        AndroidNotificationCenter.CancelScheduledNotification(NOTIFICATION_ID);

        var notification = new AndroidNotification
        {
            Title = "Max Offline Profit Reached!",
            Text = "Your servers are overflowing with cash! Time to log in and collect your loot.",
            FireTime = DateTime.Now.AddHours(OfflineIncomeManager.MAX_HOURS_OFFLINE),
            ShowTimestamp = true
        };

        AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);

        Debug.Log($"Main notification scheduled for: {notification.FireTime}");
    }

    private void ScheduleTestNotification()
    {
        const int TEST_NOTIFICATION_ID = 9999;

        AndroidNotificationCenter.CancelScheduledNotification(TEST_NOTIFICATION_ID);

        var testNotification = new AndroidNotification
        {
            Title = "TEST: Notification Working!",
            Text = "This is a test notification sent 1 minute after last session.",
            FireTime = DateTime.Now.AddMinutes(1),
            ShowTimestamp = true
        };

        AndroidNotificationCenter.SendNotification(testNotification, CHANNEL_ID);

        Debug.Log($"TEST notification scheduled for: {testNotification.FireTime}");
    }

    private bool HasNotificationPermission()
    {
        var status = AndroidNotificationCenter.UserPermissionToPost;
        return status == PermissionStatus.Allowed;
    }

    public void StopNotificationLoop()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        Debug.Log("Notification loop stopped");
    }

    public void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelScheduledNotification(NOTIFICATION_ID);
        AndroidNotificationCenter.CancelScheduledNotification(9999);
        Debug.Log("All notifications cancelled");
    }

    // Handle app going to background/foreground
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App going to background - schedule final notification and STOP loop
            Debug.Log("App paused - scheduling final notification");

            if (HasNotificationPermission())
            {
                ScheduleNotificationInMaxHours();
                ScheduleTestNotification(); // ADD test notification
            }

            StopNotificationLoop(); // Stop the loop when app goes to background
        }
        else
        {
            Debug.Log("App resumed - cancelling notifications and restarting loop");

            CancelAllNotifications();

            StartNotificationLoop().Forget();
        }
    }
    void OnDestroy()
    {
        StopNotificationLoop();
    }
}