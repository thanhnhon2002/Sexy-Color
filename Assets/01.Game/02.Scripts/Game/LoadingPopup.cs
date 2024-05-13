using System.Collections;
using System.Collections.Generic;
using BBG;
using BBG.PictureColoring;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPopup : Popup
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Image loadingImage;
    public bool isFirstLoad;

    private LevelData _levelData;
    public override void OnShowing(object[] inData)
    {
        base.OnShowing(inData);
        int duration = (int)inData[0];
        if (inData.Length > 1)
        {
            _levelData = (LevelData)inData[1];
            if (_levelData != null)
            {
                loadingImage.sprite = _levelData.levelImage;
            }
        }
        StartCoroutine(Loading(duration));
    }
    
    private IEnumerator Loading(int duration)
    {
        float time = 0f;
        if (isFirstLoad) duration += 1;
        while (time < duration)
        {
            time += Time.deltaTime;
            //if(time>duration/2) 
            loadingSlider.value = time / duration;
            yield return null;
        }       
        if (isFirstLoad)
        {
            AdmobManager.Instance.appOpen.ShowAds(()=> 
            {
                Hide(true);
                Debug.Log("Closed AppOpen");
            });
            isFirstLoad = false;
        }
        else Hide(true);
    }
}
