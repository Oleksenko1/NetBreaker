using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using VContainer;
public class BitsIncomeTracker : IDisposable
{
    private readonly ClickingStats clickingStats;
    private BitsIncomeTrackerUpdated_event trackerUpdated_Event;
    private float tickUpdateDelay = 0.5f; // In seconds
    private int clicksCount;
    private double clicksPerSecond;
    [Inject]
    public BitsIncomeTracker(ClickingManager clickingManager)
    {
        clickingStats = clickingManager.GetClickingStats();
        trackerUpdated_Event = new BitsIncomeTrackerUpdated_event();

        EventBus.Subscribe<ClickPressed_event>(OnClickPressed);

        StartTrackingLoop().Forget();
    }
    private void OnClickPressed(ClickPressed_event e)
    {
        clicksCount++;
    }
    private async UniTaskVoid StartTrackingLoop()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(tickUpdateDelay));

            clicksPerSecond = clicksCount / tickUpdateDelay;

            BigNum clickedBitsPerSecond = new BigNum(clicksPerSecond) * clickingStats.GetBitsPerClick();
            BigNum passiveBitsPerSecond = clickingStats.GetBitsPerSecond();

            BigNum totalBitsPerSecond = clickedBitsPerSecond + passiveBitsPerSecond;

            Debug.Log(totalBitsPerSecond.ToString());

            trackerUpdated_Event.bitsPerSecond = totalBitsPerSecond;
            EventBus.Publish(trackerUpdated_Event);

            clicksCount = 0;
        }
    }
    public void Dispose()
    {
        EventBus.Unsubscribe<ClickPressed_event>(OnClickPressed);
    }
}
