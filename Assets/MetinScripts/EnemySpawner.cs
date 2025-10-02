using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [SerializeField] private Transform player;
    [SerializeField] private Transform[] spawnPoints;

    private float _spawnAccum;
    private bool _allowSpawning = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // fazladan oluşanı yok et
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // sahne değişse bile korun
    }

    void Update()
    {
        if (!_allowSpawning) return;

        var lvl = LevelManager.Instance?.Current;
        if (lvl == null || player == null) return;

        if (LevelManager.Instance.SpawnedCount >= lvl.totalEnemies)
            return;

        _spawnAccum += Time.deltaTime * lvl.spawnPerSecond;

        while (_spawnAccum >= 1f && LevelManager.Instance.SpawnedCount < lvl.totalEnemies)
        {
            _spawnAccum -= 1f;
            SpawnOne(lvl);
        }
    }

    private Transform[] GetSpawnPoints(LevelData lvl)
    {
        if (lvl != null && lvl.spawnPoints != null && lvl.spawnPoints.Length > 0)
            return lvl.spawnPoints;

        return spawnPoints;
    }

    private void SpawnOne(LevelData lvl)
    {
        var data = LevelManager.Instance.PickWeightedObject();
        if (data == null) return;

        var spList = GetSpawnPoints(lvl);
        if (spList == null || spList.Length == 0) return;

        var sp = spList[Random.Range(0, spList.Length)];
        Vector3 spawnPos = GetRandomPointInSpawnArea(sp);

        var go = ObjectPoolManager.Instance.Spawn(data.poolKey, spawnPos, sp.rotation);
        if (go == null) return;

        var tgt = go.GetComponent<TargetObject>();
        if (tgt != null) tgt.SetData(data);

        var mover = go.GetComponent<EnemyMover>();
        if (mover != null) mover.Configure(player, lvl.enemyBaseSpeed, data);
       // if (mover != null) mover.Configure(player, data.moveSpeed, data);

        LevelManager.Instance?.RegisterSpawn();
    }

    // LevelManager ile uyum için
    public void SpawnEnemy()
    {
        var lvl = LevelManager.Instance?.Current;
        if (lvl == null) return;
        SpawnOne(lvl);
    }

    public void StopSpawning() => _allowSpawning = false;
    public void StartSpawning() => _allowSpawning = true;
    public void ResetSpawnAccumulator() => _spawnAccum = 0f;

    public void SpawnWaveImmediate(int count)
    {
        var lvl = LevelManager.Instance?.Current;
        if (lvl == null) return;
        for (int i = 0; i < count; i++)
            SpawnOne(lvl);
    }

    private Vector3 GetRandomPointInSpawnArea(Transform spawnPoint)
    {
        // BoxCollider varsa
        var box = spawnPoint.GetComponent<BoxCollider>();
        if (box != null)
        {
            Vector3 localPos = new Vector3(
                Random.Range(-box.size.x * 0.5f, box.size.x * 0.5f),
                Random.Range(-box.size.y * 0.5f, box.size.y * 0.5f),
                Random.Range(-box.size.z * 0.5f, box.size.z * 0.5f)
            );
            return spawnPoint.TransformPoint(localPos);
        }

        // SphereCollider varsa
        var sphere = spawnPoint.GetComponent<SphereCollider>();
        if (sphere != null)
        {
            Vector3 localPos = Random.insideUnitSphere * sphere.radius;
            return spawnPoint.TransformPoint(localPos);
        }

        // Fallback → sadece transform pozisyonu
        return spawnPoint.position;
    }

}

//private void SpawnOne(LevelData lvl)
//{
//    var data = LevelManager.Instance.PickWeightedObject();
//    if (data == null) return;

//    var spList = GetSpawnPoints(lvl);
//    if (spList == null || spList.Length == 0) return;

//    var sp = spList[Random.Range(0, spList.Length)];

//    var go = ObjectPoolManager.Instance.Spawn(data.poolKey, sp.position, sp.rotation);
//    if (go == null) return;

//    var tgt = go.GetComponent<TargetObject>();
//    if (tgt != null) tgt.SetData(data);

//    var mover = go.GetComponent<EnemyMover>();
//   // if (mover != null) mover.Configure(player, data.moveSpeed, data);

//    if (mover != null) mover.Configure(player, lvl.enemyBaseSpeed,data);

//    LevelManager.Instance?.RegisterSpawn();
//}