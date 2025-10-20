using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UpgradesListSO", menuName = "Scriptable Objects/UpgradesListSO")]
public class UpgradesListSO : ScriptableObject
{
    public List<UpgradeSO> list = new List<UpgradeSO>();
}
