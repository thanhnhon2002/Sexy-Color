using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class GameScreen : Screen
    {
        #region Inspector Variables

        [Space] [SerializeField] private PictureArea             pictureArea               = null;
        [SerializeField]         private ColorList               colorList                 = null;
        [SerializeField]         private GameObject              levelLoadingIndicator     = null;
        [Space] [SerializeField] private CanvasGroup             gameplayUI                = null;
        [SerializeField]         private CanvasGroup             levelCompleteUI           = null;
        [SerializeField]         private CanvasGroup             levelContinueUI           = null;
        [Space] [SerializeField] private GameObject              awardedHintTextContainer  = null;
        [SerializeField]         private GameObject              awardedCoinsTextContainer = null;
        [SerializeField]         private Text                    awardedCoinsAmountText    = null;
        [Space] [SerializeField] private GameObject              shareButtonsContainer     = null;
        [SerializeField]         private CanvasGroup             notificationContainer     = null;
        [SerializeField]         private Text                    notificationText          = null;
        [SerializeField]         private int                     boundaryOffset            = 10;
        [SerializeField]         private Slider                  levelProgressSlider;
        [SerializeField]         private TextMeshProUGUI         progressText;
        [SerializeField]         private MMF_Player              regionFilledFeedback;
        [SerializeField]         private GameObject              exitComfirmPopup;
        [SerializeField]         private Button                  backButton;
        [SerializeField]         private SkeletonGraphic         milestoneSpine;
        [SerializeField]         private AnimationReferenceAsset unboxRef;
        [SerializeField]         private Button                  zoomOutButton;
        [SerializeField]         private Button                  saveButton;
        

        #endregion

        #region Unity Method

        private void Awake()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
            zoomOutButton.onClick.AddListener(() =>
            {
                pictureArea.ZoomOut();
                zoomOutButton.gameObject.SetActive(false);
            });
        }

        private void OnBackButtonClicked()
        {
            if (PlayerPrefs.GetInt("BackButtonShowAdsCount", 0) == 0)
            {
                PlayerPrefs.SetInt("BackButtonShowAdsCount", 1);
            }
            else
            {
                PlayerPrefs.SetInt("BackButtonShowAdsCount", 0);
                MaxMediationWrapper.Instance.ShowInterstitital(1,"inter_back");
            }
            PlayerPrefs.Save();
            var activeLevelData = GameManager.Instance.ActiveLevelData;
            if (activeLevelData != null && activeLevelData.LevelCompletePercentage() >= .5f)
            {
                exitComfirmPopup.SetActive(true);
            }
            else
            {
                ScreenManager.Instance.Back();
                ScreenManager.Instance.Back();
            }
        }

        #endregion

        #region Public Methods

        public void OnExitConfirmButtonClicked()
        {
            ScreenManager.Instance.Back();
        }

        public override void Initialize()
        {
            base.Initialize();

            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelLoadingEvent, OnLevelLoading);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelLoadFinishedEvent,
                OnLevelLoadFinished);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelProgressDeletedEvent, OnLevelDeleted);

            pictureArea.Initialize();
            colorList.Initialize();

            pictureArea.OnPixelClicked =  OnPixelClicked;
            pictureArea.OnPixelHold    =  OnPixelHold;
            pictureArea.OnZoom         += OnPictureZoom;
            colorList.OnColorSelected  =  OnColorSelected;

            gameplayUI.gameObject.SetActive(true);
            levelCompleteUI.gameObject.SetActive(true);

            // shareButtonsContainer.SetActive(NativePlugin.Exists());
        }

        private void OnLevelDeleted(string eventid, object[] data)
        {
            ResetUI();
        }

        public void OnShopButtonClicked()
        {
            PopupManager.Instance.Show("vip_purchase_popup");    
        }
        
        /// <summary>
        /// Invoked when the hint button is clicked
        /// </summary>
        public void OnHintButtonClicked()
        {
            if (PlayerPrefs.GetInt("UnlimitedHints", 0) == 0 && PlayerPrefs.GetInt("WeeklySubscription", 0) == 0 && PlayerPrefs.GetInt("MonthlySubscription", 0) == 0)
            {
                if (CurrencyManager.Instance.GetAmount("hints") == 0)
                {
                    MaxMediationWrapper.Instance.ShowRewardAd(()=>
                    {
                        CurrencyManager.Instance.Give("hints", 1);
                    });
                    return;
                }
                if(!CurrencyManager.Instance.TrySpend("hints", 1))
                    return;
            }
            
            LevelData activeLevelData = GameManager.Instance.ActiveLevelData;

            if (activeLevelData != null)
            {
                // Get a random uncolored region inside the selected color region
                int selectedColoredIndex = colorList.SelectedColorIndex;
                int regionIndex          = activeLevelData.GetSmallestUncoloredRegion(selectedColoredIndex);

                // If -1 is returned then all regions have been colored
                if (regionIndex != -1)
                {
                    // Region is not -1 and we successfully spend a currency to use the hint
                    Region region = activeLevelData.LevelFileData.regions[regionIndex];
                    pictureArea.ZoomInOnRegion(region);
                }
            }
        }

        public override void OnShowing()
        {
            base.OnShowing();
            milestoneSpine.AnimationState.ClearTracks();
        }

        /// <summary>
        /// Invoked when the Twitter button is clicked
        /// </summary>
        public void OnTwitterButtonClicked()
        {
            LoadShareTexture(ShareToTwitter);
        }

        /// <summary>
        /// Invoked when the Instagram button is clicked
        /// </summary>
        public void OnInstagramButtonClicked()
        {
            LoadShareTexture(ShareToInstagram);
        }

        /// <summary>
        /// Invoked when the Share Other button is clicked
        /// </summary>
        public void OnShareOtherButtonClicked()
        {
            LoadShareTexture(ShareToOther);
        }

        /// <summary>
        /// Invoked when the Save button is clicked
        /// </summary>
        public void OnSaveToDevice()
        {
            MaxMediationWrapper.Instance.ShowInterstitital(1,"inter_down");
            saveButton.interactable = false;
            LoadShareTexture(SaveShareTextureToDevice);
            DOVirtual.DelayedCall(5f, () => { saveButton.interactable = true;});
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Invoked by the GameEventManager.LevelLoadingEvent when a level is loading
        /// </summary>
        private void OnLevelLoading(string eventId, object[] data)
        {
            // Show the loading indicator
            levelLoadingIndicator.SetActive(true);

            // Clear and reset the UI
            pictureArea.Clear();
            colorList.Clear();

            ResetUI();
        }

        /// <summary>
        /// Invoked by the GameEventManager.LevelLoadFinishedEvent when the level has finished loading and has all required textures to play the game
        /// </summary>
        private void OnLevelLoadFinished(string eventId, object[] data)
        {
            // Hide the loading indicator
            levelLoadingIndicator.SetActive(false);

            // First argument is a boolean, if true the level loaded successfully
            bool success = (bool)data[0];

            if (success)
            {
                int firstSelectedColor = 0;

                // Setup the picture and the color list items
                pictureArea.Setup(firstSelectedColor);
                colorList.Setup(firstSelectedColor);

                ResetUI();

                // We update the Level Progress after loading the level
                UpdateLevelProgress(true);
            }
        }

        /// <summary>
        /// Resets the UI
        /// </summary>
        public void ResetUI()
        {
            gameplayUI.gameObject.SetActive(true);
            gameplayUI.interactable   = true;
            gameplayUI.blocksRaycasts = true;
            gameplayUI.alpha          = 1f;

            levelCompleteUI.interactable   = false;
            levelCompleteUI.blocksRaycasts = false;
            levelCompleteUI.alpha          = 0f;
            
            levelContinueUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// Invoked by ColorList when a new color has been selected
        /// </summary>
        private void OnColorSelected(int colorIndex)
        {
            pictureArea.SetSelectedRegion(colorIndex);
        }

        private void OnPixelHold(int x, int y, Vector2 screenPosition)
        {
            if (GameManager.Instance.TrySelectRegion(x, y, out int colorIndex))
            {
                colorList.SelectedColorIndex = colorIndex;
                OnColorSelected(colorIndex);
            }
            else
            {
                Debug.Log("Region not existed or has been colored!");
            }
        }

        private void OnPictureZoom()
        {
            zoomOutButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Invoked by PictureArea when the picture is clicked, x/y is relative to the bottom left corner of the picture
        /// </summary>
        private void OnPixelClicked(int x, int y, Vector2 screenPosition)
        {
            LevelData activeLevelData = GameManager.Instance.ActiveLevelData;

            if (activeLevelData != null)
            {
                bool levelCompleted;
                bool hintAwarded;
                bool coinsAwarded;

                // Try and color the region which contains the pixel at x,y
                bool regionColored = GameManager.Instance.TryColorRegion(x, y, colorList.SelectedColorIndex,
                    out levelCompleted, out hintAwarded, out coinsAwarded);

                // setup boundary of x,y to try and color
                int minX = x - boundaryOffset;
                int maxX = x + boundaryOffset;
                int minY = y - boundaryOffset;
                int maxY = y + boundaryOffset;


                // If cannot find region to color, we find the nearest region to color
                if (!regionColored)
                {
                    for (int i = minX; i < maxX; i++)
                    {
                        for (int j = minY; j < maxY; j++)
                        {
                            // Try and color the region which contains the pixel at x,y
                            regionColored = GameManager.Instance.TryColorRegion(i, j, colorList.SelectedColorIndex,
                                out levelCompleted, out hintAwarded, out coinsAwarded);
                            if (regionColored)
                            {
                                break;
                            }
                        }

                        if (regionColored)
                        {
                            break;
                        }
                    }
                }


                if (regionColored)
                {
                    regionFilledFeedback.GetComponent<RectTransform>().position = screenPosition;
                    regionFilledFeedback?.PlayFeedbacks();
                    // Update the color list item if the color region is now complete
                    colorList.CheckCompleted(colorList.SelectedColorIndex);

                    // Notify the picture area that a new region was colored so it can remove the number text for that region
                    pictureArea.NotifyRegionColored();
                    UpdateLevelProgress();
                    // Check if the level has been completed
                    if (levelCompleted)
                    {
                        MMEventManager.TriggerEvent(new MMGameEvent("LevelFinished"));
                        // Show the level completed UI
                        LevelCompleted(hintAwarded, coinsAwarded);
                    }
                }
            }
        }

        /// <summary>
        /// Displays the active level as compelted
        /// </summary>
        private void LevelCompleted(bool hintAwarded, bool coinsAwarded)
        {
            SoundManager.Instance.Stop(SoundManager.SoundType.Music);
            SoundManager.Instance.Play("level-completed");
            LevelData activeLevelData = GameManager.Instance.ActiveLevelData;

            if (activeLevelData != null)
            {
                // Tell PictureArea the level is completed
                pictureArea.NotifyLevelCompleted();


                // Fade out the gameplay UI and fade in the level completed UI
                UIAnimation anim;

                anim       = UIAnimation.Alpha(gameplayUI, 0f, 0.5f);
                anim.style = UIAnimation.Style.EaseOut;
                anim.Play();

                anim       = UIAnimation.Alpha(levelCompleteUI, 1f, 0.5f);
                anim.style = UIAnimation.Style.EaseOut;
                anim.Play();

                gameplayUI.interactable   = false;
                gameplayUI.blocksRaycasts = false;

                levelCompleteUI.interactable   = true;
                levelCompleteUI.blocksRaycasts = true;
                levelCompleteUI.GetComponentInChildren<SuggestionPanel>().Show();
                levelContinueUI.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Loads the share texture and calls onTextureLoaded which it is loaded
        /// </summary>
        private void LoadShareTexture(System.Action<Texture2D> onTextureLoaded)
        {
            ScreenshotManager.Instance.GetShareableTexture(GameManager.Instance.ActiveLevelData, onTextureLoaded);
        }

        /// <summary>
        /// Shares the given texture to the twitter app if it's installed
        /// </summary>
        private void ShareToTwitter(Texture2D texture)
        {
            bool opened = ShareManager.Instance.ShareToTwitter(texture);

            if (!opened)
            {
                ShowNotification("Twitter is not installed");
            }

            Destroy(texture);
        }

        /// <summary>
        /// Shares the given texture to the instagram app if it's installed
        /// </summary>
        private void ShareToInstagram(Texture2D texture)
        {
            bool opened = ShareManager.Instance.ShareToInstagram(texture);

            if (!opened)
            {
                ShowNotification("Instagram is not installed");
            }

            Destroy(texture);
        }

        /// <summary>
        /// Shares the given texture letting the user pick what app to use
        /// </summary>
        private void ShareToOther(Texture2D texture)
        {
            ShareManager.Instance.ShareToOther(texture);

            Destroy(texture);
        }

        /// <summary>
        /// Saves the share texture to device
        /// </summary>
        private void SaveShareTextureToDevice(Texture2D texture)
        {
            ShareManager.Instance.SaveImageToPhotos(texture, OnSaveToPhotosResponse);

            Destroy(texture);
        }

        /// <summary>
        /// Shows the notification
        /// </summary>
        private void ShowNotification(string message)
        {
            // Set the text for the notification
            notificationText.text = message;

            // Fade in the notification
            UIAnimation.Alpha(notificationContainer, 1f, 0.35f).Play();

            // Wait a couple seconds then hide the notification
            StartCoroutine(WaitThenHideNotification());
        }

        /// <summary>
        /// Hides the notification.
        /// </summary>
        private void HideNotification()
        {
            UIAnimation.Alpha(notificationContainer, 0f, 0.35f).Play();
        }

        /// <summary>
        /// Waits 3 seconds then hides notification.
        /// </summary>
        private IEnumerator WaitThenHideNotification()
        {
            yield return new WaitForSeconds(3);

            HideNotification();
        }

        /// <summary>
        /// Invoked when the ShareManager has either save the image to photos or failed to due to permissions not being granted
        /// </summary>
        private void OnSaveToPhotosResponse(bool success)
        {
            if (success)
            {
                ShowNotification("Picture saved to device!");
            }
            else
            {
#if UNITY_IOS
				PopupManager.Instance.Show("permissions", new object[] { "Photos" });
#elif UNITY_ANDROID
                PopupManager.Instance.Show("permissions", new object[] { "Storage" });
#endif
            }
        }

        private void UpdateLevelProgress(bool isAwake = false)
        {
            LevelData activeLevelData = GameManager.Instance.ActiveLevelData;

            if (activeLevelData != null)
            {
                if (activeLevelData.LevelCompletePercentage() > 0 && isAwake&&activeLevelData.LevelCompletePercentage()<1&& !activeLevelData.AllRegionsColored())
                {
                    levelContinueUI.gameObject.SetActive(true);
                    gameplayUI.gameObject.SetActive(false);
                    levelContinueUI.GetComponent<LevelCompletePopup>().Setup(activeLevelData);
                    levelContinueUI.GetComponentInChildren<SuggestionPanel>().Show();

                    //gameplayUI.interactable = false;
                    //gameplayUI.blocksRaycasts = false;
                    //gameplayUI.alpha = 0f;
                    //levelCompleteUI.GetComponent<LevelCompletePopup>().Setup(activeLevelData);
                    //levelCompleteUI.interactable = true;
                    //levelCompleteUI.blocksRaycasts = true;
                    //levelCompleteUI.alpha = 1f;
                    //levelCompleteUI.GetComponentInChildren<SuggestionPanel>().Show();
                    //levelContinueUI.gameObject.SetActive(false);

                }

                levelProgressSlider.DOValue(activeLevelData.LevelCompletePercentage(), .5f);
                progressText.text = $"{activeLevelData.LevelCompletePercentage():P0}";
                if (activeLevelData.LevelCompletePercentage() >= GameManager.Instance.GiftMilestoneValue)
                {
                    milestoneSpine.AnimationState.SetAnimation(0, "Unbox", false);
                    if (!GameManager.Instance.AwardedLevels.Contains(activeLevelData.Id))
                    {
                        PopupManager.Instance.Show("halfway_gift", new object[] { activeLevelData });
                        GameManager.Instance.AwardedLevels.Add(activeLevelData.Id);
                    }
                }
            }

            if (activeLevelData?.LevelCompletePercentage() == 1||activeLevelData.AllRegionsColored())
            {
                gameplayUI.interactable        = false;
                gameplayUI.blocksRaycasts      = false;
                gameplayUI.alpha               = 0f;
                levelCompleteUI.GetComponent<LevelCompletePopup>().Setup(activeLevelData);
                levelCompleteUI.interactable   = true;
                levelCompleteUI.blocksRaycasts = true;
                levelCompleteUI.alpha          = 1f;
                levelCompleteUI.GetComponentInChildren<SuggestionPanel>().Show();
                levelContinueUI.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}