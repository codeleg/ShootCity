using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TargetObject : MonoBehaviour, IDamageable
{
    [SerializeField] private ObjectData data;
    public ObjectData Data => data;

    private int _currentHealth;
    public bool IsAlive => _currentHealth > 0;

    void OnEnable()
    {
        _currentHealth = data != null ? data.maxHealth : 1;
    }

    public void TakeDamage(int damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsAlive) return;

        _currentHealth -= damage;
        Debug.Log($"{gameObject.name} {damage} hasar aldı. Kalan HP: {_currentHealth}");

        if (data != null)
        {
            if (!string.IsNullOrEmpty(data.hitEffectKey))
                EffectPoolManager.Instance.SpawnEffect(data.hitEffectKey, hitPoint, hitNormal, 1.25f);

            if (data.hitSound != null)
                AudioPoolManager.Instance.PlayAt(data.hitSound, hitPoint);
        }

        if (_currentHealth <= 0)
            BreakObject(killedByPlayer: true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsAlive) return;

        if (other.TryGetComponent<PlayerHealth>(out var player))
        {
            int dmg = data != null ? data.maxHealth : 1;
            player.TakeDamage(dmg, transform.position);

            BreakObject(killedByPlayer: false); // çifte efekt engellendi
        }
    }

    private void BreakObject(bool killedByPlayer)
    {
        if (data != null)
        {
            if (!string.IsNullOrEmpty(data.breakEffectKey))
                EffectPoolManager.Instance.SpawnEffect(data.breakEffectKey, transform.position, Vector3.up, 2f);

            if (data.breakSound != null)
                AudioPoolManager.Instance.PlayAt(data.breakSound, transform.position);

            if (killedByPlayer)
            {
                int reward = data.coinValue > 0 ? data.coinValue : data.maxHealth;

                GameCoinManager.Instance?.CollectResource(
                    data.resourceType,
                    reward,
                    transform.position,
                    1,
                    null,
                    data.coinAnimType
                );

                Debug.Log($"[TargetObject] {gameObject.name} öldürüldü. Resource: {data.resourceType}, +{reward}");

                LevelManager.Instance?.OnEnemyKilled(data); // ✅ ObjectData gönderiyoruz
            }
        }

        var po = GetComponent<PooledObject>();
        if (po != null) po.ReturnToPool();
        else gameObject.SetActive(false);
    }

    public void SetData(ObjectData newData) => data = newData;
}
