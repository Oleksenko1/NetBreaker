using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using VContainer;

public class UpgradesManager
{
    private BitsBalance bitsBalance;
    private ClickingStats clickingStats;
    private UpgradesListSO upgradesListSO;
    private UniTaskCompletionSource<UpgradesListSO> loadCompletionSource;
    private Dictionary<UpgradeSO, int> upgradesLevels = new Dictionary<UpgradeSO, int>();
    [Inject]
    public UpgradesManager(BitsBalance bitsBalance, ClickingManager clickingManager)
    {
        this.bitsBalance = bitsBalance;
        clickingStats = clickingManager.GetClickingStats();

        loadCompletionSource = new UniTaskCompletionSource<UpgradesListSO>();

        LoadUpgradesListAsync().Forget();
    }
    public async UniTask LoadUpgradesListAsync()
    {
        var handle = Addressables.LoadAssetAsync<UpgradesListSO>("UpgradesListSO");

        upgradesListSO = await handle.Task;

        if (upgradesListSO != null)
        {
            // Sets all levels of upgrades to 0
            for (int i = 0; i < upgradesListSO.list.Count; i++)
            {
                upgradesLevels.Add(upgradesListSO.list[i], 0);
            }
        }

        loadCompletionSource.TrySetResult(upgradesListSO);

        Debug.Log("UpgradesListSO loaded successfully");
    }
    public async UniTask<UpgradesListSO> GetUpgradesListAsync()
    {
        if (upgradesListSO != null)
        {
            return upgradesListSO;
        }

        return await loadCompletionSource.Task;
    }
    public int GetUpgradeLevel(UpgradeSO upgradeSO) => upgradesLevels[upgradeSO];
    public BigNum GetUpgradeCost(UpgradeSO upgradeSO)
    {
        int level = upgradesLevels[upgradeSO];
        double startingPrice = upgradeSO.startingPrice;
        double increment = upgradeSO.priceIncrement;

        BigNum cost = new BigNum(startingPrice * Math.Pow(increment, level));

        return cost;
    }
    public bool TryBuyingUpgrade(UpgradeSO upgradeSO)
    {
        BigNum upgradeCost = GetUpgradeCost(upgradeSO);

        if (bitsBalance.GetCurrentBalance() >= upgradeCost)
        {
            ActivateUpgrade(upgradeSO);

            bitsBalance.WithdrawBits(upgradeCost);
            upgradesLevels[upgradeSO] += 1;

            return true;
        }
        else
        {
            return false;
        }
    }
    private void ActivateUpgrade(UpgradeSO upgradeSO)
    {
        var typeValuesList = upgradeSO.upgradeTypeValues;

        foreach (UpgradeTypeValue typeValue in typeValuesList)
            ExecuteUpgradeType(typeValue);
    }
    private void ExecuteUpgradeType(UpgradeTypeValue typeValue)
    {
        var type = typeValue.type;
        var value = typeValue.value;

        switch (type)
        {
            case UpgradeType.BitsPerClickIncrease:
                clickingStats.AddBitsPerClick(value);
                break;
            case UpgradeType.BitsPerSecondIncrease:
                clickingStats.AddBitsPerSecond(value);
                break;
            default:
                Debug.LogError("!!! Upgrade type is not implemented !!!");
                break;
        }
    }
}