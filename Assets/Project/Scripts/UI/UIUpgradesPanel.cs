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
    void Start()
    {
        InitializeAsync().Forget();

        closeBtn.onClick.AddListener(ClosePanel);

        ClosePanel();
    }
    private async UniTaskVoid InitializeAsync()
    {
        upgradesListSO = await upgradesManager.GetUpgradesListAsync();

        Debug.Log("Initialization complete with upgrades list");
    }
    public void OpenPanel() => gameObject.SetActive(true);
    public void ClosePanel() => gameObject.SetActive(false);
}
