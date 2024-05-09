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
        while (time < duration)
        {
            time += Time.deltaTime;
            loadingSlider.value = time / duration;
            yield return null;
        }
        Hide(true);
    }
}
