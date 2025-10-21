using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private ClickingManager clickingManager;
    [SerializeField] private ClickPopupManager clickPopupManager;
    [SerializeField] private UIBitsBalance uIBitsBalance;
    [SerializeField] private UIUpgradesPanel uIUpgradesPanel;
    [SerializeField] private UIMenuButtons uIMenuButtons;
    private SaveService saveService;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(clickingManager).AsSelf();
        builder.RegisterComponent(uIUpgradesPanel).AsSelf();
        builder.RegisterComponent(uIMenuButtons).AsSelf();

        saveService = new SaveService();
        builder.RegisterInstance(saveService);

        builder.Register<BitsBalance>(Lifetime.Singleton);
        builder.Register<PassiveClickerManager>(Lifetime.Singleton);
        builder.Register<BitsIncomeTracker>(Lifetime.Singleton);
        builder.Register<UpgradesManager>(Lifetime.Singleton);
        builder.Register<SaveManager>(Lifetime.Singleton);
    }
    async void Start()
    {
        Container.Inject(clickingManager);
        Container.Inject(clickPopupManager);
        Container.Inject(uIBitsBalance);
        Container.Inject(uIUpgradesPanel);
        Container.Inject(uIMenuButtons);

        Container.Resolve<PassiveClickerManager>();
        Container.Resolve<BitsIncomeTracker>();
        Container.Resolve<SaveManager>();

        await saveService.InitAsync();
    }
}
