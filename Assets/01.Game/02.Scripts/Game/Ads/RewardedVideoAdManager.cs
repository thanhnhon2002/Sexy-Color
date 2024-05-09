using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RewardedVideoAdManager
{
    const string adUnitId = "c13c049de7f2ae19";
    int          retryAttempt;

    public Action onRewardCallback;
    
    public RewardedVideoAdManager() 
    {
#if USE_MAX_SDK
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
                
        // Load the first rewarded ad
        LoadRewardedAd();
#endif
    }

    public bool ShowRewaredVideo(Action onRewardCallback)
    {
#if USE_MAX_SDK
        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
            this.onRewardCallback = onRewardCallback;
            MaxSdk.ShowRewardedAd(adUnitId);
            
            // FirebaseAnalytics.LogEvent("ads_reward_show");
            // AppsFlyer.sendEvent("reward_attempt", new());
            
            // Adjust.trackEvent(new AdjustEvent("adj_rewarded_ad_eligible"));
            // Adjust.trackEvent(new AdjustEvent("adj_rewarded_api_called"));
            // Adjust.trackEvent(new AdjustEvent("adj_rewarded_ad_displayed"));
            return true;
        }
        else
        {
            // FirebaseAnalytics.LogEvent("ads_reward_fail");
        }
#endif
        this.onRewardCallback = onRewardCallback;
        return AdmobManager.Instance.reward.ShowAds(null);
    }

    private void LoadRewardedAd()
    {
#if USE_MAX_SDK
        MaxSdk.LoadRewardedAd(adUnitId);
#endif
        AdmobManager.Instance.reward.LoadAds();
    }

    #region Event Listeners
#if USE_MAX_SDK
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        UnityEngine.Debug.Log("Rewarded Ad is ready");
        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        DelayReloadAds((float)retryDelay);
    }

    private async void DelayReloadAds(float delay)
    {
        await Task.Delay(Mathf.RoundToInt(delay));
        
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // FirebaseAnalytics.LogEvent("af_reward");
        // AppsFlyer.sendEvent("af_reward", new());
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.  
        onRewardCallback?.Invoke();
        onRewardCallback = null;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
#endif
    #endregion
}
