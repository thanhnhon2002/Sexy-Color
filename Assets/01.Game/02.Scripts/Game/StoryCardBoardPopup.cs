using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class StoryCardBoardPopup : Popup
    {
        [SerializeField] private LevelListItem                    levelListItemPrefab         = null;
        [SerializeField] private StoryItemDescription             lStoryItemDescriptionPrefab = null;
        [SerializeField] private GridLayoutGroup                  levelListContainer          = null;
        [SerializeField] private ScrollRect                       levelListScrollRect         = null;
        private                  int                              activeCategoryIndex;
        private                  ObjectPool                       categoryListItemPool;
        private                  RecyclableListHandler<LevelData> levelListHandler;
        private                  List<CategoryListItem>           activeCategoryListItems;
        private                  List<LevelData>                  levelDatas;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            activeCategoryIndex   = inData[0] != null ? (int)inData[0] : 0;
            Populate();
        }

        private void Populate()
        {

            if (activeCategoryIndex > GameManager.Instance.Categories.Count)
            {
                return;
            }

            levelDatas = null;


            levelDatas = GameManager.Instance.Categories[activeCategoryIndex].levels;

            // Check if this is the first time we are setting up the library list
            if (levelListHandler == null)
            {
                // Create a new RecyclableListHandler to handle recycling list items that scroll off screen
                levelListHandler = new RecyclableListHandler<LevelData>(levelDatas, levelListItemPrefab,
                    levelListContainer.transform as RectTransform, levelListScrollRect);
                
                levelListHandler.OnListItemClicked = OnListItemClicked;

                levelListHandler.Setup();
                for (int i = 0; i < levelDatas.Count; i++)
                {
                    var go =Instantiate(lStoryItemDescriptionPrefab, levelListContainer.transform);
                    go.transform.SetSiblingIndex(GetSequenceElement(i));
                    go.GetComponent<StoryItemDescription>().Populate(levelDatas[i]);
                }   
            }
            else
            {
                // Update the the RecyclableListHandler with the new data set
                levelListHandler.UpdateDataObjects(levelDatas);
            }
        }
        static int GetSequenceElement(int index)
        {
            int multiplier = index / 2;
            int baseValue  = (index % 2 == 0) ? 2 : 3;

            return baseValue + (4 * multiplier);
        }
        private void OnListItemClicked(LevelData data)
        {
            for (int i = 0; i < levelDatas.Count; i++)
            {
                if (i >0 &&levelDatas[i].Id == data.Id && !GameManager.Instance.AwardedLevels.Contains(levelDatas[i-1].Id))
                {
                    Debug.Log("Level is locked");
                    return;
                }
            }
            this.Hide(true);
            GameManager.Instance.LevelSelected(data);
        }
    }
}