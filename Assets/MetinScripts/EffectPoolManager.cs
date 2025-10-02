using UnityEngine;
using System.Collections;

public class EffectPoolManager : MonoBehaviour
{
    public static EffectPoolManager Instance;

    void Awake() => Instance = this;

    public void SpawnEffect(string effectKey, Vector3 pos, Vector3 normal, float autoReturnSeconds = 2f)
    {
        if (string.IsNullOrEmpty(effectKey)) return;

        // ObjectPoolManager üzerinden spawn
        var go = ObjectPoolManager.Instance.Spawn(effectKey, pos, Quaternion.LookRotation(normal == Vector3.zero ? Vector3.up : normal));
        if (go == null) return;

        StartCoroutine(AutoReturn(go, autoReturnSeconds));
    }

    private IEnumerator AutoReturn(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        var po = go.GetComponent<PooledObject>();
        if (po != null) po.ReturnToPool();
        else go.SetActive(false);
    }

    public void ResetAllEffects()
    {
        var activeEffects = FindObjectsOfType<PooledObject>();
        foreach (var effect in activeEffects)
        {
            if (effect.gameObject.activeInHierarchy)
                effect.ReturnToPool();
        }
    }
}
