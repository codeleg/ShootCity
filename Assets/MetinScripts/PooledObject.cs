using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [HideInInspector] public string PoolKey; // Hangi havuzdan geldi
    [HideInInspector] public ObjectPoolManager Owner;

    public void ReturnToPool()
    {
        if (Owner != null && !string.IsNullOrEmpty(PoolKey))
            Owner.Return(this);
        else
            gameObject.SetActive(false); // Fallback
    }
}
