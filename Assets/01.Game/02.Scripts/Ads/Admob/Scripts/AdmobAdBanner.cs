using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdmobAdBanner : AdmobAds
{
    public string BANNER_ID;
    public bool useCollapsible = false;
    public AdPosition position = AdPosition.Top;
    private BannerView bannerView;
    private string uuid;
    private bool available;
    private bool isShowingAds;
    private int retryCount;

    public override void Init()
    {
        bannerView = new BannerView(BANNER_ID,AdSize.IABBanner, position);
        bannerView.OnBannerAdLoaded += () =>
        {
            available = true;
            SetBannerVisible(true);
            //CollapsibleBannerFlow.Instance.OnCollapsibleAdsLoaded();
        };
        bannerView.OnBannerAdLoadFailed += (err) =>
        {
            if (showDebug) Debug.LogError("load banner failed");
            if (showDebug) Debug.LogError(err.GetMessage());
            available = false;
            retryCount++;
            float time = MathF.Pow(2, retryCount);
            if (time > 64) time = 64;
            Invoke(nameof(LoadAds), time);
            //CollapsibleBannerFlow.Instance.OnCollapsibleAdsFailed();
        };
        GenerateNewUUID();
    }

    public override void LoadAds()
    {
        if (AdmobManager.Instance.showAds == false) return;
        if (AdmobManager.isReady == false)
        {
            if (showDebug) Debug.LogError("admob is not ready for load banner");
            return;
        }
        var adRequest = new AdRequest();
        if (useCollapsible)
        {
            adRequest.Extras.Add("collapsible", "bottom");
            adRequest.Extras.Add("collapsible_request_id", uuid);
        }
        if (bannerView == null) Init();
        bannerView.LoadAd(adRequest);
    }

    public override bool ShowAds(Action onShowAdsComplete)
    {
        SetBannerVisible(available);
        return available;
    }
    public void SetBannerVisible(bool visible)
    {
        if (bannerView == null) return;
        if (visible)
        {
            bannerView.Show();
            if (showDebug) Debug.Log("Banner Showed");
            isShowingAds = true;
            //FirebaseManager.analytics.LogAdsBannerRecorded("admob", "bottom");
        }
        else
        {
            bannerView.Hide();
            if (showDebug) Debug.Log("Banner Hided");
            isShowingAds = false;
        }
    }

    [ContextMenu("Test")]
    public void GenerateNewUUID()
    {
        uuid = Guid.NewGuid().ToString();
    }

    public override bool IsAdsAvailable()
    {
        return available;
    }

    public override bool IsShowingAds()
    {
        return isShowingAds;
    }
}

