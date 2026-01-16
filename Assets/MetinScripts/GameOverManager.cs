using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    public static GameOverManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
        gameOverPanel?.SetActive(false);

        Time.timeScale = 1f;

        GameSessionManager.Instance.ResetGame();
        LevelManager.Instance?.RestartLevel();

        HandleCursor(false);
    }


    /// <summary>
    /// Continue butonundan çağrılır. Ödüllü reklam izlenirse full canla devam edilir.
    /// </summary>

    public void ContinueGame()
    {
        if (AdsManager.Instance == null)
        {
            Debug.LogWarning("AdsManager yok, continue yapılamıyor.");
            return;
        }

        Time.timeScale = 1f; // reklamdan ÖNCE aç

        AdsManager.Instance.ShowRewarded(() =>
        {
            var player = Object.FindAnyObjectByType<PlayerHealth>();
            player?.ResetHealth();

            gameOverPanel?.SetActive(false);
            LevelManager.Instance?.ResetTimer();

            HandleCursor(false);
        });
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

