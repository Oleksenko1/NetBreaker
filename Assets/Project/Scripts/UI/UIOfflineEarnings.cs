using System;
using DG.Tweening;
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
    [Inject] private SoundPlayer soundPlayer;
    private BigNum currentEarnings;
    private RectTransform claimButtonRT;
    void Awake()
    {
        gameObject.SetActive(false);

        claimButtonRT = claimButton.GetComponent<RectTransform>();

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

        claimButton.gameObject.SetActive(false);
        DOVirtual.DelayedCall(1f, ShowClaimButton);

        soundPlayer.PlayUI_SFX(SFXType.UIOpenPanelBtn);

        gameObject.SetActive(true);
    }
    private void ClaimEarnings()
    {
        if (currentEarnings == null) return;

        bitsBalance.AddBits(currentEarnings);
        currentEarnings = new BigNum(0);

        soundPlayer.PlayUI_SFX(SFXType.UICollect);

        gameObject.SetActive(false);
    }
    private void ShowClaimButton()
    {
        claimButtonRT.localScale = Vector3.zero;

        claimButtonRT.gameObject.SetActive(true);

        claimButtonRT.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutCubic);
    }
}
