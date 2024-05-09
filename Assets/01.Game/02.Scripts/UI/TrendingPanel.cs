using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BBG.PictureColoring
{
    public class TrendingPanel : MonoBehaviour
    {
        [SerializeField] private Transform trendingHolder;
        [SerializeField] private TrendingLevelItem levelItem;
        [SerializeField] private int trendingCategoryIndex;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            List<LevelData> levelDatas = null;
            var categoryList = GameManager.Instance.Categories.Where(obj => !obj.isStoryMode).ToList();
            levelDatas = categoryList[trendingCategoryIndex].levels
                .Where(level => !GameManager.Instance.AwardedLevels.Contains(level.Id)).ToList();

            foreach (var level in levelDatas)
            {
                var item = Instantiate(levelItem, trendingHolder).GetComponent<TrendingLevelItem>();
                item.Populate(level);
                item.OnTrendingLevelClickedEvent.AddListener(OnItemClicked);
            }
        }

        private void OnItemClicked(LevelData data)
        {
            GameManager.Instance.LevelSelected(data);
        }
    }
}