public class BitsBalance
{
    private int currentBalance;
    public void AddBits(int x)
    {
        if (x <= 0) return;

        currentBalance += x;
    }
    public void WithdrawBits(int x)
    {
        if (x > currentBalance) return;

        currentBalance -= x;
    }
    public bool IsSumAvailable(int x) => x <= currentBalance;
    public int GetCurrentBalance() => currentBalance;
}
