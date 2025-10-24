using UnityEngine;
using VContainer;

public class ClickingManager : MonoBehaviour
{
    [Inject] private BitsBalance bitsBalance;
    [Inject] private SoundPlayer soundPlayer;
    private ClickingStats clickingStats;
    void Awake()
    {
        if (clickingStats == null) clickingStats = new ClickingStats();

        EventBus.Subscribe<ClickPressed_event>(OnClickPressed);
    }
    private void OnClickPressed(ClickPressed_event e)
    {
        bitsBalance.AddBits(clickingStats.GetBitsPerClick());

        soundPlayer.PlayKeyboardSFX(2f);
    }
    public ClickingStats GetClickingStats()
    {
        if (clickingStats == null)
            clickingStats = new ClickingStats();

        return clickingStats;
    }
    [ContextMenu("Increase bits per second")]
    private void Increase()
    {
        clickingStats.AddBitsPerSecond(new BigNum(1));
    }
}
