using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine;

public class MaxMediationWrapper : MMPersistentSingleton<MaxMediationWrapper>
{
    public bool IsAdActivated
    {
        get
        {
            if(PlayerPrefs.GetInt("RemoveAds", 0) == 1)
                return false;
            if (!CheckWeeklySubscription()) return false;
            if (!CheckMonthlySubscription()) return false;
#if USE_MAX_SDK
            return true;
#else
            return false;
#endif
        }
    }

    private static bool CheckMonthlySubscription()
    {
        if (PlayerPrefs.GetInt("MonthlySubscription", 0) == 1)
        {
            var startDay = PlayerPrefs.GetInt("MonthlySubscription_Start", 0);
            if (startDay != 0)
            {
                var currentDay = System.DateTime.Now.DayOfYear;
                if (currentDay - startDay < 30)
                    return false;
                PlayerPrefs.SetInt("MonthlySubscription", 0);
                PlayerPrefs.SetInt("MonthlySubscription_Start", 0);
                PlayerPrefs.Save();
            }
        }

        return true;
    }

    private static bool CheckWeeklySubscription()
    {
        if (PlayerPrefs.GetInt("WeeklySubscription", 0) == 1)
        {
            var startDay = PlayerPrefs.GetInt("WeeklySubscription_Start", 0);
            if (startDay != 0)
            {
                var currentDay = System.DateTime.Now.DayOfYear;
                if (currentDay - startDay < 7)
                    return false;
                PlayerPrefs.SetInt("WeeklySubscription", 0);
                PlayerPrefs.SetInt("WeeklySubscription_Start", 0);
                PlayerPrefs.Save();
            }
        }

        return true;
    }

    [SerializeField] private string SDK_KEY;
    /// <summary>
    /// This is for internal user id. Probably just for testing or some internal features.
    /// We can leave it as empty/null
    /// </summary>
    [SerializeField] private string USER_ID;

    public  float                  cappingRewardAds = 5f;
    
    public  AppOpenAdsManager      appOpenAdsManager;
    public  BannerAdManager        bannerAdManager;
    public  InterstitialAdManager  interstitialAdManager;
    public  RewardedVideoAdManager rewardedAdVideoManager;
    private float                  lastTimePlayRewardAds; 

    private void OnEnable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UnityEngine.Debug.Log("Error. Check internet connection!");
        }
        else
            //InitializeMaxSdk();
            InitializeAdsManagers();



    }

    public void ResetInterCappingTime()
    {
        interstitialAdManager.TurnOnAdsAfter(5f);
    }

    #region public functions

    public void ShowInterstitital(int level, string placement)
    {
        if (IsAdActivated)
        {
            if(Time.timeSinceLevelLoad - lastTimePlayRewardAds > cappingRewardAds)
                interstitialAdManager.ShowInterstitialAds(level, placement);
        }
    }

    public void ShowRewardAd(Action onRewardCallback)
    {
        // If ad is activated, We show the ads
        if (IsAdActivated)
        {
            if (rewardedAdVideoManager.ShowRewaredVideo(onRewardCallback))
            {
                lastTimePlayRewardAds = Time.timeSinceLevelLoad;
            }
        }
        else onRewardCallback?.Invoke();
    }

    #endregion

    #region private functions

    /// <summary>
    /// Instantiate all type of ads manager (Interstitial, Banners, etc)
    /// </summary>
    private void InitializeAdsManagers()
    {
        appOpenAdsManager = new();
        bannerAdManager = new();
        interstitialAdManager = new();
        rewardedAdVideoManager = new();
        
        // We reset the capping time of Interstitial when User watch AOA or Reward Ads
        rewardedAdVideoManager.onRewardCallback += ResetInterCappingTime;
        appOpenAdsManager.OnAOAShow             += ResetInterCappingTime;
    }

    private void OnDisable()
   {
//#if USE_MAX_SDK
//        rewardedAdVideoManager.onRewardCallback -= ResetInterCappingTime;
//        appOpenAdsManager.OnAOAShow             -= ResetInterCappingTime;
//#endif
    }

    private async void InitializeMaxSdk()
    {
//#if USE_MAX_SDK
        // while (FireBaseManager.Instance == null || !FireBaseManager.Instance.IsReady)
        // {
        //     await Task.Delay(100);
        // }
        
        // Listen to SDK events
        //MaxSdkCallbacks.OnSdkInitializedEvent += OnSDKInitialized;
        // MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += 
        //MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAdHidden;

        // Set SDK key
        //MaxSdk.SetSdkKey(SDK_KEY);
        //if (!string.IsNullOrEmpty(USER_ID)) MaxSdk.SetUserId(USER_ID);

        // Then start
        //MaxSdk.InitializeSdk();

        // Track ad revenue event
        //TrackAdRevenueEvents();
//#endif
    }
//#if USE_MAX_SDK
    //private void TrackAdRevenueEvents()
    //{
    //    MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
    //    MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent     += OnAdRevenuePaidEvent;
    //    MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent       += OnAdRevenuePaidEvent;
    //    MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent         += OnAdRevenuePaidEvent;
    //}
//#endif

    #endregion

    #region Event Listeners
//#if USE_MAX_SDK
//    private void OnSDKInitialized(MaxSdkBase.SdkConfiguration configuration)
//    {
//        // Load the ads here!!
//#if UNITY_ANDROID || UNITY_IOS
//        UnityEngine.Debug.Log("Show Debugger Max Mediation");
//       // MaxSdk.ShowMediationDebugger();
//#endif
//        UnityEngine.Debug.Log("Max Mediation ready");
//        InitializeAdsManagers();

//        if (IsAdActivated)
//        {
//            bannerAdManager.ShowBanner();
//            appOpenAdsManager?.ShowAdIfReady();
//        }
//    }



//    private void OnAdHidden(string adsUnitID, MaxSdkBase.AdInfo adInfo)
//    {

//    }

//    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//    {
//        MaxSdk.LoadAppOpenAd(AppOpenAdsManager.AppOpenAdUnitId);
//    }

//    private void OnApplicationPause(bool pauseStatus)
//    {
//        if (!pauseStatus)
//        {
//            if(IsAdActivated)
//                appOpenAdsManager?.ShowAdIfReady();
//        }
//    }

//    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
//    {
//        // double revenue = impressionData.Revenue;
//        // var impressionParameters = new[] {
//        //     new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
//        //     new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
//        //     new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
//        //     new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
//        //     new Firebase.Analytics.Parameter("value", revenue),
//        //     new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
//        // };
//        // Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
//    }
//#endif
    #endregion
}
