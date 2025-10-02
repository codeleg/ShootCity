using UnityEngine;

[RequireComponent(typeof(TargetObject))]
public class EnemyMover : MonoBehaviour
{
    private Transform _player;
    private TargetObject _target;
    private float _speed = 3f;

    private IEnemyMovement _movement; // strateji

    void Awake() => _target = GetComponent<TargetObject>();

    public void Configure(Transform player, float speed, ObjectData data)
    {
        _player = player;
        _speed = speed;

        // Hangi hareket sistemi kullanýlacak?
        switch (data.movementType)
        {
            case EnemyMovementType.Straight: _movement = new StraightMovement(); break;
            case EnemyMovementType.NavMesh: _movement = new NavMeshMovement(); break;
            case EnemyMovementType.Flash: _movement = new FlashMovement(); break;
            case EnemyMovementType.ZigZag: _movement = new ZigZagMovement(); break;
            case EnemyMovementType.SineWave: _movement = new SineWaveMovement(); break;
            case EnemyMovementType.Rolling: _movement = new RollingMovement(); break;
        }
        _movement?.Init(this, _player, _speed);
    }

    void Update()
    {
        if (_player == null || !_target.IsAlive) return;
        _movement?.Tick();
    }

    // OnTriggerEnter kýsmý hiç deðiþmeden kalýr
    void OnTriggerEnter(Collider other)
    {
        if (!_target.IsAlive) return;
        if (!other.CompareTag("Player")) return;

        var ph = other.GetComponent<PlayerHealth>();
        if (ph != null && _target.Data != null)
        {
            int dmg = _target.Data.maxHealth;
            ph.TakeDamage(dmg, transform.position);

            EffectPoolManager.Instance.SpawnEffect(_target.Data.impactEffectKey, transform.position, Vector3.up, 1.5f);
            AudioPoolManager.Instance.PlayAt(_target.Data.impactSound, transform.position);
        }
        // Çarpýnca yok ol ( poola gönderidm)
        var po = GetComponent<PooledObject>();
        if (po != null) po.ReturnToPool();
        else gameObject.SetActive(false);
    }
}
