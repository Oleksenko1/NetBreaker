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
    private SaveService saveService;
    private UpgradesListSO upgradesListSO;
    private UniTaskCompletionSource<UpgradesListSO> loadCompletionSource;
    private Dictionary<UpgradeSO, int> upgradesLevels = new Dictionary<UpgradeSO, int>();
    private UpgradePurchased_event upgradePurchased_Event = new UpgradePurchased_event();
    [Inject]
    public UpgradesManager(BitsBalance bitsBalance, ClickingManager clickingManager, SaveService saveService)
    {
        this.bitsBalance = bitsBalance;
        this.saveService = saveService;
        clickingStats = clickingManager.GetClickingStats();

        loadCompletionSource = new UniTaskCompletionSource<UpgradesListSO>();

        LoadUpgradesListAsync().Forget();
    }
    public async UniTask LoadUpgradesListAsync()
    {
        var handle = Addressables.LoadAssetAsync<UpgradesListSO>("UpgradesListSO");

        upgradesListSO = await handle.Task;

        // Initializing all upgrades in the game
        if (upgradesListSO != null)
        {
            // Sets all levels of upgrades to 0
            for (int i = 0; i < upgradesListSO.list.Count; i++)
            {
                upgradesLevels.Add(upgradesListSO.list[i], 0);
            }
        }

        List<UpgradeLevelEntry> unlockedUpgrades = await saveService.GetUnlockedUpgradesAsync();
        
        // Setting levels to purchased upgrades from the save file
        foreach (UpgradeLevelEntry upgradeLevelEntry in unlockedUpgrades)
        {
            string uniqueId = upgradeLevelEntry.uniqueId;
            int level = upgradeLevelEntry.level;

            foreach (UpgradeSO upgradeSO in upgradesLevels.Keys)
            {
                if (upgradeSO.uniqueName == uniqueId)
                {
                    upgradesLevels[upgradeSO] = level;

                    // Activating upgrade
                    for (int i = 0; i < level; i++)
                    {
                        ActivateUpgrade(upgradeSO);
                    }

                    break;
                }
            }
        }

        loadCompletionSource.TrySetResult(upgradesListSO);
    }
    public async UniTask<UpgradesListSO> GetUpgradesListAsync()
    {
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

            upgradePurchased_Event.upgradeSO = upgradeSO;
            upgradePurchased_Event.newLevel = upgradesLevels[upgradeSO];
            EventBus.Publish(upgradePurchased_Event);

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