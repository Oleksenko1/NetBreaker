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
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<BitsBalance>(Lifetime.Singleton);
        builder.Register<PassiveClickerManager>(Lifetime.Singleton);
        builder.Register<BitsIncomeTracker>(Lifetime.Singleton);
        builder.Register<UpgradesManager>(Lifetime.Singleton);

        builder.RegisterComponent(clickingManager).AsSelf();
        builder.RegisterComponent(uIUpgradesPanel).AsSelf();
        builder.RegisterComponent(uIMenuButtons).AsSelf();
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
    }
}
