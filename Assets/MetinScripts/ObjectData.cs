using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectData", menuName = "Game/Object Data")]
public class ObjectData : ScriptableObject
{
    [Header("Identity")]
    public string objectType;

    [Header("Stats")]
    public int maxHealth = 10;
    public int coinValue = 10;

    public ResourceType resourceType = ResourceType.Gold;
    [Tooltip("Coin animasyon tipi (Inspector’dan seçilir)")]
    public CoinAnimType coinAnimType = CoinAnimType.Default;

    [Header("Prefab & Pool")]
    public string poolKey;
    public GameObject prefab;
    public int initialPoolCount = 200;

    [Header("Effects (Pool Keys)")]
    public string hitEffectKey;
    public string breakEffectKey;
    public string impactEffectKey;

    [Header("Audio Clips")]
    public AudioClip hitSound;
    public AudioClip breakSound;
    public AudioClip impactSound;


    [Header("Movement")]
    public EnemyMovementType movementType; // hangi hareket sistemi
    public float moveSpeed = 3f;
}
public enum EnemyMovementType
{
    Straight,
    NavMesh,
    Flash,
    ZigZag,
    SineWave,
    Rolling
}
