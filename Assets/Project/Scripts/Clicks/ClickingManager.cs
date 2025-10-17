using UnityEngine;
using VContainer;

public class ClickingManager : MonoBehaviour
{
    [Inject] private BitsBalance bitsBalance;
    private ClickingStats clickingStats;
    void Awake()
    {
        clickingStats = new ClickingStats();

        EventBus.Subscribe<ClickPressed_event>(OnClickPressed);
    }
    private void OnClickPressed(ClickPressed_event e)
    {
        bitsBalance.AddBits(clickingStats.GetBitsPerClick());

        Debug.Log("Balance: " + bitsBalance.GetCurrentBalance().ToString());
    }
    public ClickingStats GetClickingStats() => clickingStats;
}
