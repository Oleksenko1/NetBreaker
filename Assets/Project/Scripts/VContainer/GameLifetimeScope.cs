using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private ClickingManager clickingManager;
    [SerializeField] private UIBitsBalance uIBitsBalance;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<BitsBalance>(Lifetime.Singleton);
    }
    async void Start()
    {
        Container.Inject(clickingManager);
        Container.Inject(uIBitsBalance);
    }
}
