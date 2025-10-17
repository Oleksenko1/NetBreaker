public class ClickingStats
{
    public ClickingStats()
    {
        bitsPerClick = 1;
        bitsPerSecond = 0;
    }
    private int bitsPerClick;
    private int bitsPerSecond;
    public void AddBitsPerClick(int x) => bitsPerClick += x;
    public void AddBitsPerSecond(int x) => bitsPerSecond += x;
    public int GetBitsPerClick() => bitsPerClick;
    public int GetBitsPerSecond() => bitsPerSecond;
}
