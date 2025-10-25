using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradePanelUnit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeNameTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private TextMeshProUGUI levelAmountTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Image iconImg;
    [SerializeField] private Button purchaseBtn;
    private UpgradeSO upgradeSO;
    private UpgradesManager upgradesManager;
    private BitsBalance bitsBalance;
    private SoundPlayer soundPlayer;
    private BigNum currentCost;
    private bool isAvailable;
    private bool IsAvailable
    {
        get => isAvailable;
        set
        {
            if (isAvailable == value) return;

            isAvailable = value;
            purchaseBtn.interactable = isAvailable;
        }
    }
    public void Initialize(UpgradeSO upgradeSO, UpgradesManager upgradesManager, BitsBalance bitsBalance, SoundPlayer soundPlayer)
    {
        this.upgradeSO = upgradeSO;
        this.upgradesManager = upgradesManager;
        this.bitsBalance = bitsBalance;
        this.soundPlayer = soundPlayer;

        EventBus.Subscribe<BitsGained_event>(OnBitsGained);
        EventBus.Subscribe<BitsSpent_event>(OnBitsSpent);

        upgradeNameTxt.SetText(upgradeSO.nameLabel);
        descriptionTxt.SetText(upgradeSO.description);
        iconImg.sprite = upgradeSO.sprite;
        purchaseBtn.onClick.AddListener(TryPurchasing);

        UpdateCostLevelInfo();

        // IsAvailable setter doesn't work on initialization :(
        purchaseBtn.interactable = isAvailable;
    }
    public void TryPurchasing()
    {
        bool success = upgradesManager.TryBuyingUpgrade(upgradeSO);

        if (success)
        {
            soundPlayer.PlayUI_SFX(SFXType.UIPurchaseUpgrade);

            PlayPurchaseAnim();

            UpdateCostLevelInfo();
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }
    private void PlayPurchaseAnim()
    {
        var rectTransform = iconImg.rectTransform;

        DOTween.Kill(rectTransform);

        rectTransform.localScale = Vector3.one;

        rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.4f);
        rectTransform.DOShakeRotation(0.3f, 20);
    }
    private void UpdateCostLevelInfo()
    {
        currentCost = upgradesManager.GetUpgradeCost(upgradeSO);
        int level = upgradesManager.GetUpgradeLevel(upgradeSO);

        levelAmountTxt.SetText(level.ToString());
        priceTxt.SetText(currentCost + " <sprite=0>");

        IsAvailable = bitsBalance.GetCurrentBalance() >= currentCost;
    }
    private void OnBitsSpent(BitsSpent_event e)
    {
        if (!IsAvailable) return;

        IsAvailable = bitsBalance.GetCurrentBalance() >= currentCost;
    }
    private void OnBitsGained(BitsGained_event e)
    {
        if (IsAvailable) return;

        IsAvailable = bitsBalance.GetCurrentBalance() >= currentCost;
    }
}
