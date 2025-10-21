using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class SaveData
{
    public BigNum bitsBalance;
    public List<UpgradeLevelEntry> upgradeLevels = new List<UpgradeLevelEntry>();
}
