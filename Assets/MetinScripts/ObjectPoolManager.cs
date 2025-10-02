using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    private class Pool
    {
        public readonly Queue<PooledObject> queue = new Queue<PooledObject>(256);
        public GameObject prefab;
        public Transform parent;
    }

    private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>(64);

    void Awake() => Instance = this;

    public void Register(string key, GameObject prefab, int initialCount, Transform parent = null)
    {
        if (string.IsNullOrEmpty(key) || prefab == null) return;
        if (_pools.ContainsKey(key)) return;

        var pool = new Pool { prefab = prefab, parent = parent };
        _pools[key] = pool;

        Prewarm(key, initialCount);
    }

    public void Prewarm(string key, int count)
    {
        if (!_pools.TryGetValue(key, out var pool)) return;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(pool.prefab, pool.parent);
            go.SetActive(false);
            var po = go.GetComponent<PooledObject>();
            if (po == null) po = go.AddComponent<PooledObject>();
            po.PoolKey = key;
            po.Owner = this;
            pool.queue.Enqueue(po);
        }
    }

    public GameObject Spawn(string key, Vector3 pos, Quaternion rot)
    {
        if (!_pools.TryGetValue(key, out var pool)) return null;

        PooledObject po = (pool.queue.Count > 0) ? pool.queue.Dequeue() : CreateNew(pool, key);
        var go = po.gameObject;
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }

    private PooledObject CreateNew(Pool pool, string key)
    {
        var go = Instantiate(pool.prefab, pool.parent);
        var po = go.GetComponent<PooledObject>();
        if (po == null) po = go.AddComponent<PooledObject>();
        po.PoolKey = key;
        po.Owner = this;
        go.SetActive(false);
        return po;
    }

    public void Return(PooledObject po)
    {
        if (po == null) return;
        if (!_pools.TryGetValue(po.PoolKey, out var pool)) { po.gameObject.SetActive(false); return; }
        po.gameObject.SetActive(false);
        pool.queue.Enqueue(po);
    }
    public void ResetAllPools()
    {
        foreach (var kvp in _pools)
        {
            var pool = kvp.Value;

            // Kuyruktaki tüm objeleri kapat
            foreach (var po in pool.queue)
            {
                if (po != null) po.gameObject.SetActive(false);
            }

            // Aktif sahnedeki pool objelerini geri al
            var activeObjects = FindObjectsOfType<PooledObject>();
            foreach (var obj in activeObjects)
            {
                if (obj.PoolKey == kvp.Key && obj.gameObject.activeInHierarchy)
                {
                    Return(obj);
                }
            }
        }
    }
}
