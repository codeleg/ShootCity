using UnityEngine;

public class ZigZagMovement : IEnemyMovement
{
    private Transform _enemy, _player;
    private float _speed;
    private float _frequency = 5f; // zigzag sýklýðý
    private float _magnitude = 1.5f; // zigzag geniþliði
    private float _time;

    public void Init(MonoBehaviour owner, Transform player, float speed)
    {
        _enemy = owner.transform;
        _player = player;
        _speed = speed;
    }

    public void Tick()
    {
        if (_player == null) return;

        _time += Time.deltaTime * _frequency;

        Vector3 dir = (_player.position - _enemy.position).normalized;
        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized; // dik yön

        _enemy.position += (dir * _speed + perp * Mathf.Sin(_time) * _magnitude) * Time.deltaTime;
    }
}
