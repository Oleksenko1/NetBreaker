using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(UpgradeSO), true)]
public class LegacyUpgradeSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif