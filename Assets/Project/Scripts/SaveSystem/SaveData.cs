using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class SaveData
{
    public BigNum bitsBalance;
    public Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();
}
