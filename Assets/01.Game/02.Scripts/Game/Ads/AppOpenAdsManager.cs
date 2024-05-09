using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AppOpenAdsManager
{
    public const string AppOpenAdUnitId = "a17fa31dc26f5ec4";

    private CancellationTokenSource tokenSource;
    private float                   timeBetweenAds = 15f;
    private bool                    canShowOpenAds = true;

    private float  lastTimeShowAds = -1;
    public  Action OnAOAShow;

    public AppOpenAdsManager()
    {
        // TryGoGetAdsConfigurations();
        //MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);

        //MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAdHidden;
        //MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAdLoaded;
        //MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAdLoadedFail;
    }

    //private void OnAdLoadedFail(string arg1, MaxSdkBase.ErrorInfo arg2)
    //{
    //    Debug.Log("AOA ADS LOADED FAILED!!");
    //}

    //private void OnAdLoaded(string arg1, MaxSdkBase.AdInfo arg2)
    //{
    //    Debug.Log("AOA ADS LOADED!");
    //}

    ~AppOpenAdsManager()
    {
        tokenSource?.Cancel();
    }

    private bool IsFirstTimeAppOpen()
    {
        if (PlayerPrefs.GetInt("APP_FIRST_OPEN", 0) == 0)
        {
            PlayerPrefs.SetInt("APP_FIRST_OPEN", 1);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }
    
    public void ShowAdIfReady()
    {
        // Don't show AoA on the first time open the game
        if(IsFirstTimeAppOpen()) return;

        Debug.Log($"DUCK : TRY TO SHOW AOA {lastTimeShowAds} - {canShowOpenAds}");
        // Check if we can show this ad (can be configured from Firebase
        if (!canShowOpenAds) return;

        if (lastTimeShowAds > 0)
        {
            ShowAdsIfNoWaitTime();
            return;
        }

        ShowAdsIfNoWaitTime();
    }

    private void ShowAdsIfNoWaitTime()
    {
        // We have to wait for a while until show the AoA again
        // timeBetweenAds can be configured from Firebase
        if (Time.realtimeSinceStartup - lastTimeShowAds >= timeBetweenAds)
        {
            lastTimeShowAds = Time.realtimeSinceStartup;
            //if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
            //{
            //    Debug.Log($"DUCK : SHOW AOA NOW");
            //    MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
            //}
            //else
            //{
            //    tokenSource?.Cancel();
            //    tokenSource = new();
            //    TryShowingAds(tokenSource.Token);
            //}
        }
    }

    private async Task TryShowingAds( CancellationToken token)
    {
        //while (!MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
        //{
        //    Debug.Log($"DUCK : KEEP TRYING AOA NOW");
        //    await Task.Delay(1000, token);
        //    if (token.IsCancellationRequested) return;
        //}
        
        //if (!token.IsCancellationRequested) MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
    }
    
    private async void TryGoGetAdsConfigurations()
    {
        // while (FireBaseManager.Instance == null)
        // {
        //     await Task.Delay(100);
        // }
        // var timeBetween = (await FireBaseManager.Instance.GetValue("open_ad_capping_time"));
        // var showOpenAds = (await FireBaseManager.Instance.GetValue("open_ad_on_off"));
        //
        // timeBetweenAds = (long)timeBetween;
        // canShowOpenAds = (bool)showOpenAds;
#if DEV_MODE
        timeBetweenAds = 15f;
        canShowOpenAds = true;
#endif

        Debug.Log($"DUCK Fetched : {timeBetweenAds} - {canShowOpenAds}");
    }

    //private void OnAdHidden(string arg1, MaxSdkBase.AdInfo arg2)
    //{
    //    OnAOAShow?.Invoke();
    //    lastTimeShowAds = Time.realtimeSinceStartup;
    //}
}
