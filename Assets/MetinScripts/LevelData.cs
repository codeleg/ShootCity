using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("General")]
    public int levelNumber = 1;
    public float durationSeconds = 60f;

    [Header("Enemy Settings")]
    public int totalEnemies = 20;
    public int requiredKills;


    [Header("Spawn Settings")]
    public float spawnPerSecond = 1.5f;
    public float enemyBaseSpeed = 3.0f;

    [Header("Enemy Distribution")]
    public SpawnRule[] spawnRules; // Aðýrlýklý daðýlým
   

    [Header("Spawn Points (Optional Override)")]
    public Transform[] spawnPoints; // Bu Level’a özel spawn noktalarý
 
    [Header("Audio & Environment")]
    public AudioClip backgroundMusic;
 
    public GameObject[] towers;

}

[System.Serializable]
public class SpawnRule
{
    public ObjectData objectData;
    [Range(0, 100)] public int spawnChance; // yüzde
}
