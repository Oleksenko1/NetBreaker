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
    public void Initialize(UpgradeSO upgradeSO, UpgradesManager upgradesManager)
    {
        this.upgradeSO = upgradeSO;
        this.upgradesManager = upgradesManager;

        upgradeNameTxt.SetText(upgradeSO.nameLabel);
        descriptionTxt.SetText(upgradeSO.description);
        iconImg.sprite = upgradeSO.sprite;
        purchaseBtn.onClick.AddListener(TryPurchasing);

        UpdateCostLevelInfo();
    }
    public void TryPurchasing()
    {
        bool success = upgradesManager.TryBuyingUpgrade(upgradeSO);

        if (success)
        {
            Debug.Log("Purchase successful");

            UpdateCostLevelInfo();
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }
    private void UpdateCostLevelInfo()
    {
        levelAmountTxt.SetText(upgradesManager.GetUpgradeLevel(upgradeSO).ToString());
        priceTxt.SetText(upgradesManager.GetUpgradeCost(upgradeSO) + " <sprite=0>");
    }
}
