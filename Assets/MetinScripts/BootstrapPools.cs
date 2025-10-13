using UnityEngine;
using System.Collections;

public class BootstrapPools : MonoBehaviour
{
    [Header("All Enemy Types")]
    [SerializeField] private ObjectData[] allObjects;

    [System.Serializable]
    public struct EffectEntry
    {
        public string key;
        public GameObject prefab;
        public int initial;
    }

    [Header("Effect Prefabs")]
    [SerializeField] private EffectEntry[] effectEntries;

    [Header("Parents")]
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private Transform effectsParent;

    [Header("Optimization")]
    [SerializeField] private bool useIncrementalPrewarm = true; // Uyarý artýk anlamlý

    private void Start()
    {
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("ObjectPoolManager.Instance is null!");
            return;
        }

        if (useIncrementalPrewarm)
            StartCoroutine(IncrementalPrewarm());
        else
            InstantPrewarm();
    }

    private void InstantPrewarm()
    {
        // Enemy Pool
        foreach (var od in allObjects)
        {
            if (od == null || od.prefab == null || string.IsNullOrEmpty(od.poolKey)) continue;
            ObjectPoolManager.Instance.Register(od.poolKey, od.prefab, Mathf.Max(od.initialPoolCount, 200), enemiesParent);
        }

        // Effect Pool
        foreach (var e in effectEntries)
        {
            if (string.IsNullOrEmpty(e.key) || e.prefab == null) continue;
            ObjectPoolManager.Instance.Register(e.key, e.prefab, Mathf.Max(e.initial, 32), effectsParent);
        }
    }

    private IEnumerator IncrementalPrewarm()
    {
        // Enemy Pool
        foreach (var od in allObjects)
        {
            if (od == null || od.prefab == null || string.IsNullOrEmpty(od.poolKey)) continue;
            ObjectPoolManager.Instance.Register(od.poolKey, od.prefab, Mathf.Max(od.initialPoolCount, 200), enemiesParent);
            yield return null; // 1 frame bekle -> GC yükünü daðýtýr
        }

        // Effect Pool
        foreach (var e in effectEntries)
        {
            if (string.IsNullOrEmpty(e.key) || e.prefab == null) continue;
            ObjectPoolManager.Instance.Register(e.key, e.prefab, Mathf.Max(e.initial, 32), effectsParent);
            yield return null;
        }

        Debug.Log("[BootstrapPools] Incremental prewarm completed.");
    }
}
