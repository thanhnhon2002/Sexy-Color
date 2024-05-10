using UnityEngine;

public class BannerAdManager
{
    const string bannerAdUnitId = "09999e98ef692d7b"; // Retrieve the ID from your account

    public BannerAdManager()
    {
//#if USE_MAX_SDK
//        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
//        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
//        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.TopCenter);

//        // Set background or background color for banners to be fully functional
//        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.clear);

//        MaxSdkCallbacks.Banner.OnAdLoadedEvent      += OnBannerAdLoadedEvent;
//        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent  += OnBannerAdLoadFailedEvent;
//        MaxSdkCallbacks.Banner.OnAdClickedEvent     += OnBannerAdClickedEvent;
//        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
//        MaxSdkCallbacks.Banner.OnAdExpandedEvent    += OnBannerAdExpandedEvent;
//        MaxSdkCallbacks.Banner.OnAdCollapsedEvent   += OnBannerAdCollapsedEvent;
//#endif
    }

    public void ShowBanner()
    {
        //#if USE_MAX_SDK
        //        if(MaxMediationWrapper.Instance.IsAdActivated == false) return;
        //        MaxSdk.SetBannerWidth(bannerAdUnitId, Screen.width);
        //        MaxSdk.StartBannerAutoRefresh(bannerAdUnitId);
        //        MaxSdk.ShowBanner(bannerAdUnitId);
        //        RefreshBanner();
        //#endif
        AdmobManager.Instance.banner.SetBannerVisible(true);
    }

    public void RefreshBanner()
    {
        //AdmobManager.banner.LoadAds();
        //MaxSdk.LoadBanner(bannerAdUnitId);
    }
    public void HideBanner()
    {
        AdmobManager.Instance.banner.SetBannerVisible(false);
        //MaxSdk.HideBanner(bannerAdUnitId);
    }

    #region Event Listeners
//#if USE_MAX_SDK
//    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

//    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {}

//    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

//    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

//    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)  {}

//    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {}
//#endif
    #endregion
    
}