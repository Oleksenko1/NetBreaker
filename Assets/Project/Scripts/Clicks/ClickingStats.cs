using Cysharp.Threading.Tasks;
using System;

public class ClickingStats
{
    private bool saveLoaded = false;
    private BigNum bitsPerClick;
    private BigNum bitsPerSecond;
    private BigNum percentToAllBits;
    public ClickingStats()
    {
        bitsPerClick = new BigNum(1);
        bitsPerSecond = new BigNum(0);
        percentToAllBits = new BigNum(0);
    }
    public void AddBitsPerClick(BigNum x) => bitsPerClick += x;
    public void AddBitsPerSecond(BigNum x) => bitsPerSecond += x;
    public void AddPercentToAllBits(BigNum x) => percentToAllBits += x;
    public BigNum GetBitsPerClick()
    {
        // Calculating percent to all bits
        BigNum totalOutput = bitsPerClick * ((new BigNum(100) + percentToAllBits) / new BigNum(100));

        return totalOutput;
    }
    public BigNum GetBitsPerClickRaw() => bitsPerClick;
    public BigNum GetBitsPerSecond()
    {
        // Calculating percent to all bits
        BigNum totalOutput = bitsPerSecond * ((new BigNum(100) + percentToAllBits) / new BigNum(100));

        return totalOutput;
    }
    public BigNum GetBitsPerSecondRaw() => bitsPerSecond;
    public void MarkSaveAsLoaded()
    {
        saveLoaded = true;
    }
    public async UniTask WaitUntilSaveLoadedAsync()
    {
        await UniTask.WaitUntil(() => saveLoaded);
    }
}
