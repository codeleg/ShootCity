using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private int startLevelIndex = 0;

    private int _idx;
    private LevelData _current;

    private int _spawnedCount;
    private int _killedCount;
    private bool _objectiveComplete;
    private float _timeLeft;

    private static readonly WaitForSeconds LevelDelay = new WaitForSeconds(1f);

    public LevelData Current => _current;
    public int CurrentLevelNumber => _current != null ? _current.levelNumber : 0;
    public int SpawnedCount => _spawnedCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        LoadLevel(startLevelIndex);
    }

    private void Update()
    {
        if (_current == null || _objectiveComplete) return;

        // Süre sayacı
        _timeLeft -= Time.deltaTime;
        PlayerUI.Instance?.UpdateTimer(Mathf.Max(_timeLeft, 0f), _current.durationSeconds);

        if (_timeLeft <= 0f && !_objectiveComplete)
        {
#if UNITY_EDITOR
            Debug.Log("[LevelManager] Süre doldu, görev başarısız!");
#endif
            GameOverManager.Instance?.TriggerGameOver();
            return;
        }

        // Öldürülmesi gereken düşman sayısı
        int requiredKills = _current.requiredKills > 0 ? _current.requiredKills : _current.totalEnemies;

        if (_killedCount >= requiredKills && !_objectiveComplete)
        {
            _objectiveComplete = true;
            PlayerUI.Instance?.ShowWaveCleared(CurrentLevelNumber);
            StartCoroutine(NextLevelRoutine());
        }
    }

    public void LoadLevel(int index)
    {
        if (levels == null || levels.Count == 0) return;

        _idx = Mathf.Clamp(index, 0, levels.Count - 1);
        _current = levels[_idx];

        _spawnedCount = 0;
        _killedCount = 0;
        _objectiveComplete = false;
        _timeLeft = _current.durationSeconds;

        if (_current.backgroundMusic != null)
            AudioPoolManager.Instance.PlayAt(_current.backgroundMusic, Vector3.zero);

        PlayerUI.Instance?.OnLevelChanged(CurrentLevelNumber);

        // Spawn sistemi bilgilendir
        EnemySpawner.Instance?.ResetSpawnAccumulator();
        EnemySpawner.Instance?.StartSpawning();
    }

    public void RegisterSpawn() => _spawnedCount++;

    public void OnEnemyKilled(ObjectData data)
    {
        _killedCount++;

        int requiredKills = _current.requiredKills > 0 ? _current.requiredKills : _current.totalEnemies;

        if (!_objectiveComplete && _killedCount >= requiredKills)
        {
            _objectiveComplete = true;
            PlayerUI.Instance?.ShowWaveCleared(CurrentLevelNumber);
            StartCoroutine(NextLevelRoutine());
        }
    }

    private IEnumerator NextLevelRoutine()
    {
        yield return LevelDelay;
        NextLevel();
    }

    public void RestartLevel()
    {
        if (_current == null) return;

        _spawnedCount = 0;
        _killedCount = 0;
        _objectiveComplete = false;
        _timeLeft = _current.durationSeconds;

        ObjectPoolManager.Instance.ResetAllPools();

        PlayerUI.Instance?.ShowMessage("Görev başarısız! Yeniden başlıyor...");
        LoadLevel(_idx);
    }

    public void NextLevel()
    {
        int next = Mathf.Min(_idx + 1, levels.Count - 1);
        LoadLevel(next);
    }

    public ObjectData PickWeightedObject()
    {
        if (_current == null || _current.spawnRules == null || _current.spawnRules.Length == 0)
            return null;

        int totalChance = 0;
        foreach (var rule in _current.spawnRules)
            totalChance += rule.spawnChance;

        int roll = Random.Range(0, totalChance);
        int cumulative = 0;

        foreach (var rule in _current.spawnRules)
        {
            cumulative += rule.spawnChance;
            if (roll < cumulative)
                return rule.objectData;
        }

        return _current.spawnRules[0].objectData;
    }

    public void ResetTimer()
    {
        if (_current == null) return;

        _timeLeft = _current.durationSeconds;
        PlayerUI.Instance?.UpdateTimer(_timeLeft, _current.durationSeconds);

#if UNITY_EDITOR
        Debug.Log("[LevelManager] Timer sıfırlandı. Yeni süre: " + _timeLeft);
#endif
    }
}

