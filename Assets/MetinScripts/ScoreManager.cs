using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // Her resource için ayrý skor tut
    private Dictionary<ResourceType, int> resourceScores = new Dictionary<ResourceType, int>();

    void Awake()
    {
        if (Instance != null && Instance !=this)
        {
            Destroy(gameObject);
            return;

        }
        Instance = this;
        if (transform.parent != null)
            transform.parent = null;

        DontDestroyOnLoad(gameObject);
        Debug.Log("[ScoreManager] Instance oluþturuldu.");

        // Enum’daki tüm tipler için baþlangýçta 0 atayalým
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (!resourceScores.ContainsKey(type))
                resourceScores[type] = 0;
        }
    }

    /// <summary>
    /// Resource ekle
    /// </summary>
    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[ScoreManager] Geçersiz {type} ekleme denemesi: {amount}");
            return;
        }

        resourceScores[type] += amount;
       // Debug.Log($"[ScoreManager] {amount} {type} eklendi. Toplam: {resourceScores[type]}");

        // UI güncellemesi
        PlayerUI.Instance?.UpdateResourceUI(type, resourceScores[type]);
    }

    /// <summary>
    /// Resource sýfýrla
    /// </summary>
    public void ResetResource(ResourceType type)
    {
        resourceScores[type] = 0;
        Debug.Log($"[ScoreManager] {type} sýfýrlandý.");
        PlayerUI.Instance?.UpdateResourceUI(type, 0);
    }

    /// <summary>
    /// Tüm resource skorlarýný sýfýrla
    /// </summary>
    public void ResetAll()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            resourceScores[type] = 0;

        Debug.Log("[ScoreManager] Tüm resource skorlarý sýfýrlandý.");
        PlayerUI.Instance?.UpdateAllResources(resourceScores);
    }

    /// <summary>
    /// Belirli resource skorunu al
    /// </summary>
    public int GetResource(ResourceType type)
    {
        return resourceScores.ContainsKey(type) ? resourceScores[type] : 0;
    }

    /// <summary>
    /// Tüm skorlarý dictionary olarak döner
    /// </summary>
    public Dictionary<ResourceType, int> GetAllResources()
    {
        return new Dictionary<ResourceType, int>(resourceScores);
    }
}

