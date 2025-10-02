using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : IEnemyMovement
{
    private NavMeshAgent _agent;
    private Transform _player;

    public void Init(MonoBehaviour owner, Transform player, float speed)
    {
        _agent = owner.GetComponent<NavMeshAgent>();
        if (_agent == null)
            _agent = owner.gameObject.AddComponent<NavMeshAgent>();

        _agent.speed = speed;
        _player = player;
    }

    public void Tick()
    {
        if (_player != null && _agent != null && _agent.isActiveAndEnabled)
            _agent.SetDestination(_player.position);
    }
}
