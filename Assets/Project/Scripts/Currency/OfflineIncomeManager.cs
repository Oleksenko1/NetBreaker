using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class OfflineIncomeManager
{
    private const int DELAY_TO_INVOKE = 5; // Delay from last session to current to activate offline income payout in SECONDS
    private const int MAX_HOURS_OFFLINE = 12;
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
        double difference = (e.currentSession - e.previousSession).TotalSeconds;
        difference = difference > maxSecondsOffline ? maxSecondsOffline : difference;

        if (difference > DELAY_TO_INVOKE)
        {
            await clickingStats.WaitUntilSaveLoadedAsync();

            // Returns if there is no bits per second
            if (clickingStats.GetBitsPerSecond() == new BigNum(0)) return;

            BigNum totalIncome = clickingStats.GetBitsPerSecond() * new BigNum(difference);

            offlineIncome_event.totalIncome = totalIncome;
            EventBus.Publish(offlineIncome_event);

            Debug.Log($"You were away for {difference} seconds and earned {totalIncome}");
        }
    }
}
