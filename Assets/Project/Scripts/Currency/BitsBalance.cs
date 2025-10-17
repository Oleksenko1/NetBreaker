public class BitsBalance
{
    private BigNum currentBalance;
    public void AddBits(BigNum x)
    {
        if (x <= new BigNum(0)) return;

        currentBalance += x;
    }
    public void WithdrawBits(BigNum x)
    {
        if (x > currentBalance) return;

        currentBalance -= x;
    }
    public bool IsSumAvailable(BigNum x) => x <= currentBalance;
    public BigNum GetCurrentBalance() => currentBalance;
}
