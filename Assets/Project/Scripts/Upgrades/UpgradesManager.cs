using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public class UpgradesManager
{
    private UpgradesListSO upgradesList;
    private UniTaskCompletionSource<UpgradesListSO> loadCompletionSource;
    public UpgradesManager()
    {
        loadCompletionSource = new UniTaskCompletionSource<UpgradesListSO>();

        LoadUpgradesListAsync().Forget();
    }
    public async UniTask LoadUpgradesListAsync()
    {
        var handle = Addressables.LoadAssetAsync<UpgradesListSO>("UpgradesListSO");

        upgradesList = await handle.Task;

        loadCompletionSource.TrySetResult(upgradesList);

        Debug.Log("UpgradesListSO loaded successfully");
    }

    public async UniTask<UpgradesListSO> GetUpgradesListAsync()
    {
        if (upgradesList != null)
        {
            return upgradesList;
        }

        return await loadCompletionSource.Task;
    }
}