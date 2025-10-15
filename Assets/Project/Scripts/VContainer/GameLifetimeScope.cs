using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private ClickingZone clickingZone;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(clickingZone).AsSelf();
    }
    async void Start()
    {

    }
}
