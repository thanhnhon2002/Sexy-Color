using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class VipPopup : Popup
    {
        [SerializeField] private LevelListItem   levelListItemPrefab = null;
        [SerializeField] private GridLayoutGroup levelListContainer  = null;
        [SerializeField] private ScrollRect      levelListScrollRect = null;



        private int                              activeCategoryIndex;
        private ObjectPool                       categoryListItemPool;
        private RecyclableListHandler<LevelData> levelListHandler;
        private List<CategoryListItem>           activeCategoryListItems;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            Populate();
        }

        private void Populate()
        {
            if (activeCategoryIndex > GameManager.Instance.Categories.Count)
            {
                return;
            }
            
            // Get the VIP Category
            List<LevelData> levelDatas = GameManager.Instance.Categories.FirstOrDefault(obj => obj.displayName.Equals("VIP"))?.levels;
            
            if(levelDatas == null)
                return;

            // Check if this is the first time we are setting up the library list
            if (levelListHandler == null)
            {
                // Create a new RecyclableListHandler to handle recycling list items that scroll off screen
                levelListHandler = new RecyclableListHandler<LevelData>(levelDatas, levelListItemPrefab, levelListContainer.transform as RectTransform, levelListScrollRect);

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
            OnPremiumButtonClicked();
        }

        public void OnPremiumButtonClicked()
        {
            Hide(true);
        }
    }
}