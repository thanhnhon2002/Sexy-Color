using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace BBG.PictureColoring
{
    public class LevelListItem : RecyclableListItem<LevelData>, MMEventListener<MMGameEvent>, MMEventListener<SelectImageEvent>
    {
        #region Inspector Variables

        [SerializeField] private PictureCreator  pictureCreator     = null;
        [SerializeField] private GameObject      loadingIndicator   = null;
        [SerializeField] private GameObject      completedIndicator = null;
        [SerializeField] private GameObject      playedIndicator    = null;
        [SerializeField] private GameObject      lockedIndicator    = null;
        [SerializeField] private GameObject      coinCostContainer  = null;
        [SerializeField] private Text            coinCostText       = null;
        [SerializeField] private Image           grayScaleImage;
        [SerializeField] private MMSpriteReplace likeSprite;
        [SerializeField] private MMSpriteReplace selectImage;

        #endregion

        #region Member Variables

        private string        levelId;
        private LevelData     _levelData;
        private float         _width;
        private float         _height;
        private LevelSaveData _levelSaveData;

        #endregion

        #region Public Methods

        private void OnEnable()
        {
            MMEventManager.AddListener<MMGameEvent>(this);
            MMEventManager.AddListener<SelectImageEvent>(this);
        }

        private void OnDisable()
        {
            MMEventManager.RemoveListener<MMGameEvent>(this);
            MMEventManager.RemoveListener<SelectImageEvent>(this);
        }

        public override void Initialize(LevelData dataObject)
        {
            loadingIndicator.SetActive(false);
        }

        public override void Removed()
        {
            ReleaseLevel();
        }

        public override void Setup(LevelData levelData)
        {
            GridLayoutGroup gridLayoutGroup = pictureCreator.GetComponentInParent<GridLayoutGroup>();
            _width  = gridLayoutGroup.cellSize.x;
            _height = gridLayoutGroup.cellSize.y;
            ReleaseLevel();

            UpdateUI(levelData);

            levelId    = levelData.Id;
            _levelData = levelData;
            Data       = levelData;
            bool loading = LoadManager.Instance.LoadLevel(levelData, OnLoadManagerFinished);

            if (loading)
            {
                pictureCreator.Clear();

                loadingIndicator.SetActive(true);
            }
            else
            {
                // Level already loaded
                SetImages(levelId);

                loadingIndicator.SetActive(false);
            }

            Refresh(levelData);
            EnableSelectPanel(false);

            if (grayScaleImage != null)
            {
                grayScaleImage.sprite = _levelData.levelImagePreview ? _levelData.levelImagePreview : levelData.levelImage;
                grayScaleImage.gameObject.SetActive(true);
            }
        }

        public override void Refresh(LevelData levelData)
        {
            if (levelData.Id != levelId)
            {
                Setup(levelData);
            }
            else
            {
                pictureCreator.RefreshImages();
            }
        }

        public void EnableSelectPanel(bool isSelect)
        {
            selectImage?.gameObject.SetActive(isSelect);
        }

        public void LikeImage()
        {
            _levelSaveData.isFavourite = !_levelSaveData.isFavourite;
            likeSprite.Swap();
            GameEventManager.Instance.SendEvent(GameEventManager.LevelLikeEvent, _levelData);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Invoked when the LevelLoadManager finishes loading everything needed to display the levels thumbnail
        /// </summary>
        private void OnLoadManagerFinished(string levelId, bool success)
        {
            if (success && this.levelId == levelId)
            {
                loadingIndicator.SetActive(false);

                SetImages(levelId);
            }
        }

        /// <summary>
        /// Sets the images
        /// </summary>
        private void SetImages(string levelId)
        {
            LevelFileData levelFileData = LoadManager.Instance.GetLevelFileData(levelId);

            // float containerWidth	= (pictureCreator.transform.parent as RectTransform).rect.width;
            // float containerHeight	= (pictureCreator.transform.parent as RectTransform).rect.height;
            float contentWidth  = levelFileData.imageWidth;
            float contentHeight = levelFileData.imageHeight;
            float scale         = Mathf.Min(_width / contentWidth, _height / contentHeight, 1f);

            pictureCreator.RectT.sizeDelta  = new Vector2(contentWidth, contentHeight);
            pictureCreator.RectT.localScale = new Vector3(scale, scale, 1f);


            _levelSaveData = GameManager.Instance.GetLevelSaveData(levelId);
            if (_levelSaveData.isFavourite)
            {
                likeSprite.SwitchToOnSprite();
            }
            else
            {
                likeSprite.SwitchToOffSprite();
            }

            if (_levelSaveData.isCompleted) grayScaleImage.material = null;
            // pictureCreator.Setup(levelId, padding: 2);
        }

        /// <summary>
        /// Updates the UI of the list item
        /// </summary>
        private void UpdateUI(LevelData levelData)
        {
            bool isLocked    = levelData.locked && !levelData.LevelSaveData.isUnlocked;
            bool isPlaying   = GameManager.Instance.IsLevelPlaying(levelData.Id);
            bool isCompleted = levelData.LevelSaveData.isCompleted;

            completedIndicator.SetActive(isCompleted);
            playedIndicator.SetActive(!isCompleted && isPlaying);
            lockedIndicator.SetActive(isLocked);
            // coinCostContainer.SetActive(isLocked);

            coinCostText.text = levelData.coinsToUnlock.ToString();
        }

        private void ReleaseLevel()
        {
            if (!string.IsNullOrEmpty(levelId))
            {
                LoadManager.Instance.ReleaseLevel(levelId);
                levelId = null;
            }
        }

        #endregion

        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName.Equals("Selecting"))
            {
                EnableSelectPanel(GameManager.Instance.IsSelecting);
            }
        }

        public void OnMMEvent(SelectImageEvent eventType)
        {
            if (_levelData.Id == eventType.LevelData.Id)
            {
                selectImage?.Swap();
            }
        }
    }
}