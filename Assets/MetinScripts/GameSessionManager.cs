using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance;

    void Awake() => Instance = this;

    public void ResetGame()
    {

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.ResetHealth();
        }

        // 2️⃣ Score sıfırla
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetAll();
        }

        // 3️⃣ LevelManager reset
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartLevel();
        }

        // 4️⃣ ObjectPoolManager: tüm pool’ları geri döndür
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ResetAllPools();
        }

        // 5️⃣ EffectPoolManager: tüm efektleri temizle
        if (EffectPoolManager.Instance != null)
        {
            EffectPoolManager.Instance.ResetAllEffects();
        }

        // 6️⃣ GameOver panel kapat
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.HideGameOverPanel();
        }

        // 7️⃣ Zamanı sıfırla
        Time.timeScale = 1f;

    }
}
