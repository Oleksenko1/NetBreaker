using TMPro;
using UnityEngine;
using VContainer;

public class UIBitsBalance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI balanceTxt;
    [Inject] private BitsBalance bitsBalance;
    void Start()
    {
        balanceTxt.SetText(bitsBalance.GetCurrentBalance().ToString());

        EventBus.Subscribe<BitsGained_event>(OnBitsAdded);
    }
    private void OnBitsAdded(BitsGained_event e)
    {
        balanceTxt.SetText(e.balanceAmount.ToString());
    }
}
