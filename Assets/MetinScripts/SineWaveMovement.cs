using UnityEngine;

public class SineWaveMovement : IEnemyMovement
{
    private Transform _enemy, _player;
    private float _speed;
    private float _frequency = 3f;
    private float _magnitude = 1f;
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
        Vector3 offset = Vector3.up * Mathf.Sin(_time) * _magnitude;

        _enemy.position += (dir * _speed * Time.deltaTime) + offset * Time.deltaTime;
    }
}
