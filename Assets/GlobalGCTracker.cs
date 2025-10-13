using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

public class GlobalGCTracker : MonoBehaviour
{
    private long lastAlloc;
    private float timer;
    private const float checkInterval = 0.5f;

    void Start()
    {
        lastAlloc = Profiler.GetMonoUsedSizeLong();
        Debug.Log($"[GCTracker] Baþlangýç: {lastAlloc / 1024} KB");
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < checkInterval) return;
        timer = 0f;

        long current = Profiler.GetMonoUsedSizeLong();
        long diff = current - lastAlloc;

        if (diff > 0)
            Debug.Log($"[GCTracker] Son {checkInterval}s içinde +{diff / 1024f:F1} KB arttý");

        lastAlloc = current;
    }
}
