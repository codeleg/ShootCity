using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

public class GlobalGCTracker : MonoBehaviour
{
    private Dictionary<string, long> lastAllocPerScript = new Dictionary<string, long>();

    void Update()
    {
        // Sahnede aktif olan tüm MonoBehaviour'larý bul
        MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (var behaviour in behaviours)
        {
            if (behaviour == null) continue;

            string scriptName = behaviour.GetType().Name;

            // Þu anki memory kullanýmý
            long currentAlloc = Profiler.GetMonoUsedSizeLong();

            // Bu script için son kaydý al
            long lastAlloc = lastAllocPerScript.ContainsKey(scriptName) ? lastAllocPerScript[scriptName] : currentAlloc;

            long diff = currentAlloc - lastAlloc;

            if (diff > 0)
            {
                Debug.Log($"[GC Alloc] {scriptName} bu framede {diff} byte GC oluþturdu.");
            }

            // Güncelle
            lastAllocPerScript[scriptName] = currentAlloc;
        }
    }
}
