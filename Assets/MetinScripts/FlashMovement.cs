using UnityEngine;

public class FlashMovement : IEnemyMovement
{
    private Transform _enemy, _player;
    private float _timer;

    public void Init(MonoBehaviour owner, Transform player, float speed)
    {
        _enemy = owner.transform;
        _player = player;
        _timer = Random.Range(1f, 2f);
    }

    public void Tick()
    {
        if (_player == null) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _enemy.position = Vector3.Lerp(_enemy.position, _player.position, 0.3f);
            _timer = Random.Range(1f, 2f); // tekrar sayacý
        }
    }
}
