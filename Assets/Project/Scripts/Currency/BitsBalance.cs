public class BitsBalance
{
    private BigNum currentBalance;
    private BitsGained_event bitsGained_Event = new();
    public void AddBits(BigNum x)
    {
        if (x <= new BigNum(0)) return;

        currentBalance += x;

        bitsGained_Event.balanceAmount = currentBalance;
        EventBus.Publish(bitsGained_Event);
    }
    public void WithdrawBits(BigNum x)
    {
        if (x > currentBalance) return;

        currentBalance -= x;
    }
    public bool IsSumAvailable(BigNum x) => x <= currentBalance;
    public BigNum GetCurrentBalance() => currentBalance;
}
