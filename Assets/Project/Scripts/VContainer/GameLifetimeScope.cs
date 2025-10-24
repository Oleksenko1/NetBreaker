using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private ClickingManager clickingManager;
    [SerializeField] private ClickPopupManager clickPopupManager;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private UIBitsBalance uIBitsBalance;
    [SerializeField] private UIUpgradesPanel uIUpgradesPanel;
    [SerializeField] private UIMenuButtons uIMenuButtons;
    [SerializeField] private UISettingsPanel uISettingsPanel;
    [SerializeField] private UIOfflineEarnings uIOfflineEarnings;
    private SaveService saveService;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(clickingManager).AsSelf();
        builder.RegisterComponent(soundPlayer).AsSelf();
        builder.RegisterComponent(uIUpgradesPanel).AsSelf();
        builder.RegisterComponent(uIMenuButtons).AsSelf();
        builder.RegisterComponent(uISettingsPanel).AsSelf();
        builder.RegisterComponent(uIOfflineEarnings).AsSelf();

        saveService = new SaveService();
        builder.RegisterInstance(saveService);

        builder.Register<BitsBalance>(Lifetime.Singleton);
        builder.Register<PassiveClickerManager>(Lifetime.Singleton);
        builder.Register<BitsIncomeTracker>(Lifetime.Singleton);
        builder.Register<UpgradesManager>(Lifetime.Singleton);
        builder.Register<SaveManager>(Lifetime.Singleton);
        builder.Register<OfflineIncomeManager>(Lifetime.Singleton);
    }
    async void Start()
    {
        Container.Inject(clickingManager);
        Container.Inject(clickPopupManager);
        Container.Inject(uIBitsBalance);
        Container.Inject(uIUpgradesPanel);
        Container.Inject(uIMenuButtons);
        Container.Inject(uISettingsPanel);
        Container.Inject(uIOfflineEarnings);

        Container.Resolve<PassiveClickerManager>();
        Container.Resolve<BitsIncomeTracker>();
        Container.Resolve<SaveManager>();
        Container.Resolve<OfflineIncomeManager>();

        PerformanceUnlocker.Execute();

        await saveService.InitAsync();
    }
    [ContextMenu("Reset savefile")]
    private void ResetSavefile()
    {
        ResetSavefileAsync().Forget();
    }
    private async UniTask ResetSavefileAsync()
    {
        await saveService.DeleteSaveAsync();

        SceneManager.LoadScene(0);
    }
}
