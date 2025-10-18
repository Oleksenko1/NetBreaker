using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public string nameLabel;
    public string uniqueName;
    public Sprite sprite;
    public string description;
    public double startingPrice;
    public double priceIncrement;
    public List<UpgradeTypeValue> upgradeTypeValues = new List<UpgradeTypeValue>();
}
