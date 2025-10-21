using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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

    public async UniTask<List<UpgradeLevelEntry>> GetUnlockedUpgradesAsync()
    {
        if (!isLoaded)
            await initCompletion.Task;

        return runtimeData.upgradeLevels;
    }
    public async void SetUnlockedUpgrade(string uniqueId, int level)
    {
        await SetUnlockedUpgradeAsync(uniqueId, level);
    }
    private async UniTask SetUnlockedUpgradeAsync(string _uniqueId, int _level)
    {
        bool foundUpgrade = false;
        for (int i = 0; i < runtimeData.upgradeLevels.Count; i++)
        {
            if (runtimeData.upgradeLevels[i].uniqueId == _uniqueId)
            {
                runtimeData.upgradeLevels[i].level = _level;
                foundUpgrade = true;

                break;
            }
        }

        if (!foundUpgrade)
            runtimeData.upgradeLevels.Add(new UpgradeLevelEntry { uniqueId = _uniqueId, level = _level });

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
    public async void SaveLastSession(DateTime dateTime)
    {
        await SaveLastSessionAsync(dateTime);
    }
    public async UniTask SaveLastSessionAsync(DateTime dateTime, CancellationToken ct = default)
    {
        runtimeData.lastSessionString = dateTime.ToString("O");
        await SaveToDiskAsync(ct);
    }
    public async UniTask<string> GetLastSessionString()
    {
        if (!isLoaded)
            await initCompletion.Task;

        Debug.Log(runtimeData.lastSessionString);

        if (string.IsNullOrEmpty(runtimeData.lastSessionString))
            return DateTime.MinValue.ToString("O");

        return runtimeData.lastSessionString;
    }
    public async UniTask<DateTime> GetLastSessionDatetime()
    {
        if (!isLoaded)
            await initCompletion.Task;

        Debug.Log(runtimeData.lastSessionString);

        if (string.IsNullOrEmpty(runtimeData.lastSessionString))
            return DateTime.MinValue;

        return DateTime.Parse(runtimeData.lastSessionString, null, System.Globalization.DateTimeStyles.RoundtripKind);
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