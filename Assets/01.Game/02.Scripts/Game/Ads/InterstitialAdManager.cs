using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class InterstitialAdManager : MonoBehaviour
{
    const string adUnitId = "c143315d26ec6fee";

    /// <summary>
    /// Duration we have to wait between showing 2 ads
    /// </summary>
    private float timeBetweenAds = 30f;

    private int retryAttempt;

    private bool canShowAds = true;

    private CancellationTokenSource _tokenSource = new();

    private float _lastTimeShowAds = -1;

    public InterstitialAdManager()
    {
#if USE_MAX_SDK
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent        += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent    += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent     += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent       += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent        += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
#endif

        LoadInterstitial();
    }

    private async void OnEnable()
    {
        // var capping_time = await FireBaseManager.Instance.GetValue("inter_ad_capping_time");
        // timeBetweenAds = (float)capping_time;
    }

    private void OnDisable()
    {
        _tokenSource.Cancel();
    }

    public void ResetCappingTime()
    {
        TurnOnAdsAfter(timeBetweenAds);
    }
    

    /// <summary>
    /// Show an interstitial ads if it's ready
    /// </summary>
    public bool ShowInterstitialAds(int level, string placement)
    {
#if USE_MAX_SDK
        if (canShowAds && MaxSdk.IsInterstitialReady(adUnitId))
        {
            MaxSdk.ShowInterstitial(adUnitId, placement);
            canShowAds = false;
            TurnOnAdsAfter(timeBetweenAds);

            // FirebaseAnalytics.LogEvent("ad_inter_show");
            // AppsFlyer.sendEvent("inters_attempt", new());
            // Adjust.trackEvent(new AdjustEvent("adj_inters_ad_eligible"));
            // Adjust.trackEvent(new AdjustEvent("adj_inters_api_called"));
            // Adjust.trackEvent(new AdjustEvent("adj_inters_displayed"));
            return true;
        }
        else
        {
            // FirebaseAnalytics.LogEvent("ad_inter_fail");
        }
#endif

        return false;
    }

    #region private functions

    /// <summary>
    /// Load the interstitial ads. Just load, not show it yet
    /// </summary>
    private void LoadInterstitial()
    {
#if USE_MAX_SDK
        MaxSdk.LoadInterstitial(adUnitId);
#endif
    }


    public async void TurnOnAdsAfter(float delayTime)
    {
        canShowAds =  false;
        delayTime  += 10;
        await Task.Delay((int)delayTime*1000);
        if (_tokenSource.Token.IsCancellationRequested) return;
        canShowAds = true;
    }

    #endregion


    #region Event listeners

#if USE_MAX_SDK
    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        DelayLoadInterstitial((float)retryDelay);
    }

    private async void DelayLoadInterstitial(float delay)
    {
        await Task.Delay(Mathf.RoundToInt(delay * 1000));

        LoadInterstitial();
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // FirebaseAnalytics.LogEvent("af_inters");
        // AppsFlyer.sendEvent("af_inters", new());
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo                                    adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }
#endif

    #endregion
}