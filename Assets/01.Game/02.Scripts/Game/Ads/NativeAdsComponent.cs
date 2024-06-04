using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class NativeAdsComponent : MonoBehaviour
{
    [SerializeField] GameObject _loadingIndicator;
    [SerializeField] GameObject _dataContainer;


    [SerializeField] RawImage _topIcon;


    [SerializeField] RawImage _adIcon;

    [SerializeField] RawImage _thumbnail;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _desc;
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] Button _button;
    private void Start()
    {
        //ChangeState(true);
    }
    public void ChangeState(bool isLoading)
    {
        _loadingIndicator.SetActive(isLoading);
        _dataContainer.SetActive(!isLoading);
    }
    public void LoadAdData(NativeAd nativeAd)
    {
        //_topIcon.texture = nativeAd.GetAdChoicesLogoTexture();
        //_adIcon.texture =  nativeAd.GetIconTexture();
        _thumbnail.texture = nativeAd.GetImageTextures()[0];
        _title.text = nativeAd.GetHeadlineText();
        _desc.text = nativeAd.GetBodyText();
        _buttonText.text = nativeAd.GetCallToActionText();

        try
        {
            if (!nativeAd.RegisterIconImageGameObject(_adIcon.gameObject))
            {
                Debug.LogError("Cannot Register Icon Ad");
            }
            if(!nativeAd.RegisterAdChoicesLogoGameObject(_topIcon.gameObject))
            {
                Debug.LogError("Cannot Register Ad Choices");
            }
            if (!
            nativeAd.RegisterHeadlineTextGameObject(_title.gameObject))
            {
                Debug.LogError("Cannot Register Ad Headline");
            }
            if (!nativeAd.RegisterBodyTextGameObject(_desc.gameObject))
            {
                Debug.LogError("Cannot Register Ad Body Text");
            }
            if (!nativeAd.RegisterCallToActionGameObject(_button.gameObject))
            {
                Debug.LogError("Cannot Register Ad CTA");
            }
        }
        catch { 
        
        }


        ChangeState(false);
    }
}
