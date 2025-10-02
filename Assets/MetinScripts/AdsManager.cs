using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    private InterstitialAd interstitial;
    private RewardedAd rewarded;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Google Mobile Ads baþlat
        MobileAds.Initialize(initStatus => { });
    }

    void Start()
    {
        RequestInterstitial();
        RequestRewarded();
    }


    // --- Interstitial (Geçiþ Reklamý) ---
    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; // Test ID
#elif UNITY_IOS
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial failed to load: " + error);
                return;
            }

            interstitial = ad;

            interstitial.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial closed, requesting new one...");
                RequestInterstitial(); // tekrar yükle
            };
        });
    }

    public void ShowInterstitial(Action onClosed = null)
    {
        if (interstitial != null)
        {
            interstitial.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial closed");
                onClosed?.Invoke();
                RequestInterstitial(); // sonraki reklamý hazýrla
            };

            interstitial.Show();
            interstitial = null;
        }
        else
        {
            Debug.Log("Interstitial is not ready.");
            onClosed?.Invoke(); // fallback: reklam yoksa yine devam et
        }
    }

    public bool IsInterstitialReady()
    {
        return interstitial != null;
    }


    // --- Rewarded (Ödüllü Reklam) ---
    public void RequestRewarded()
    {
#if UNITY_ANDROID
    string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test ID
#elif UNITY_IOS
    string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded failed to load: " + error);
                return;
            }

            rewarded = ad;

            rewarded.OnAdFullScreenContentClosed += () =>
            {
                GameOverManager.Instance.HandleCursor(false); // mouse kilitlenir
                Time.timeScale = 1f; // oyun normale döner
                Debug.Log("Rewarded closed, requesting new one...");
                RequestRewarded();
            };
        });
    }

    public void ShowRewarded(Action onRewardEarned = null)
    {
        if (rewarded != null)
        {
            rewarded.Show((Reward reward) =>
            {
                Debug.Log("Reward earned: " + reward.Amount);
                onRewardEarned?.Invoke(); // ödül burada veriliyor
            });

            rewarded = null;
        }
        else
        {
            Debug.Log("Rewarded ad is not ready.");
        }
    }

}
