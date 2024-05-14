using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BBG.PictureColoring
{
    [System.Serializable]
    public class LevelData
    {
        #region Inspector Variables

        public TextAsset levelFile;
        public int       coinsToAward;
        public bool      locked;
        public int       coinsToUnlock;
        public Sprite    levelImage;
        public Sprite    levelImagePreview;
        public int       hintsToAward;
        public string    displayName;
        public string    description;

        #endregion

        #region Member Variables

        private bool   levelFileParsed;
        private string id;
        private string assetPath;

        #endregion

        #region Properties

        public string Id
        {
            get
            {
                if (!levelFileParsed)
                {
                    ParseLevelFile();
                }

                return id;
            }
        }

        public string AssetPath
        {
            get
            {
                if (!levelFileParsed)
                {
                    ParseLevelFile();
                }

                return assetPath;
            }
        }

        public LevelSaveData LevelSaveData
        {
            get { return GameManager.Instance.GetLevelSaveData(Id); }
        }

        /// <summary>
        /// Gets the level file data, should only call this if you know the level has been loaded
        /// </summary>
        public LevelFileData LevelFileData
        {
            get { return LoadManager.Instance.GetLevelFileData(Id); }
        }

        #endregion

        #region Public Methods

        public bool IsColorComplete(int colorIndex)
        {
            if (LevelFileData == null)
            {
                Debug.LogError("[LevelData] IsColorRegionComplete | LevelFileData has not been loaded.");

                return false;
            }

            if (colorIndex < 0 || colorIndex >= LevelFileData.regions.Count)
            {
                Debug.LogErrorFormat(
                    "[LevelData] IsColorComplete | Given colorIndex ({0}) is out of bounds for the regions list of size {1}.",
                    colorIndex, LevelFileData.regions.Count);

                return false;
            }

            LevelSaveData levelSaveData = LevelSaveData;
            List<Region>  regions       = LevelFileData.regions;

            for (int i = 0; i < regions.Count; i++)
            {
                Region region = regions[i];

                if (region.colorIndex == colorIndex && !levelSaveData.coloredRegions.Contains(region.id))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the percentage of the level that has been completed
        /// </summary>
        /// <returns></returns>
        public float LevelCompletePercentage()
        {
            return LevelSaveData.coloredRegions.Count / (float)LevelFileData.regions.Count;
        }

        public float LevelRegionColoredPercentage(int colorIndex)
        {
            var           totalCount    = 0;
            var           coloredCount  = 0;
            var levelSaveData = LevelSaveData;
            var  regions       = LevelFileData.regions;
            foreach (var region in regions)
            {
                if (colorIndex == region.colorIndex)
                {
                    totalCount++;
                    
                    if (levelSaveData.coloredRegions.Contains(region.id))
                    {
                        coloredCount++;
                    }
                }

            }

            return (float)coloredCount/totalCount;
        }

        /// <summary>
        /// Checks if all regions have been colored
        /// </summary>
        public bool AllRegionsColored()
        {
            if (LevelFileData == null)
            {
                Debug.LogError("[LevelData] AllRegionsColored | LevelFileData has not been loaded.");

                return false;
            }

            LevelSaveData levelSaveData = LevelSaveData;
            List<Region>  regions       = LevelFileData.regions;

            for (int i = 0; i < regions.Count; i++)
            {
                Region region = regions[i];

                if (region.colorIndex > -1 && !levelSaveData.coloredRegions.Contains(region.id))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a random region index in the given ColorRegion that has not been colored in
        /// </summary>
        public int GetSmallestUncoloredRegion(int colorIndex)
        {
            if (LevelFileData == null)
            {
                Debug.LogError("[LevelData] GetRandomUncoloredRegion | LevelFileData has not been loaded.");

                return -1;
            }

            if (colorIndex < 0 || colorIndex >= LevelFileData.regions.Count)
            {
                Debug.LogErrorFormat(
                    "[LevelData] GetRandomUncoloredRegion | Given colorRegionIndex ({0}) is out of bounds for the colorRegions list of size {1}.",
                    colorIndex, LevelFileData.regions.Count);

                return -1;
            }

            LevelSaveData levelSaveData = LevelSaveData;
            List<Region>  regions       = LevelFileData.regions;

            int minRegionSize = int.MaxValue;
            int index         = -1;

            for (int i = 0; i < regions.Count; i++)
            {
                Region region = regions[i];

                if (colorIndex == region.colorIndex && !levelSaveData.coloredRegions.Contains(region.id))
                {
                    if (minRegionSize > region.numberSize)
                    {
                        minRegionSize = region.numberSize;
                        index         = i;
                    }
                }
            }

            return index;
        }

        public int GetLevelCategoryIndex()
        {
            //return GetLevelCategoryIndexDG();
            for (var i = 0; i < GameManager.Instance.Categories.Count; i++)
            {
                var category = GameManager.Instance.Categories[i];
                foreach (var level in category.levels)
                {
                    if (level == this) return i;
                }
            }

            return -1;
        }
        public int GetLevelCategoryIndexDG()
        {
            var categoryList = GameManager.Instance.Categories.Where(obj => !obj.isStoryMode).ToList();
            for (var i = 0; i < categoryList.Count; i++)
            {
                var category = categoryList[i];
                foreach (var level in category.levels)
                {
                    if (level == this) return i;
                }
            }
            return -1;
        }
        #endregion

        #region Private Methods

        private void ParseLevelFile()
        {
            string[] fileContents = levelFile.text.Split('\n');

            if (fileContents.Length != 2)
            {
                Debug.LogError(levelFile.name);
            }

            id        = fileContents[0].Trim();
            assetPath = fileContents[1].Trim();

            levelFileParsed = true;
        }

        #endregion
    }
}