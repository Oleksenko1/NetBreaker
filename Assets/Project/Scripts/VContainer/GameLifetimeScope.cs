using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private ClickingManager clickingManager;
    [SerializeField] private ClickPopupManager clickPopupManager;
    [SerializeField] private UIBitsBalance uIBitsBalance;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<BitsBalance>(Lifetime.Singleton);
        builder.Register<PassiveClickerManager>(Lifetime.Singleton);
        builder.RegisterComponent(clickingManager).AsSelf();
    }
    async void Start()
    {
        Container.Inject(clickingManager);
        Container.Inject(clickPopupManager);
        Container.Inject(uIBitsBalance);

        Container.Resolve<PassiveClickerManager>();
    }
}
