using Cysharp.Threading.Tasks;
using System;

public class ClickingStats
{
    private bool saveLoaded = false;
    private BigNum bitsPerClick;
    private BigNum bitsPerSecond;
    public ClickingStats()
    {
        bitsPerClick = new BigNum(1);
        bitsPerSecond = new BigNum(0);
    }
    public void AddBitsPerClick(BigNum x) => bitsPerClick += x;
    public void AddBitsPerSecond(BigNum x) => bitsPerSecond += x;
    public BigNum GetBitsPerClick() => bitsPerClick;
    public BigNum GetBitsPerSecond() => bitsPerSecond;
    public void MarkSaveAsLoaded()
    {
        saveLoaded = true;
    }
    public async UniTask WaitUntilSaveLoadedAsync()
    {
        await UniTask.WaitUntil(() => saveLoaded);
    }
}
