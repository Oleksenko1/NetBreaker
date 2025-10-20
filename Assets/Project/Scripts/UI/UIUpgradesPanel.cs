using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIUpgradesPanel : MonoBehaviour
{
    [SerializeField] private Transform contentTR;
    [SerializeField] private Transform upgradeUnitPf;
    [Header("Buttons")]
    [SerializeField] private Button openBtn;
    [SerializeField] private Button closeBtn;
    [Inject] private UpgradesManager upgradesManager;
    void Start()
    {
        InitializeAsync().Forget();
    }
    private async UniTaskVoid InitializeAsync()
    {
        var upgradesList = await upgradesManager.GetUpgradesListAsync();

        Debug.Log("Initialization complete with upgrades list");
    }
}
