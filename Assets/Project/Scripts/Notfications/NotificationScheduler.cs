using Unity.Notifications.Android;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
public class NotificationScheduler
{
    private const string CHANNEL_ID = "session_notifications";
    private const int NOTIFICATION_ID = 2001;
    private CancellationTokenSource _cancellationTokenSource;
    public NotificationScheduler()
    {
        // Start the notification scheduling loop
        StartNotificationLoop().Forget();
    }

    /// <summary>
    /// Continuously updates notification every 5 seconds to be sent in 4 hours
    /// </summary>
    private async UniTaskVoid StartNotificationLoop()
    {
        // Create new cancellation token
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Check if has a permission before scheduling
                if (HasNotificationPermission())
                {
                    ScheduleNotificationIn4Hours();
                }
                else
                {
                    Debug.Log("No notification permission - skipping schedule");
                }

                // Wait for 5 seconds
                await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: _cancellationTokenSource.Token);
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

    /// <summary>
    /// Schedules (or reschedules) a notification to be sent in 4 hours
    /// </summary>
    private void ScheduleNotificationIn4Hours()
    {
        // Cancel previous notification with same ID
        AndroidNotificationCenter.CancelScheduledNotification(NOTIFICATION_ID);

        // Create new notification
        var notification = new AndroidNotification
        {
            Title = "Max Offline Profit Reached!",
            Text = "Your servers are overflowing with cash! Time to log in and collect your loot.",
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            FireTime = DateTime.Now.AddHours(OfflineIncomeManager.MAX_HOURS_OFFLINE),
            ShowTimestamp = true
        };

        // Schedule notification
        AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);

        Debug.Log($"Notification scheduled for: {notification.FireTime}");

        // Test notification - sends in 5 minutes
        ScheduleTestNotification();
    }

    private void ScheduleTestNotification()
    {
        const int TEST_NOTIFICATION_ID = 9999; // Different ID so it doesn't conflict with main notification

        // Cancel previous test notification
        AndroidNotificationCenter.CancelScheduledNotification(TEST_NOTIFICATION_ID);

        // Create test notification
        var testNotification = new AndroidNotification
        {
            Title = "TEST: Notification Working!",
            Text = "This is a test notification sent 5 minutes after last session.",
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            FireTime = DateTime.Now.AddMinutes(5),
            ShowTimestamp = true
        };

        // Schedule test notification
        AndroidNotificationCenter.SendNotification(testNotification, CHANNEL_ID);

        Debug.Log($"TEST notification scheduled for: {testNotification.FireTime}");
    }

    /// <summary>
    /// Checks if notification permission is granted
    /// </summary>
    private bool HasNotificationPermission()
    {
        var status = AndroidNotificationCenter.UserPermissionToPost;
        return status == PermissionStatus.Allowed;
    }

    /// <summary>
    /// Stops the notification scheduling loop
    /// </summary>
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
        Debug.Log("All notifications cancelled");
    }

    /// <summary>
    /// Called when app goes to background - continue scheduling
    /// </summary>
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App going to background - schedule final notification
            if (HasNotificationPermission())
            {
                ScheduleNotificationIn4Hours();
            }
        }
        else
        {
            // App returning to foreground - cancel notifications
            CancelAllNotifications();
        }
    }
    void OnDestroy()
    {
        StopNotificationLoop();
    }
}