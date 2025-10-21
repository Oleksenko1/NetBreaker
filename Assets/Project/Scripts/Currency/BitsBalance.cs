using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.PlayerLoop;
using VContainer;

public class BitsBalance
{
    private BigNum currentBalance;
    private BitsGained_event bitsGained_Event = new();
    private BitsSpent_event bitsSpent_Event = new();
    private bool isInitialized = false;
    private UniTaskCompletionSource initCompletion = new();
    [Inject]
    public BitsBalance(SaveService saveService)
    {
        Initialize(saveService).Forget();
    }
    private async UniTask Initialize(SaveService saveService)
    {
        currentBalance = await saveService.GetSavedBalance();

        isInitialized = true;
        initCompletion.TrySetResult();
    }
    private async UniTask WaitUntilInitialized()
    {
        if (!isInitialized)
            await initCompletion.Task;
    }
    public void AddBits(BigNum x)
    {
        if (!isInitialized) return;

        if (x <= new BigNum(0)) return;

        currentBalance += x;

        bitsGained_Event.balanceAmount = currentBalance;
        EventBus.Publish(bitsGained_Event);
    }
    public void WithdrawBits(BigNum x)
    {
        if (!isInitialized) return;

        if (x > currentBalance) return;

        currentBalance -= x;

        bitsSpent_Event.balanceAmount = currentBalance;
        EventBus.Publish(bitsSpent_Event);
    }
    public bool IsSumAvailable(BigNum x)
    {
        if (!isInitialized) return false;
        return x <= currentBalance;
    }
    public BigNum GetCurrentBalance()
    {
        if (!isInitialized) return new BigNum(0);
        return currentBalance;
    }
    public async UniTask<BigNum> GetCurrentBalanceAsync()
    {
        await WaitUntilInitialized();
        return currentBalance;
    }
}
