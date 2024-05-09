using System;
using System.Collections.Generic;
using BBG;
using BBG.PictureColoring;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscoverCategoryListItem : MonoBehaviour,MMEventListener<MMGameEvent>
{
    [SerializeField] private Button          button;
    [SerializeField] private TextMeshProUGUI categoryNameText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image           categoryImage;
    [SerializeField] private Slider          categoryProgressSlider;


    private int          _categoryIndex = 1;
    private CategoryData _data;

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClicked);
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void Populate(CategoryData data)
    {
        _data                        = data;
        for (int i = 0; i < GameManager.Instance.Categories.Count; i++)
        {
            if (data.displayName.Equals(GameManager.Instance.Categories[i].displayName))
            {
                _categoryIndex = i;
                break;
            }
        }
        LoadData();
    }

    public void LoadData()
    {
        categoryNameText.text        = _data.displayName;
        progressText.text            = $"{GetProgress(_data, false)}/{_data.levels.Count}";
        categoryImage.sprite         = _data.categoryImage;
        categoryProgressSlider.value = GetProgress(_data);
        if(descriptionText != null)
            descriptionText.text = _data.description;
    }
    
    // Return finished levels progress in this category
    public float GetProgress(CategoryData data, bool needPercentage = true)
    {
        float progress = 0f;

        for (int i = 0; i < data.levels.Count; i++)
        {
            LevelData levelData = data.levels[i];
            
            GameManager.Instance.GetMyWorksLevelDatas(out List<LevelData> listLevelCompleted, out List<LevelData> listLevelOnGoing, out List<LevelData> listFavourite);

            if (listLevelCompleted.Contains(levelData))
            {
                progress += 1f;
            }
        }

        return needPercentage ? progress / data.levels.Count : progress;
    }
    private void OnButtonClicked()
    {
        PopupManager.Instance.Show(_data.isStoryMode ? "storyboard_selected" : "category_selected",
            new object[] { _categoryIndex, categoryProgressSlider.value, progressText.text, categoryNameText.text });
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        if (eventType.EventName.Equals("LevelFinished"))
        {
            LoadData();
        }
    }
}