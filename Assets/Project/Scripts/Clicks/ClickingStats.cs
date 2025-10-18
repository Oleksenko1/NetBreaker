public class ClickingStats
{
    public ClickingStats()
    {
        bitsPerClick = new BigNum(1);
        bitsPerSecond = new BigNum(1);
    }
    private BigNum bitsPerClick;
    private BigNum bitsPerSecond;
    public void AddBitsPerClick(BigNum x) => bitsPerClick += x;
    public void AddBitsPerSecond(BigNum x) => bitsPerSecond += x;
    public BigNum GetBitsPerClick() => bitsPerClick;
    public BigNum GetBitsPerSecond() => bitsPerSecond;
}
