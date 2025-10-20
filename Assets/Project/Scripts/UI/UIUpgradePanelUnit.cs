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
        levelAmountTxt.SetText(0.ToString());
        priceTxt.SetText(upgradeSO.startingPrice + " <sprite=0>");
        iconImg.sprite = upgradeSO.sprite;
        purchaseBtn.onClick.AddListener(() => Debug.Log($"Buy btn of {upgradeSO.uniqueName} upgrade pressed"));
    }
}
