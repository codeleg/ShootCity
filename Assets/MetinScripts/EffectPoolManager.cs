using UnityEngine;
using System.Collections;

public class EffectPoolManager : MonoBehaviour
{
    public static EffectPoolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnEffect(string effectKey, Vector3 pos, Vector3 normal, float autoReturnSeconds = 2f)
    {
        if (string.IsNullOrEmpty(effectKey)) return;

        var go = ObjectPoolManager.Instance.Spawn(
            effectKey,
            pos,
            Quaternion.LookRotation(normal == Vector3.zero ? Vector3.up : normal)
        );

        if (go == null) return;

        StartCoroutine(AutoReturn(go, autoReturnSeconds));
    }

    private IEnumerator AutoReturn(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);

        if (go == null) yield break;

        if (go.TryGetComponent(out PooledObject po))
            po.ReturnToPool();
        else
            go.SetActive(false);
    }

    public void ResetAllEffects()
    {
        var activeEffects = Object.FindObjectsByType<PooledObject>(FindObjectsSortMode.None);

        foreach (var effect in activeEffects)
        {
            if (effect.gameObject.activeInHierarchy)
                effect.ReturnToPool();
        }
    }
}
