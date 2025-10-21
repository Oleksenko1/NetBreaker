using UnityEngine;
using System.Collections.Generic;
using System;
[System.Serializable]
public class SaveData
{
    public BigNum bitsBalance;
    public List<UpgradeLevelEntry> upgradeLevels = new List<UpgradeLevelEntry>();
    public string lastSessionString;
}
