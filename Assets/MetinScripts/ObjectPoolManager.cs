using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    private class Pool
    {
        public readonly Queue<PooledObject> queue = new Queue<PooledObject>(125);
        public GameObject prefab;
        public Transform parent;
        public int prewarmed = 0;
    }

    private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>(64);

    [Header("Prewarm Settings")]
    [Tooltip("Max prefab instances to prewarm per key by default")]
    [SerializeField] private int defaultPrewarmCap = 50;
    [Tooltip("How many instances to instantiate per frame when prewarming heavy")]
    [SerializeField] private int prewarmPerFrame = 10;

    void Awake() => Instance = this;

    public void Register(string key, GameObject prefab, int initialCount, Transform parent = null)
    {
        if (string.IsNullOrEmpty(key) || prefab == null) return;
        if (_pools.ContainsKey(key)) return;

        var pool = new Pool { prefab = prefab, parent = parent };
        _pools[key] = pool;

        // limit initialCount to a reasonable cap to avoid huge single-frame allocation
        int cap = Mathf.Min(initialCount, defaultPrewarmCap);

        // If caller explicitly requests a very large initialCount (> defaultPrewarmCap),
        // schedule async prewarm for remaining
        if (initialCount <= cap)
        {
            PrewarmInternal(key, cap);
        }
        else
        {
            PrewarmInternal(key, cap);
            // schedule the rest spread over frames
            StartCoroutine(PrewarmAsync(key, initialCount - cap));
        }
    }

    private void PrewarmInternal(string key, int count)
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
            pool.prewarmed++;
        }
    }

    private IEnumerator PrewarmAsync(string key, int remaining)
    {
        if (!_pools.TryGetValue(key, out var pool)) yield break;

        while (remaining > 0)
        {
            int toDo = Mathf.Min(prewarmPerFrame, remaining);
            for (int i = 0; i < toDo; i++)
            {
                var go = Instantiate(pool.prefab, pool.parent);
                go.SetActive(false);
                var po = go.GetComponent<PooledObject>();
                if (po == null) po = go.AddComponent<PooledObject>();
                po.PoolKey = key;
                po.Owner = this;
                pool.queue.Enqueue(po);
                pool.prewarmed++;
            }
            remaining -= toDo;
            // yield one frame to spread allocations
            yield return null;
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

    // Optimized ResetAllPools uses parent traversal, not FindObjectsOfType
    public void ResetAllPools()
    {
        foreach (var pool in _pools.Values)
        {
            if (pool.parent == null) continue;
            var allObjects = pool.parent.GetComponentsInChildren<PooledObject>(true);
            for (int i = 0; i < allObjects.Length; i++)
            {
                var po = allObjects[i];
                if (po != null && po.gameObject.activeInHierarchy)
                    Return(po);
            }
        }
    }
}

//using UnityEngine;
//using System.Collections.Generic;

//public class ObjectPoolManager : MonoBehaviour
//{
//    public static ObjectPoolManager Instance;

//    private class Pool
//    {
//        public readonly Queue<PooledObject> queue = new Queue<PooledObject>(125);
//        public GameObject prefab;
//        public Transform parent;
//    }

//    private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>(64);

//    void Awake() => Instance = this;

//    public void Register(string key, GameObject prefab, int initialCount, Transform parent = null)
//    {
//        if (string.IsNullOrEmpty(key) || prefab == null) return;
//        if (_pools.ContainsKey(key)) return;

//        var pool = new Pool { prefab = prefab, parent = parent };
//        _pools[key] = pool;

//        Prewarm(key, initialCount);
//    }

//    public void Prewarm(string key, int count)
//    {
//        if (!_pools.TryGetValue(key, out var pool)) return;

//        for (int i = 0; i < count; i++)
//        {
//            var go = Instantiate(pool.prefab, pool.parent);
//            go.SetActive(false);
//            var po = go.GetComponent<PooledObject>();
//            if (po == null) po = go.AddComponent<PooledObject>();
//            po.PoolKey = key;
//            po.Owner = this;
//            pool.queue.Enqueue(po);
//        }
//    }

//    public GameObject Spawn(string key, Vector3 pos, Quaternion rot)
//    {
//        if (!_pools.TryGetValue(key, out var pool)) return null;

//        PooledObject po = (pool.queue.Count > 0) ? pool.queue.Dequeue() : CreateNew(pool, key);
//        var go = po.gameObject;
//        go.transform.SetPositionAndRotation(pos, rot);
//        go.SetActive(true);
//        return go;
//    }

//    private PooledObject CreateNew(Pool pool, string key)
//    {
//        var go = Instantiate(pool.prefab, pool.parent);
//        var po = go.GetComponent<PooledObject>();
//        if (po == null) po = go.AddComponent<PooledObject>();
//        po.PoolKey = key;
//        po.Owner = this;
//        go.SetActive(false);
//        return po;
//    }

//    public void Return(PooledObject po)
//    {
//        if (po == null) return;
//        if (!_pools.TryGetValue(po.PoolKey, out var pool)) { po.gameObject.SetActive(false); return; }
//        po.gameObject.SetActive(false);
//        pool.queue.Enqueue(po);
//    }

//    public void ResetAllPools()
//    {
//        foreach (var pool in _pools.Values)
//        {
//            var allObjects = pool.parent.GetComponentsInChildren<PooledObject>(true);
//            foreach (var po in allObjects)
//            {
//                if (po != null && po.gameObject.activeInHierarchy)
//                    Return(po);
//            }
//        }
//    }

//    //public void ResetAllPools()
//    //{
//    //    foreach (var kvp in _pools)
//    //    {
//    //        var pool = kvp.Value;

//    //        // Kuyruktaki tüm objeleri kapat
//    //        foreach (var po in pool.queue)
//    //        {
//    //            if (po != null) po.gameObject.SetActive(false);
//    //        }

//    //        // Aktif sahnedeki pool objelerini geri al
//    //        var activeObjects = FindObjectsOfType<PooledObject>();
//    //        foreach (var obj in activeObjects)
//    //        {
//    //            if (obj.PoolKey == kvp.Key && obj.gameObject.activeInHierarchy)
//    //            {
//    //                Return(obj);
//    //            }
//    //        }
//    //    }
//    //}
//}
