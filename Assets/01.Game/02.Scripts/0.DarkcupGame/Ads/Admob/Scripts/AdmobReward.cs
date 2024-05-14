using GoogleMobileAds.Api;
using System;
using UnityEngine;
using DarkcupGames;

[RequireComponent(typeof(MainThreadScriptRunner))]
public class AdmobReward : AdmobAds
{
    public string REWARD_ID;
    private RewardedAd _rewardedAd;
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
        //throw new NotImplementedException();
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
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        if (showDebug) Debug.Log("Loading the rewarded ad.");
        var adRequest = new AdRequest();

        RewardedAd.Load(REWARD_ID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    if (showDebug) Debug.LogError("error : " + error);
                    available = false;
                    retryCount++;
                    float time = MathF.Pow(2, retryCount);
                    if (time > 64) time = 64;
                    Invoke(nameof(LoadAds), time);
                    return;
                }

                if (showDebug) Debug.Log(ad.GetResponseInfo());
                available = true;
                _rewardedAd = ad;
                _rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    available = false;
                    isShowingAds = false;
                    mainThread.Run(onShowAdsComplete);
                    LoadAds();                   
                };
            });

    }

    public override bool ShowAds(Action onShowAdsComplete)
    {
        this.onShowAdsComplete = onShowAdsComplete;
        const string rewardMsg =
         "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {          
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                if( showDebug) Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
            isShowingAds = true;
            return true;
        }
        else
        {
            isShowingAds = false;
            return false;
        }
    }
}