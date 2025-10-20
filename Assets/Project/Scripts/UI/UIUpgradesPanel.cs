using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIUpgradesPanel : MonoBehaviour
{
    [SerializeField] private Transform contentTR;
    [SerializeField] private Transform upgradeUnitPf;
    [Header("Buttons")]
    [SerializeField] private Button closeBtn;
    [Inject] private UpgradesManager upgradesManager;
    private UpgradesListSO upgradesListSO;
    private Dictionary<UpgradeSO, UIUpgradePanelUnit> upgradePanelUnits = new Dictionary<UpgradeSO, UIUpgradePanelUnit>();
    void Start()
    {
        closeBtn.onClick.AddListener(ClosePanel);

        ClosePanel();

        InitializeAsync().Forget();
    }
    private async UniTaskVoid InitializeAsync()
    {
        // Loading upgrades list
        upgradesListSO = await upgradesManager.GetUpgradesListAsync();

        foreach (UpgradeSO upgradeSO in upgradesListSO.list)
        {
            var upgradeUnit = Instantiate(upgradeUnitPf, contentTR).GetComponent<UIUpgradePanelUnit>();
            upgradeUnit.Initialize(upgradeSO, upgradesManager);

            upgradePanelUnits.Add(upgradeSO, upgradeUnit);
        }
    }
    public void OpenPanel() => gameObject.SetActive(true);
    public void ClosePanel() => gameObject.SetActive(false);
}
