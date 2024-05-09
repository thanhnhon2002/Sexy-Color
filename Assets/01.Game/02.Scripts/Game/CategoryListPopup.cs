using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class CategoryListPopup : Popup
    {
        [SerializeField] private LevelListItem   levelListItemPrefab = null;
        [SerializeField] private GridLayoutGroup levelListContainer  = null;
        [SerializeField] private ScrollRect      levelListScrollRect = null;

        [SerializeField] private TextMeshProUGUI categoryNameText = null;
        [SerializeField] private Slider          slider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image categoryImage;
        


        private int                              activeCategoryIndex;
        private ObjectPool                       categoryListItemPool;
        private RecyclableListHandler<LevelData> levelListHandler;
        private List<CategoryListItem>           activeCategoryListItems;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            activeCategoryIndex   = inData[0] != null ? (int)inData[0] : 0;
            slider.value          = (float)inData[1];
            progressText.text     = inData[2].ToString();
            categoryNameText.text = inData[3].ToString();
            Populate();
        }

        private void Populate()
        {
            if (activeCategoryIndex > GameManager.Instance.Categories.Count)
            {
                return;
            }

            List<LevelData> levelDatas = null;


            levelDatas = GameManager.Instance.Categories[activeCategoryIndex].levels;

            categoryImage.sprite = GameManager.Instance.Categories[activeCategoryIndex].categoryImage;
            
            // Check if this is the first time we are setting up the library list
            if (levelListHandler == null)
            {
                // Create a new RecyclableListHandler to handle recycling list items that scroll off screen
                levelListHandler = new RecyclableListHandler<LevelData>(levelDatas, levelListItemPrefab,
                    levelListContainer.transform as RectTransform, levelListScrollRect);

                levelListHandler.OnListItemClicked = OnListItemClicked;

                levelListHandler.Setup();
            }
            else
            {
                // Update the the RecyclableListHandler with the new data set
                levelListHandler.UpdateDataObjects(levelDatas);
            }
        }

        private void OnListItemClicked(LevelData data)
        {
            this.Hide(true);
            GameManager.Instance.LevelSelected(data);
        }
    }
}