using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class LibraryScreen : Screen
    {
        #region Inspector Variables

        [Space] [SerializeField] private CategoryListItem categoryListItemPrefab = null;
        [SerializeField]         private Transform        categoryListContainer  = null;

        [Space] [SerializeField] private LevelListItem   levelListItemPrefab = null;
        [SerializeField]         private LayoutGroup levelListContainer  = null;
        [SerializeField]         private ScrollRect      levelListScrollRect = null;

        #endregion

        #region Member Variables

        private ObjectPool                       categoryListItemPool;
        private RecyclableListHandler<LevelData> levelListHandler;
        private List<CategoryListItem>           activeCategoryListItems;
        private List<CategoryData>               categoryList;
        private int                              activeCategoryIndex;

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();

            activeCategoryListItems = new List<CategoryListItem>();
            categoryListItemPool    = new ObjectPool(categoryListItemPrefab.gameObject, 1, categoryListContainer);

            // Set the cells size based on the width of the screen
            // Utilities.SetGridCellSize(levelListContainer);

            SetupCategoryList();
            SetupLibraryList();

            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelPlayedEvent,    OnLevelGameEvent);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelCompletedEvent, OnLevelGameEvent);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelProgressDeletedEvent,
                OnLevelGameEvent);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelUnlockedEvent, OnLevelGameEvent);
        }

        public override void OnShowing()
        {
            if (levelListHandler != null)
            {
                // levelListHandler.Refresh();
            }
        }

        #endregion

        #region Private Methods

        private void OnLevelGameEvent(string id, object[] data)
        {
            // Call the Setup method on all visible LevelListItems
            // levelListHandler.Refresh();
            SetupLibraryList();
        }

        private void SetupCategoryList()
        {
            categoryListItemPool.ReturnAllObjectsToPool();
            activeCategoryListItems.Clear();

            categoryList = GameManager.Instance.Categories.Where(obj => !obj.isStoryMode).ToList();
            for (int i = 0; i < categoryList.Count; i++)
            {
                CategoryListItem categoryListItem = categoryListItemPool.GetObject<CategoryListItem>();

                activeCategoryListItems.Add(categoryListItem);

                categoryListItem.Setup(categoryList[i].displayName,
                    categoryList[i].isVIP);

                // Set all list items to not selected at the beginning
                categoryListItem.SetSelected(false);

                // Setup click index and listener
                categoryListItem.Index             = i;
                categoryListItem.OnListItemClicked = OnCategoryListItemSelected;
            }

            // Set the CategoryListItem as selected
            SetCategoryListItemSelected(0);
        }

        /// <summary>
        /// Invoked when a CategoryListItem is clicked
        /// </summary>
        private void OnCategoryListItemSelected(int index, object data)
        {
            if (activeCategoryIndex != index)
            {
                // Set the CategoryListItem as selected
                SetCategoryListItemSelected(index);

                // Setup the library list for the new selected category
                SetupLibraryList();
            }
        }

        /// <summary>
        /// Sets the given category list item index as the selected index
        /// </summary>
        private void SetCategoryListItemSelected(int index)
        {
            // Set the current categoy list item to not selected
            if (activeCategoryIndex >= 0 && activeCategoryIndex < activeCategoryListItems.Count)
            {
                activeCategoryListItems[activeCategoryIndex].SetSelected(false);
            }

            // Set the new category list item to selected
            if (index >= 0 && index < activeCategoryListItems.Count)
            {
                activeCategoryListItems[index].SetSelected(true);

                activeCategoryIndex = index;
            }
        }

        /// <summary>
        /// Clears then resets the list of library level items using the current active category index
        /// </summary>
        private void SetupLibraryList()
        {
            if (activeCategoryIndex > categoryList.Count)
            {
                return;
            }

            List<LevelData> levelDatas = null;


            levelDatas = categoryList[activeCategoryIndex].levels;
            // Check if this is the first time we are setting up the library list
            // if (levelListHandler == null)
            // {
            //     // Create a new RecyclableListHandler to handle recycling list items that scroll off screen
            //     levelListHandler = new RecyclableListHandler<LevelData>(levelDatas, levelListItemPrefab,
            //         levelListContainer.transform as RectTransform, levelListScrollRect);
            //
            //     levelListHandler.OnListItemClicked = GameManager.Instance.LevelSelected;
            //
            //     levelListHandler.Setup();
            // }
            // else
            // {
            //     // Update the the RecyclableListHandler with the new data set
            // levelListHandler.UpdateDataObjects(levelDatas);
            // }
            levelListContainer.transform.MMDestroyAllChildren();
            foreach (var levelData in levelDatas)
            {
                var levelListItem = Instantiate(levelListItemPrefab, levelListContainer.transform).GetComponent<LevelListItem>();
                levelListItem.Setup(levelData);
                levelListItem.OnListItemClicked = OnItemClicked;
            }
        }

        private void OnItemClicked(int arg1, object arg2)
        {
            GameManager.Instance.LevelSelected((LevelData)arg2);
        }

        #endregion
    }
}