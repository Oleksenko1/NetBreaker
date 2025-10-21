using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveService
{
    private const string SaveFileName = "savefile.json";
    private SaveData runtimeData = new();
    private bool isLoaded = false;

    private readonly SemaphoreSlim saveSemaphore = new(1);
    private string GetSavePath() => Path.Combine(Application.persistentDataPath, SaveFileName);

    private UniTaskCompletionSource initCompletion = new();
    public async UniTask InitAsync(CancellationToken ct = default)
    {
        if (isLoaded) return;

        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(path), cancellationToken: ct);
            if (!string.IsNullOrEmpty(json))
                runtimeData = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }

        isLoaded = true;
        initCompletion.TrySetResult();
    }

    public async UniTask SaveToDiskAsync(CancellationToken ct = default)
    {
        await saveSemaphore.WaitAsync();
        try
        {
            string path = GetSavePath();
            string json = JsonUtility.ToJson(runtimeData, true);

            await UniTask.RunOnThreadPool(() =>
            {
                if (!ct.IsCancellationRequested)
                    File.WriteAllText(path, json);
            }, cancellationToken: ct);
        }
        finally
        {
            saveSemaphore.Release();
        }
    }

    public async UniTask<Dictionary<string, int>> GetUnlockedUpgradesAsync()
    {
        if (!isLoaded)
            await initCompletion.Task;

        return runtimeData.upgradeLevels;
    }
    public async void SetUnlockedUpgrade(string uniqueId, int level)
    {
        await SetUnlockedUpgradeAsync(uniqueId, level);
    }
    private async UniTask SetUnlockedUpgradeAsync(string uniqueId, int level)
    {
        if (runtimeData.upgradeLevels.ContainsKey(uniqueId))
            runtimeData.upgradeLevels[uniqueId] = level;
        else
            runtimeData.upgradeLevels.Add(uniqueId, level);

        await SaveToDiskAsync();
    }
    public async UniTask<BigNum> GetSavedBalance()
    {
        if (!isLoaded)
            await initCompletion.Task;

        return runtimeData.bitsBalance;
    }

    public async void SaveBalance(BigNum newBalance)
    {
        await SaveBalanceAsync(newBalance);
    }

    public async UniTask SaveBalanceAsync(BigNum newBalance, CancellationToken ct = default)
    {
        runtimeData.bitsBalance = newBalance;
        await SaveToDiskAsync(ct);
    }

    public async UniTask DeleteSaveAsync(CancellationToken ct = default)
    {
        await saveSemaphore.WaitAsync();
        try
        {
            string path = GetSavePath();

            PlayerPrefs.DeleteAll();

            await UniTask.RunOnThreadPool(() =>
            {
                if (File.Exists(path))
                    File.Delete(path);
            }, cancellationToken: ct);

            runtimeData = new SaveData();
            isLoaded = false;
        }
        finally
        {
            saveSemaphore.Release();
        }
    }
}