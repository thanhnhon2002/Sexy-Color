using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using GoogleMobileAds.Api;
//using GoogleMobileAds.Api.Mediation;

public class AdmobNative : AdmobAds
{
    public string NATIVE_ID;
    public NativeAd nativeAds;
    private bool isShowingAds;
    private int retryCount;
    private bool available = false;
    bool success;
    AdLoader adLoader;
    [SerializeField] private RawImage imgIcon;
    [SerializeField] private RawImage imgChoice;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtBody;
    [SerializeField] private TextMeshProUGUI txtAdvertiser;
    //[SerializeField] private TextMeshProUGUI txtCallToAction;

    public GameObject popupNative;
    public GameObject leanTouch;

    private void Update()
    {
        UpdateDisplay();
        //if(Input.GetKeyUp(KeyCode.Escape))
        //{
        //    ShowAds(null);
        //    Debug.Log("call Show AdNative");
        //}
    }

    public void UpdateDisplay()
    {
        if (!available) return;
        available = false;
        if (showDebug) Debug.Log("AdNative Set popup img + text");
        imgIcon.color = Color.white;
        imgChoice.color = Color.white;
        imgIcon.texture = nativeAds.GetIconTexture();
        imgChoice.texture = nativeAds.GetAdChoicesLogoTexture();
        txtTitle.text = nativeAds.GetHeadlineText();
        txtBody.text = nativeAds.GetBodyText();
        txtAdvertiser.text = nativeAds.GetAdvertiserText();
        //txtCallToAction.text = nativeAds.GetCallToActionText();

        success = false;
        success = nativeAds.RegisterIconImageGameObject(imgIcon.gameObject);
        if (showDebug) Debug.LogError($"icon image = {success}");
        success = nativeAds.RegisterAdChoicesLogoGameObject(imgChoice.gameObject);
        if (showDebug) Debug.LogError($"ad choice = {success}");
        success = nativeAds.RegisterHeadlineTextGameObject(txtTitle.gameObject);
        if (showDebug) Debug.LogError($"headline = {success}");
        success = nativeAds.RegisterBodyTextGameObject(txtBody.gameObject);
        if (showDebug) Debug.LogError($"body = {success}");
        success = nativeAds.RegisterAdvertiserTextGameObject(txtAdvertiser.gameObject);
        if (showDebug) Debug.LogError($"advertiser = {success}");
        //success = nativeAds.RegisterCallToActionGameObject(txtCallToAction.gameObject);
        //if (showDebug) Debug.LogError($"call to action = {success}");
    }

    public override void Init()
    {
        // throw new NotImplementedException();
        adLoader = new AdLoader.Builder(NATIVE_ID).ForNativeAd().Build();
        adLoader.OnNativeAdLoaded += OnNativeAdsLoaded;
        adLoader.OnAdFailedToLoad += OnNativeAdFailedToLoad;
    }

    public override void LoadAds()
    {
        Debug.Log("Native Call load ad");
        adLoader.LoadAd(new AdRequest());
    }

    public override bool ShowAds(Action onShowAdsComplete)
    {
        if (nativeAds == null) return false;
        if (showDebug) Debug.Log("Show AdNative");
        leanTouch.SetActive(true);
        popupNative.SetActive(true);
        isShowingAds=true;
        return true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SetIsShowing(bool result)
    {
        isShowingAds = result;
    }
    public override bool IsAdsAvailable()
    {
        return nativeAds==null;
    }

    public override bool IsShowingAds()
    {
        return isShowingAds;
    }
    private void OnNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.LogError("Native ad Load fail");
        if (showDebug) Debug.LogError($"Native ads failed to load: {args.LoadAdError}");
        available = false;
        retryCount++;
        float time = MathF.Pow(2, retryCount);
        if (time > 64) time = 64;
        Invoke(nameof(LoadAds), time);
    }
    private void OnNativeAdsLoaded(object sender, NativeAdEventArgs args)
    {
        Debug.LogError("Native ad loaded.");
        this.nativeAds = args.nativeAd;
        available = true;
        if (showDebug) Debug.Log("AdNative Loaded");
    }
}





