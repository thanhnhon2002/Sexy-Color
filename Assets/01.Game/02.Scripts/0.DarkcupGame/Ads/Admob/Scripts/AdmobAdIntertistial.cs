using GoogleMobileAds.Api;
using System;
using UnityEngine;
using DarkcupGames;

[RequireComponent(typeof(MainThreadScriptRunner))]
public class AdmobAdInterstitial : AdmobAds
{
    public string INTERTISTIAL_ID;
    private InterstitialAd _interstitialAd;
    private bool available;
    private bool isShowingAds;
    private int retryCount;
    private MainThreadScriptRunner mainThread;
    private Action onShowAdsComplete;

    private void Awake()
    {
        mainThread = GetComponent<MainThreadScriptRunner>();
    }
    public override void Init()
    {
        //move to load ads
    }

    public override bool IsAdsAvailable()
    {
        return available;
    }

    public override bool IsShowingAds()
    {
        return isShowingAds;
    }

    public override void LoadAds()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
        if (AdmobManager.isReady == false)
        {
            if (showDebug) Debug.LogError("admob is not ready for load Interstitial");
            return;
        }
        if (showDebug) Debug.Log("Loading the interstitial ad.");

        var adRequest = new AdRequest();
        InterstitialAd.Load(INTERTISTIAL_ID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError(error);
                available = false;
                retryCount++;
                float time = MathF.Pow(2, retryCount);
                if (time > 64) time = 64;
                Invoke(nameof(LoadAds), time);
                return;
            }
            if (showDebug) Debug.Log(ad.GetResponseInfo());
            available = true;
            _interstitialAd = ad;
            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                available = false;
                isShowingAds = false;
                LoadAds();                          
                mainThread.Run(onShowAdsComplete); 
            };
        });
    }

    public override bool ShowAds(Action onShowAdsComplete)
    {
        this.onShowAdsComplete = onShowAdsComplete;
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            if (showDebug) Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
            isShowingAds = true;
            return true;
        }
        else
        {
            if (showDebug) Debug.LogError("Interstitial ad is not ready yet.");
            isShowingAds = false;
            return false;
        }
    }
}
