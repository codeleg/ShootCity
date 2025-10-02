using UnityEngine;

public class StraightMovement : IEnemyMovement
{
    private Transform _enemy, _player;
    private float _speed;

    public void Init(MonoBehaviour owner, Transform player, float speed)
    {
        _enemy = owner.transform;
        _player = player;
        _speed = speed;
    }

    public void Tick()
    {
        if (_player == null) return;
        var dir = (_player.position - _enemy.position).normalized;
        _enemy.position += dir * _speed * Time.deltaTime;
    }
}
