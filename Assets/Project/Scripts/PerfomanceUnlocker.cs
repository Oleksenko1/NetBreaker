using UnityEngine;

public static class PerformanceUnlocker
{
    public static void Execute()
    {
        Application.targetFrameRate = 60;

        QualitySettings.vSyncCount = 0;
    }
}