using UnityEngine;

public class RollingMovement : IEnemyMovement
{
    private Transform _enemy, _player;
    private float _speed;
    private float _rollSpeed = 360f; // derece/sn

    public void Init(MonoBehaviour owner, Transform player, float speed)
    {
        _enemy = owner.transform;
        _player = player;
        _speed = speed;
    }

    public void Tick()
    {
        if (_player == null) return;

        Vector3 dir = (_player.position - _enemy.position).normalized;

        _enemy.position += dir * _speed * Time.deltaTime;

        // X ekseni etrafýnda yuvarlanma animasyonu
        _enemy.Rotate(Vector3.right, _rollSpeed * Time.deltaTime, Space.Self);
    }
}
