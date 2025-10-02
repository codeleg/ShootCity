using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
    [SerializeField] private GameObject gameOverPanel;

    void Awake() => Instance = this;

    /// <summary>
    /// Oyuncunun öldüğü anda GameOver panelini açar.
    /// </summary>
    public void TriggerGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        HandleCursor(true); // Panel açıldığında cursor serbest (PC için)
    }

    /// <summary>
    /// Restart butonundan çağrılır. Önce interstitial reklam, sonra oyun sıfırlanır.
    /// </summary>
    public void RestartGame()
    {
        if (AdsManager.Instance != null && AdsManager.Instance.IsInterstitialReady())
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                DoRestart();
            });
        }
        else
        {
            DoRestart();
        }
    }

    /// <summary>
    /// Asıl restart işlemini yapar (paneli kapatır, ResetGame çağırır).
    /// </summary>
    private void DoRestart()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        GameSessionManager.Instance.ResetGame();

        HandleCursor(false); // Restart sonrası cursor tekrar gizlensin (PC için)
    }

    /// <summary>
    /// Continue butonundan çağrılır. Ödüllü reklam izlenirse full canla devam edilir.
    /// </summary>
   
    public void ContinueGame()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowRewarded(() =>
            {
                var player = FindObjectOfType<PlayerHealth>();
                if (player != null)
                {
                    player.ResetHealth(); // canı fulle

                }

                if (gameOverPanel != null)
                    gameOverPanel.SetActive(false);

                // Timer’ı sıfırla, ama düşmanları temizleme
                LevelManager.Instance?.ResetTimer();

                if (gameOverPanel != null)
                    gameOverPanel.SetActive(false);

                Time.timeScale = 1f; // hız normale dönmeli
               
            });
  
        }
        else
        {
            Debug.LogWarning("AdsManager yok, continue yapılamıyor.");
        }
    }


    /// <summary>
    /// GameOver panelini kapatır (dışarıdan erişim için).
    /// </summary>
    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        HandleCursor(false);
    }

    /// <summary>
    /// PC platformunda cursor kontrolünü yapar (mobilde hiçbir şey yapmaz).
    /// </summary>
    public void HandleCursor(bool showCursor)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        Cursor.visible = showCursor;
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
#endif
    }
}

