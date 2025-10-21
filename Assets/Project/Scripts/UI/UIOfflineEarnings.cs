using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIOfflineEarnings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeOfflineTxt;
    [SerializeField] private TextMeshProUGUI bitsEarnedTxt;
    [SerializeField] private Button claimButton;
    [Inject] private BitsBalance bitsBalance;
    private BigNum currentEarnings;
    void Awake()
    {
        gameObject.SetActive(false);

        EventBus.Subscribe<OfflineIncomeInvoke_event>(OnOfflineIncomeInvoke);
        claimButton.onClick.AddListener(ClaimEarnings);
    }
    private void OnOfflineIncomeInvoke(OfflineIncomeInvoke_event e)
    {
        TimeSpan offlineTimeSpan = TimeSpan.FromSeconds(e.offlineSeconds);
        string offlineTimeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}",
            offlineTimeSpan.Hours,
            offlineTimeSpan.Minutes,
            offlineTimeSpan.Seconds);

        currentEarnings = e.totalIncome;

        timeOfflineTxt.SetText(offlineTimeFormatted);
        bitsEarnedTxt.SetText($"{currentEarnings} <sprite=0>");

        gameObject.SetActive(true);
    }
    private void ClaimEarnings()
    {
        if (currentEarnings == null) return;

        bitsBalance.AddBits(currentEarnings);
        currentEarnings = new BigNum(0);

        gameObject.SetActive(false);
    }
}
