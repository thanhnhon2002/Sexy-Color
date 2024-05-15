using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BBG;
using BBG.PictureColoring;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class SuggestionPanel : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup levelListContainer;
    [SerializeField] private LevelListItem levelListItemPrefab;
    [SerializeField] private ScrollRect levelListScrollRect;
    [SerializeField] private int categoryIndex = -1;
    [SerializeField] private bool showOnStart = false;
    

    private RecyclableListHandler<LevelData> levelListHandler;

    private void Start()
    {
        if(showOnStart) Show();
    }

    public void Show()
    {
        List<LevelData> levelDatas = null;
        var categoryList = GameManager.Instance.Categories;//.Where(obj => !obj.isStoryMode).ToList();
        //Debug.Log("Count: " + categoryList.Count);
        var activeCategoryIndex = categoryIndex == -1
            ? GameManager.Instance.ActiveLevelData.GetLevelCategoryIndex()
            : categoryIndex;
        //Debug.Log("Value index: "+activeCategoryIndex);
        
        levelDatas = categoryList[activeCategoryIndex].levels
            .Where(level => !GameManager.Instance.AwardedLevels.Contains(level.Id)).ToList();
        // Check if this is the first time we are setting up the library list
        if (levelListHandler == null)
        {
            // Create a new RecyclableListHandler to handle recycling list items that scroll off screen
            levelListHandler = new RecyclableListHandler<LevelData>(true,levelDatas, levelListItemPrefab,
                levelListContainer.transform as RectTransform, levelListScrollRect);

            levelListHandler.OnListItemClicked = GameManager.Instance.LevelSelected;

            levelListHandler.Setup();
            levelListHandler.Refresh();
        }
        else
        {
            // Update the the RecyclableListHandler with the new data set
            levelListHandler.UpdateDataObjects(true,levelDatas);
        }
    }
}