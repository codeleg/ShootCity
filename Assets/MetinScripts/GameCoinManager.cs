using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum ResourceType
{
    Gold,
    Silver,
    Wood,
    Stone
}

public enum CoinAnimType
{
    Default,
    Bounce,
    Spin,
    Shake
}

public class GameCoinManager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceSettings
    {
        public ResourceType type;
        public RectTransform targetUI;
        public GameObject prefab;
        public AudioClip collectSound;
        public CoinAnimType defaultAnim = CoinAnimType.Default; // artık enum
    }

    public static GameCoinManager Instance;

    [Header("Canvas & Coin Parent")]
    public RectTransform coinParent;

    [Header("Anim Ayarları")]
    public float travelTime = 0.45f;
    public Ease travelEase = Ease.InQuad;
    public float scatterRadius = 20f;

    [Header("Resource Settings")]
    public List<ResourceSettings> resourceSettings = new List<ResourceSettings>();

    private Dictionary<ResourceType, ResourceSettings> settingsDict = new Dictionary<ResourceType, ResourceSettings>();
    private Dictionary<ResourceType, List<GameObject>> coinPools = new Dictionary<ResourceType, List<GameObject>>();

    private Tweener endPunchTween;

    void Awake()
    {
        Instance = this;

        foreach (var set in resourceSettings)
        {
            if (!settingsDict.ContainsKey(set.type))
            {
                settingsDict.Add(set.type, set);
                coinPools.Add(set.type, new List<GameObject>());
            }
        }
    }

    /// <summary>
    /// Belirli resource tipinde coin topla
    /// </summary>
    public void CollectResource(ResourceType type, int amount, Vector3 spawnPointWorld, int coinScorePerCoin, AudioClip overrideSound = null, CoinAnimType? animType = null) 
    { 

        if (amount <= 0 || !settingsDict.ContainsKey(type)) return;

        ResourceSettings set = settingsDict[type];
        RectTransform targetUI = set.targetUI;
        AudioClip sfx = overrideSound ?? set.collectSound;
        CoinAnimType animKey = animType ?? set.defaultAnim;

        var canvas = coinParent.GetComponentInParent<Canvas>();
        var uiCam = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        Vector2 endLocal;
        {
            Vector2 endScreen = RectTransformUtility.WorldToScreenPoint(uiCam ?? Camera.main, targetUI.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(coinParent, endScreen, uiCam, out endLocal);
        }

        for (int i = 0; i < amount; i++)
        {
            var coinGO = CoinFromPoolOrCreate(type);
            if (coinGO == null) break;

            var coinRT = coinGO.GetComponent<RectTransform>();
            if (coinRT == null) continue;

            Vector3 screenPt3 = Camera.main.WorldToScreenPoint(spawnPointWorld);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(coinParent, (Vector2)screenPt3, uiCam, out Vector2 localPt);
            localPt += Random.insideUnitCircle * scatterRadius;

            coinRT.anchoredPosition = localPt;
            coinRT.localScale = Vector3.one;
            coinGO.SetActive(true);

            var seq = DOTween.Sequence();
            seq.AppendInterval(Random.Range(0f, 0.05f));

            // 🔥 Animasyon Seçimi (enum ile)
            switch (animKey)
            {
                case CoinAnimType.Bounce:
                    seq.Append(coinRT.DOScale(1.3f, 0.1f).SetEase(Ease.OutBack));
                    break;
                case CoinAnimType.Spin:
                    seq.Join(coinRT.DORotate(new Vector3(0, 0, 360f), travelTime, RotateMode.FastBeyond360));
                    break;
                case CoinAnimType.Shake:
                    seq.Join(coinRT.DOShakeRotation(travelTime, 45f));
                    break;
                default:
                    seq.Append(coinRT.DOScale(1.15f, 0.08f).SetEase(Ease.OutBack, 2f));
                    break;
            }

            seq.Append(coinRT.DOAnchorPos(endLocal, travelTime).SetEase(travelEase));
            seq.Join(coinRT.DOScale(0.8f, travelTime));

            seq.OnComplete(() =>
            {
                if (sfx != null)
                    AudioPoolManager.Instance.PlayAt(sfx, coinParent.position, 1f, 0f, AudioPoolManager.Instance.defaultSFXMixerGroup);

                coinGO.SetActive(false);

                if (endPunchTween == null || !endPunchTween.IsActive() || !endPunchTween.IsPlaying())
                    targetUI.DOPunchScale(new Vector3(0.25f, 0.25f, 0f), 0.12f, 6, 0.6f)
                        .OnComplete(() => targetUI.localScale = Vector3.one);

                if (ScoreManager.Instance != null)
                    ScoreManager.Instance.AddResource(type, coinScorePerCoin);

                coinRT.anchoredPosition = Vector2.zero;
                coinRT.localScale = Vector3.one;
            });
        }
    }

    private GameObject CoinFromPoolOrCreate(ResourceType type)
    {
        List<GameObject> pool = coinPools[type];

        foreach (var coin in pool)
            if (!coin.activeInHierarchy)
                return coin;

        if (settingsDict[type].prefab != null)
        {
            var newCoin = Instantiate(settingsDict[type].prefab, coinParent);
            newCoin.SetActive(false);
            pool.Add(newCoin);
            return newCoin;
        }

        return null;
    }
}
