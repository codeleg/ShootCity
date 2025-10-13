using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [HideInInspector] public string PoolKey;
    [HideInInspector] public ObjectPoolManager Owner;

    // caches
    [HideInInspector] public TargetObject CachedTarget;
    [HideInInspector] public EnemyMover CachedMover;

    private void Awake()
    {
        // cache if present
        CachedTarget = GetComponent<TargetObject>();
        CachedMover = GetComponent<EnemyMover>();
    }

    public void ReturnToPool()
    {
        if (Owner != null && !string.IsNullOrEmpty(PoolKey))
            Owner.Return(this);
        else
            gameObject.SetActive(false);
    }
}
