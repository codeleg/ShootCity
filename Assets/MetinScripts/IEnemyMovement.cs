using UnityEngine;

public interface IEnemyMovement
{
    void Init(MonoBehaviour owner, Transform player, float speed);
    void Tick(); // her Update’de çaðrýlacak
}
