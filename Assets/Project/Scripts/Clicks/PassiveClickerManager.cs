using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
public class PassiveClickerManager
{
    private readonly ClickingStats clickingStats;
    private readonly BitsBalance bitsBalance;
    [Inject]
    public PassiveClickerManager(ClickingManager clickingManager, BitsBalance bitsBalance)
    {
        clickingStats = clickingManager.GetClickingStats();
        this.bitsBalance = bitsBalance;

        StartLoop().Forget();
    }
    private async UniTaskVoid StartLoop()
    {
        while (true)
        {
            bitsBalance.AddBits(clickingStats.GetBitsPerSecond());

            await UniTask.Delay(1000);
        }
    }
}
