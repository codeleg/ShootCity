using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

public class GlobalGCTracker : MonoBehaviour
{
    private long lastGCSize;
    private float checkInterval = 0.5f;
    private float timer;

    void Start()
    {
        lastGCSize = Profiler.GetMonoUsedSizeLong();
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < checkInterval) return; // her 0.5 sn'de bir ölç
        timer = 0f;

        long current = Profiler.GetMonoUsedSizeLong();
        long diff = current - lastGCSize;

        if (diff > 0)
        {
            Debug.Log($"[GC Alloc] Son {checkInterval}s içinde {diff} byte oluþtu.");
        }

        lastGCSize = current;
    }
}
