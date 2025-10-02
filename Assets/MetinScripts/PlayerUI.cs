using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; set; }

    [Header("General UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image healthBar; // eski slider yerine image bar
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Resource UI")]
    [SerializeField] private ResourceUI[] resourceUIs;

    private Dictionary<ResourceType, ResourceUI> resourceUIDict = new();

    void Awake()
    {
        Instance = this;

        // dictionary hazırlayalım
        foreach (var res in resourceUIs)
        {
            if (!resourceUIDict.ContainsKey(res.type))
                resourceUIDict.Add(res.type, res);
        }
    }

    public void UpdateScore(int coins)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {coins}";
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBar != null)
            healthBar.fillAmount = (float)current / max;

        if (healthText != null)
            healthText.text = $"{current}";
    }

    public void UpdateTimer(float timeLeft, float total)
    {
        if (timerText != null)
        {
            int t = Mathf.CeilToInt(timeLeft);
            timerText.text = $"{t}s";
        }
    }

    public void OnLevelChanged(int levelNumber)
    {
        if (levelText != null)
            levelText.text = $"LV {levelNumber}";
    }

    public void UpdateResourceUI(ResourceType type, int value)
    {
        if (resourceUIDict.TryGetValue(type, out var ui))
        {
            if (ui.textField != null)
                ui.textField.text = $"{value}";
        }
        else
        {
            Debug.LogWarning($"[PlayerUI] {type} için UI bulunamadı!");
        }
    }

    public void UpdateAllResources(Dictionary<ResourceType, int> allResources)
    {
        foreach (var kvp in allResources)
        {
            UpdateResourceUI(kvp.Key, kvp.Value);
        }
    }
    public void ShowWaveCleared(int levelNumber)
    {
        // Ekranda kısa bir mesaj gösterebilirsin
        if (timerText != null)
        {
            timerText.text = $"Wave {levelNumber} Cleared!";
        }

        // İstersen buraya animasyon, popup panel veya efekt de ekleyebilirsin
        Debug.Log($"Wave {levelNumber} cleared!");
    }
    [Header("Level Feedback")]
    [SerializeField] private TextMeshProUGUI waveClearedText;
    [SerializeField] private TextMeshProUGUI messageText;
    public void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideAfterSeconds(messageText, 2.5f));
        }
    }
    private System.Collections.IEnumerator HideAfterSeconds(TextMeshProUGUI txt, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        txt.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class ResourceUI
{
    public ResourceType type;
    public TextMeshProUGUI textField;
    public Image icon; // opsiyonel, inspector’dan atarsın
}