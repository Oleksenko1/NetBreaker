using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

/// <summary>
/// Class responsible for saving the game in needed moments of the game 
/// </summary>
public class SaveManager
{
    private SaveService saveService;
    private BitsBalance bitsBalance;
    private DateTime lastSession;
    private const int AUTOSAVE_DELAY = 2000; // In milliseconds
    private LastSessionSaved_event lastSessionSaved_Event = new();
    [Inject]
    public SaveManager(SaveService saveService, BitsBalance bitsBalance)
    {
        this.saveService = saveService;
        this.bitsBalance = bitsBalance;

        EventBus.Subscribe<BitsSpent_event>(OnBitsSpent);
        EventBus.Subscribe<UpgradePurchased_event>(OnUpgradePurchase);

        InitializeLastSession().Forget();
        AutomaticSaveLoop().Forget();
    }
    private async UniTaskVoid InitializeLastSession()
    {
        lastSession = await saveService.GetLastSessionDatetime();
    }
    private async UniTaskVoid AutomaticSaveLoop()
    {
        while (true)
        {
            await UniTask.Delay(AUTOSAVE_DELAY);
            SaveLastSession();
            SaveCurrentBalance();
        }
    }
    private void SaveCurrentBalance()
    {
        //Debug.Log("Saving balance after AUTOSAVE: " + bitsBalance.GetCurrentBalance());
        saveService.SaveBalance(bitsBalance.GetCurrentBalance());
    }
    private void SaveLastSession()
    {
        saveService.SaveLastSession(DateTime.Now);

        lastSessionSaved_Event.previousSession = lastSession;
        lastSessionSaved_Event.currentSession = DateTime.Now;

        EventBus.Publish(lastSessionSaved_Event);

        lastSession = DateTime.Now;
    }
    private void OnBitsSpent(BitsSpent_event e)
    {
        //Debug.Log("Saving balance after SPENDING: " + e.balanceAmount);
        saveService.SaveBalance(e.balanceAmount);
    }
    private void OnUpgradePurchase(UpgradePurchased_event e)
    {
        //Debug.Log($"Saving balance after UPGRADE PURCHASE: {e.upgradeSO.nameLabel}, level {e.newLevel}");
        saveService.SetUnlockedUpgrade(e.upgradeSO.uniqueName, e.newLevel);
    }
}
