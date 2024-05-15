using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using MoreMountains.Tools;

using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class GameManager : SaveableManager<GameManager>
    {
        [SerializeField] public int REWARD_POPUP_INTERVAL = 180;
        #region Inspector Variables

        [Header("Data")][SerializeField] private List<CategoryData> categories = null;
        [SerializeField] private List<SuperCategoryData> superCategories;

        [Header("Values")][SerializeField] private bool awardHints = false;
        [SerializeField] private int numLevelsBetweenAds = 0;

        [Header("Gift Milestone")]
        [SerializeField]
        private float giftMilestoneValue = .7f;

        private int _splashLoadingTime = 6;
        #endregion

        #region Member Variables

        private List<LevelData> allLevels;
        private int numLevelsStarted;

        // Contains all LevelSaveDatas which have been requested but the level has yet to be colored (This is not saved to file)
        private Dictionary<string, LevelSaveData> levelSaveDatas;

        // Contains all LevelSaveDatas which have atleast one region colored in but have not been completed yet
        private Dictionary<string, LevelSaveData> playedLevelSaveDatas;

        /// <summary>
        /// Contains all level ids which have been completed by the player
        /// </summary>
        private HashSet<string> unlockedLevels;

        /// <summary>
        /// Levels that have been completed atleast one and the player has been awarded the coins/hints
        /// </summary>
        private HashSet<string> awardedLevels;

        public HashSet<string> AwardedLevels
        {
            get => awardedLevels;
            set => awardedLevels = value;
        }

        public bool IsSelecting;
        public List<LevelData> SelectedLevels = new List<LevelData>();

        #endregion

        #region Properties

        public override string SaveId
        {
            get { return "game_manager"; }
        }

        public float GiftMilestoneValue => giftMilestoneValue;

        public List<CategoryData> Categories
        {
            get { return categories; }
        }

        public List<SuperCategoryData> SuperCategories
        {
            get
            {
                foreach (var superCategory in superCategories)
                {
                    superCategory.categoryDatas.Clear();
                    foreach (var categoryName in superCategory.categories)
                    {
                        if (categories.Any(obj => obj.displayName.Equals(categoryName)))
                        {
                            superCategory.categoryDatas.Add(categories.FirstOrDefault(obj =>
                                obj.displayName.Equals(categoryName)));
                        }
                    }
                }

                return superCategories;
            }
        }

        public LevelData ActiveLevelData{ get; private set; }
        private void Update()
        {
            //if (ActiveLevelData == null) Debug.Log("Nulllllllll");
            //else
            //Debug.Log(GameManager.Instance.ActiveLevelData == null);
        }
        public void SetActiveLevelData(LevelData leveldata)
        {
            ActiveLevelData = leveldata;
        }

        public List<LevelData> AllLevels
        {
            get
            {
                if (allLevels == null)
                {
                    allLevels = new List<LevelData>();

                    for (int i = 0; i < categories.Count; i++)
                    {
                        allLevels.AddRange(categories[i].levels);
                    }
                }

                return allLevels;
            }
        }

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 300;
            playedLevelSaveDatas = new Dictionary<string, LevelSaveData>();
            levelSaveDatas = new Dictionary<string, LevelSaveData>();
            awardedLevels = new HashSet<string>();
            unlockedLevels = new HashSet<string>();

            InitSave();

            ScreenManager.Instance.OnSwitchingScreens += OnSwitchingScreens;
            UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("Tutorial_Finished", 0) != 0) _splashLoadingTime = 3;
            PopupManager.Instance.Show("splash_loading", new object[] { _splashLoadingTime },
                (bool cancelled, object[] outData) =>
                {
                    //MaxMediationWrapper.Instance.bannerAdManager.HideBanner();
                    if (PlayerPrefs.GetInt("Tutorial_Finished", 0) == 0)
                    {
                        PopupManager.Instance.Show("tutorial", new object[] { 1 },
                            (bool cancelled, object[] outData) =>
                            {
                                PlayerPrefs.SetInt("Tutorial_Finished", 1);
                                //MaxMediationWrapper.Instance.bannerAdManager.ShowBanner();
                            });
                    }
                });
            ShowRewardAfterDuration(REWARD_POPUP_INTERVAL);
        }

        private void ShowRewardAfterDuration(float duration)
        {
            DOVirtual.DelayedCall(duration, () =>
            {
                PopupManager.Instance.Show("reward_popup");
                ShowRewardAfterDuration(duration);
            });
        }

        #endregion

        public Action OnGameChanged;

        #region Public Methods

        /// <summary>
        /// Shows the level selected popup
        /// </summary>
        public void LevelSelected(LevelData levelData)
        {
            if (IsSelecting)
            {
                if (!SelectedLevels.Contains(levelData))
                {
                    SelectedLevels.Add(levelData);
                    Debug.Log($"Add Level {levelData.Id} - SelectedLevels.Count: " + SelectedLevels.Count);
                }
                else
                {
                    SelectedLevels.Remove(levelData);
                    Debug.Log($"Remove Level {levelData.Id} - SelectedLevels.Count: " + SelectedLevels.Count);
                }

                MMEventManager.TriggerEvent(new SelectImageEvent(levelData));
                return;
            }

            bool isLocked = levelData.locked && !levelData.LevelSaveData.isUnlocked;

#if !CHEAT_APK
            if (isLocked && PlayerPrefs.GetInt("WeeklySubscription", 0) == 0 && PlayerPrefs.GetInt("MonthlySubscription", 0) == 0)
            {
                PopupManager.Instance.Show("unlock-popup", new object[] { levelData },
                    (bool cancelled, object[] outData) =>
                    {
                        if (levelData.LevelSaveData.isUnlocked)
                        {
                            StartLevel(levelData);
                        }
                    });
                return;
            }
#endif

            var isStoryMode = false;
            foreach (var category in categories)
            {
                foreach (var level in category.levels)
                {
                    if (level.Id == levelData.Id)
                    {
                        isStoryMode = category.isStoryMode;
                        break;
                    }
                }
            }


            PopupManager.Instance.Show("loading", new object[] { 1, levelData },
                (bool cancelled, object[] outData) =>
                {
                    MaxMediationWrapper.Instance.ShowInterstitital(1, "inter_select");
                    PopupManager.Instance.CloseAllPopup();
                    StartLevel(levelData);
                });
        }

        public void DeleteSelected()
        {
            foreach (var level in SelectedLevels)
            {
                DeleteLevelSaveData(level);
            }

            SelectedLevels.Clear();
        }

        public void RestartLevel()
        {
            DeleteLevelSaveData(ActiveLevelData);
            StartLevel(ActiveLevelData);
        }

        public void StartLevel(LevelData levelData)
        {
            bool check = this.check;
            ReleaseLevel();
            if (check) this.check = check;
            // Set the new active LevelData
            ActiveLevelData = levelData;

            // Start loading everything needed to play the level
            bool loading = LoadManager.Instance.LoadLevel(levelData, OnLevelLoaded);

            if (loading)
            {
                GameEventManager.Instance.SendEvent(GameEventManager.LevelLoadingEvent);
            }
            else
            {
                OnLevelLoaded(levelData.Id, true);
            }

            
            // Show the game screen now
            ScreenManager.Instance.Show("game",false,true);

            // Increate the number of levels started since the last ad was shown
            numLevelsStarted++;
            _lastStartTime = Time.time;
            DOVirtual.DelayedCall(3f * 60, () =>
            {
                if (Time.time - _lastStartTime >= 3 * 60)
                    MaxMediationWrapper.Instance.ShowInterstitital(1, "inter_3m");
            });
        }

        private float _lastStartTime;

        public bool TrySelectRegion(int x, int y, out int colorIndex)
        {
            colorIndex = -1;
            LevelData activeLevelData = ActiveLevelData;
            if (activeLevelData != null)
            {
                Region region = GetRegionAt(x, y);
                colorIndex = region.colorIndex;
                if (activeLevelData.LevelRegionColoredPercentage(colorIndex) >= 1)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to color the region at the given pixel x/y. Returns true if a new region is colored, false if nothing changed.
        /// </summary>
        public bool TryColorRegion(int x, int y, int colorIndex, out bool levelCompleted, out bool hintAwarded,
            out bool coinsAwarded)
        {
            LevelData activeLevelData = ActiveLevelData;

            levelCompleted = false;
            hintAwarded = false;
            coinsAwarded = false;

            if (activeLevelData != null)
            {
                Region region = GetRegionAt(x, y, colorIndex);

                // Check that this is the correct region for the selected color index and the region has not already been colored in
                if (region != null && region.colorIndex == colorIndex &&
                    !activeLevelData.LevelSaveData.coloredRegions.Contains(region.id))
                {
                    // Color the region
                    ColorRegion(region);

                    // Set the region as colored in the level save data
                    activeLevelData.LevelSaveData.coloredRegions.Add(region.id);

                    // Check if the level is not in the playedLevelSaveDatas dictionary, it not then this is the first region to be colored
                    if (!playedLevelSaveDatas.ContainsKey(activeLevelData.Id))
                    {
                        // Set the LevelSaveData of the active LevelData in the playedLevelSaveDatas so will will saved now that a region has been colored
                        playedLevelSaveDatas.Add(activeLevelData.Id, activeLevelData.LevelSaveData);
                        levelSaveDatas.Remove(activeLevelData.Id);

                        GameEventManager.Instance.SendEvent(GameEventManager.LevelPlayedEvent, activeLevelData);
                    }

                    // Check if all regions have been colored
                    levelCompleted = activeLevelData.AllRegionsColored();

                    if (levelCompleted)
                    {
                        // Check if this level has not been awarded hints / coins yet (ie. first time the level is completed)
                        if (!awardedLevels.Contains(activeLevelData.Id))
                        {
                            awardedLevels.Add(activeLevelData.Id);

                            if (awardHints)
                            {
                                hintAwarded = true;
                            }

                            if (activeLevelData.coinsToAward > 0)
                            {
                                // Award coins to the player for completing this level
                                CurrencyManager.Instance.Give("coins", activeLevelData.coinsToAward);

                                coinsAwarded = true;
                            }
                        }

                        // The level is now complete
                        activeLevelData.LevelSaveData.isCompleted = true;

                        // Notify a lwevel has been compelted
                        GameEventManager.Instance.SendEvent(GameEventManager.LevelCompletedEvent, activeLevelData);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a LevelSaveData for the given level id
        /// </summary>
        public LevelSaveData GetLevelSaveData(string levelId)
        {
            LevelSaveData levelSaveData;

            if (playedLevelSaveDatas.ContainsKey(levelId))
            {
                levelSaveData = playedLevelSaveDatas[levelId];
            }
            else if (levelSaveDatas.ContainsKey(levelId))
            {
                levelSaveData = levelSaveDatas[levelId];
            }
            else
            {
                levelSaveData = new LevelSaveData();
                levelSaveDatas.Add(levelId, levelSaveData);
            }

            if (unlockedLevels.Contains(levelId))
            {
                levelSaveData.isUnlocked = true;
            }

            return levelSaveData;
        }

        /// <summary>
        /// Returns true if the level was completed atleast once by the player
        /// </summary>
        public bool IsLevelPlaying(string levelId)
        {
            return playedLevelSaveDatas.ContainsKey(levelId);
        }

        /// <summary>
        /// Gets all level datas that are beening played or have been completed
        /// </summary>
        public void GetMyWorksLevelDatas(out List<LevelData> myFinishedWorksLeveDatas,
            out List<LevelData> myOnGoingdWorksLeveDatas,
            out List<LevelData> myFavoriteWorksLeveDatas)
        {
            myFinishedWorksLeveDatas = new List<LevelData>();
            myOnGoingdWorksLeveDatas = new List<LevelData>();
            myFavoriteWorksLeveDatas = new List<LevelData>();

            int completeInsertIndex = 0;

            for (int i = 0; i < categories.Count; i++)
            {
                List<LevelData> levelDatas = categories[i].levels;

                for (int j = 0; j < levelDatas.Count; j++)
                {
                    LevelData levelData = levelDatas[j];
                    string levelId = levelData.Id;

                    if (playedLevelSaveDatas.ContainsKey(levelId))
                    {
                        LevelSaveData levelSaveData = playedLevelSaveDatas[levelId];

                        if (levelSaveData.isCompleted)
                        {
                            myFinishedWorksLeveDatas.Add(levelData);
                        }
                        else
                        {
                            myOnGoingdWorksLeveDatas.Add(levelData);
                        }

                        if (levelSaveData.isFavourite)
                            myFavoriteWorksLeveDatas.Add(levelData);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Invoked when the LevelLoadManager has finished loading the active LevelData
        /// </summary>
        private void OnLevelLoaded(string levelId, bool success)
        {
            if (ActiveLevelData != null && ActiveLevelData.Id == levelId)
            {
                GameEventManager.Instance.SendEvent(GameEventManager.LevelLoadFinishedEvent, success);
            }
        }

        /// <summary>
        /// Gets the Region which contains the given pixel
        /// </summary>
        private Region GetRegionAt(int x, int y, int colorIndex = -1)
        {
            List<Region> regions = ActiveLevelData.LevelFileData.regions;

            // Check all regions for the one that contains the pixel
            for (int i = 0; i < regions.Count; i++)
            {
                Region region = regions[i];
                // LevelData activeLevelData = ActiveLevelData;
                // if (colorIndex != -1 && region.colorIndex == colorIndex && !activeLevelData.LevelSaveData.coloredRegions.Contains(region.id))
                //     return region;
                // else continue;
                // Check if this region contains the pixel
                if (IsPixelInRegion(region, x, y))
                {
                    return region;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns true if one of the triangles in this region contains the given pixel point
        /// </summary>
        private bool IsPixelInRegion(Region region, int pixelX, int pixelY)
        {
            if (pixelX < region.bounds.minX || pixelX > region.bounds.maxX || pixelY < region.bounds.minY ||
                pixelY > region.bounds.maxY)
            {
                return false;
            }

            int index = region.pixelsByX ? (pixelX - region.bounds.minX) : (pixelY - region.bounds.minY);
            int value = region.pixelsByX ? pixelY : pixelX;

            List<int[]> pixelSections = region.pixelsInRegion[index];

            for (int k = 0; k < pixelSections.Count; k++)
            {
                int[] startEnd = pixelSections[k];
                int start = startEnd[0];
                int end = startEnd[1];

                if (value >= start && value <= end)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Colors the regions pixels on the levelData ColoredTexture
        /// </summary>
        private void ColorRegion(Region region)
        {
            // Color		regionColor		= ActiveLevelData.LevelFileData.colors[region.colorIndex];
            // Texture2D	coloredTexture	= ActiveLevelData.ColoredTexture;
            // Color[]		coloredPixels	= ActiveLevelData.ColoredTexture.GetPixels();
            // int			textureWidth	= coloredTexture.width;

            // int min = (region.pixelsByX ? region.bounds.minX : region.bounds.minY);

            // for (int j = 0; j < region.pixelsInRegion.Count; j++)
            // {
            // 	List<int[]> pixelSections = region.pixelsInRegion[j];

            // 	for (int k = 0; k < pixelSections.Count; k++)
            // 	{
            // 		int[]	startEnd	= pixelSections[k];
            // 		int		start		= startEnd[0];
            // 		int		end			= startEnd[1];

            // 		for (int i = start; i <= end; i++)
            // 		{
            // 			int x = region.pixelsByX ? min + j : i;
            // 			int y = region.pixelsByX ? i : min + j;

            // 			coloredPixels[y * textureWidth + x] = regionColor;
            // 		}
            // 	}
            // }

            // coloredTexture.SetPixels(coloredPixels);
            // coloredTexture.Apply();
        }

        /// <summary>
        /// Clears any progress from the level and sets the level as not completed
        /// </summary>
        public void DeleteLevelSaveData(LevelData levelData)
        {
            LevelSaveData levelSaveData = levelData.LevelSaveData;

            // Clear the colored regions
            levelSaveData.coloredRegions.Clear();

            // Make sure the completed flag is false
            levelSaveData.isCompleted = false;

            // Remove the level from the played and completed levels
            playedLevelSaveDatas.Remove(levelData.Id);

            GameEventManager.Instance.SendEvent(GameEventManager.LevelProgressDeletedEvent, levelData);
        }

        /// <summary>
        /// Unlocks the level
        /// </summary>
        public void UnlockLevel(LevelData levelData)
        {
            if (!unlockedLevels.Contains(levelData.Id))
            {
                unlockedLevels.Add(levelData.Id);
            }

            levelData.LevelSaveData.isUnlocked = true;

            GameEventManager.Instance.SendEvent(GameEventManager.LevelUnlockedEvent, new object[] { levelData });
        }

        /// <summary>
        /// Invoked by ScreenManager when screens are transitioning
        /// </summary>
        private void OnSwitchingScreens(string fromScreen, string toScreen)
        {
            if (fromScreen == "game")
            {
                ReleaseLevel();
            }
        }
        public bool check;
        private void ReleaseLevel()
        {
            if (ActiveLevelData != null&&!check)
            {
                LoadManager.Instance.ReleaseLevel(ActiveLevelData.Id);
                ActiveLevelData = null;
                //Debug.Log("Bong dung muon khoc ==========================================");
            }
            check =false;
        }

        #endregion

        #region Save Methods

        public override Dictionary<string, object> Save()
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();
            List<object> levelSaveDatas = new List<object>();

            foreach (KeyValuePair<string, LevelSaveData> pair in playedLevelSaveDatas)
            {
                Dictionary<string, object> levelSaveData = new Dictionary<string, object>();

                levelSaveData["key"] = pair.Key;
                levelSaveData["data"] = pair.Value.ToJson();

                levelSaveDatas.Add(levelSaveData);
            }

            saveData["levels"] = levelSaveDatas;
            saveData["awarded"] = SaveHashSetValues(awardedLevels);
            saveData["unlocked"] = SaveHashSetValues(unlockedLevels);

            return saveData;
        }

        protected override void LoadSaveData(bool exists, JSONNode saveData)
        {
            if (!exists)
            {
                return;
            }

            // Load all the levels that have some progress
            JSONArray levelSaveDatasJson = saveData["levels"].AsArray;

            for (int i = 0; i < levelSaveDatasJson.Count; i++)
            {
                JSONNode levelSaveDataJson = levelSaveDatasJson[i];
                string key = levelSaveDataJson["key"].Value;
                JSONNode data = levelSaveDataJson["data"];

                LevelSaveData levelSaveData = new LevelSaveData();

                levelSaveData.FromJson(data);

                playedLevelSaveDatas.Add(key, levelSaveData);
            }

            LoadHastSetValues(saveData["awarded"].Value, awardedLevels);
            LoadHastSetValues(saveData["unlocked"].Value, unlockedLevels);
        }

        /// <summary>
        /// Saves all values in the HashSet hash as a single string
        /// </summary>
        private string SaveHashSetValues(HashSet<string> hashSet)
        {
            string jsonStr = "";

            List<string> list = new List<string>(hashSet);

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                {
                    jsonStr += ";";
                }

                jsonStr += list[i];
            }

            return jsonStr;
        }

        /// <summary>
        /// Loads the hast set values.
        /// </summary>
        private void LoadHastSetValues(string str, HashSet<string> hashSet)
        {
            string[] values = str.Split(';');

            for (int i = 0; i < values.Length; i++)
            {
                hashSet.Add(values[i]);
            }
        }

        #endregion
    }
}