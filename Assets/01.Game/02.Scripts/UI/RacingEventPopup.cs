using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class RacingEventPopup:Popup
    {
        [SerializeField] private GridLayoutGroup levelListContainer;
        [SerializeField] private LevelListItem levelListItemPrefab;
        [SerializeField] private ScrollRect levelListScrollRect;
        [SerializeField] private int categoryIndex;
        
        private RecyclableListHandler<LevelData> levelListHandler;
        public override void OnShowing(object[] inData)
        {
            ShowEvent();
        }
        
        private void ShowEvent()
        {
            List<LevelData> levelDatas = null;
            var categoryList = GameManager.Instance.Categories;
            levelDatas = categoryList[categoryIndex].levels.Where(level => !GameManager.Instance.AwardedLevels.Contains(level.Id)).ToList();
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
    }
}