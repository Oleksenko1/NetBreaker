using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class OfflineIncomeManager
{
    private const int SECONDS_TO_INVOKE = 30; // Delay from last session to current to activate offline income payout in SECONDS
    public const int MAX_HOURS_OFFLINE = 4;
    private OfflineIncomeInvoke_event offlineIncome_event = new();
    private ClickingStats clickingStats;
    private double maxSecondsOffline;
    [Inject]
    public OfflineIncomeManager(ClickingManager clickingManager)
    {
        clickingStats = clickingManager.GetClickingStats();

        maxSecondsOffline = MAX_HOURS_OFFLINE * 60 * 60;

        EventBus.Subscribe<LastSessionSaved_event>(OnSessionSaved);
    }
    private async void OnSessionSaved(LastSessionSaved_event e)
    {
        await OnSessionSavedAsync(e);
    }
    private async UniTask OnSessionSavedAsync(LastSessionSaved_event e)
    {
        // Difference in seconds
        double difference = (e.currentSession - e.previousSession).TotalSeconds;
        difference = difference > maxSecondsOffline ? maxSecondsOffline : difference;

        if (difference > SECONDS_TO_INVOKE)
        {
            await clickingStats.WaitUntilSaveLoadedAsync();

            // Returns if there is no bits per second
            if (clickingStats.GetBitsPerSecond() == new BigNum(0)) return;

            BigNum totalIncome = clickingStats.GetBitsPerSecond() * new BigNum(difference) / new BigNum(2); // Dividing offline income by 2

            offlineIncome_event.totalIncome = totalIncome;
            offlineIncome_event.offlineSeconds = difference;
            EventBus.Publish(offlineIncome_event);
        }
    }
}
