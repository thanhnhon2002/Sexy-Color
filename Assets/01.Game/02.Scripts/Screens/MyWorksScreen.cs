using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lofelt.NiceVibrations;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class MyWorksScreen : Screen,MMEventListener<SelectImageEvent>
    {
        #region Inspector Variables

        [Space] [SerializeField] private LevelListItem listItemPrefab = null;
        [SerializeField] private GridLayoutGroup listContainer = null;
        [SerializeField] private ScrollRect listScrollRect = null;
        [SerializeField] private MMSpriteReplace[] buttonSprites;
        [SerializeField] private GameObject emptyPanel;
        [SerializeField] private GameObject notEmptyPanel;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private GameObject deleteConfirmPopup;
        
        
        

        #endregion

        #region Member Variables

        private List<LevelData> myWorksLevelDatas
        {
            get
            {
                switch (_tabName)
                {
                    case "Finished":
                        return myFinishedWorks;
                    case "OnGoing":
                        return myOnGoingWorks;
                    case "Favourite":
                        return favouriteWorks;
                    default:
                        return myFinishedWorks;
                }
            }
        }

        private List<LevelData> myFinishedWorks;
        private List<LevelData> myOnGoingWorks;
        private List<LevelData> favouriteWorks;
        private RecyclableListHandler<LevelData> listHandler;
        private string _tabName = "OnGoing";
        private bool _isSelecting;

        #endregion
        
        #region Unity Methods

        private void OnEnable()
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
            MMEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            MMEventManager.RemoveListener(this);
        }

        private void OnDeleteButtonClicked()
        {
            if (!GameManager.Instance.IsSelecting) return;
            if (GameManager.Instance.SelectedLevels.Count == 0) return;
            deleteConfirmPopup.SetActive(true);
        }

        public void OnConfirmDeletePopup()
        {
            GameManager.Instance.DeleteSelected();
            deleteConfirmPopup.SetActive(false);
        }

        private void OnSelectButtonClicked()
        {
            if (myWorksLevelDatas.Count == 0)
            {
                deleteButton.gameObject.SetActive(false);
                GameManager.Instance.IsSelecting = false;
                return;
            }
            GameManager.Instance.IsSelecting = !GameManager.Instance.IsSelecting;
            if (GameManager.Instance.IsSelecting)
            {
                deleteButton.gameObject.SetActive(true);
                GameManager.Instance.SelectedLevels.Clear();
            }
            else
            {
                deleteButton.gameObject.SetActive(false);
            }
            
            MMEventManager.TriggerEvent(new MMGameEvent("Selecting"));
        }

        
        public override void Initialize()
        {
            base.Initialize();

            // Set the cells size based on the width of the screen
            Utilities.SetGridCellSize(listContainer);

            SetupLibraryList();

            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelPlayedEvent, OnLevelPlayedEvent);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelCompletedEvent, OnLevelCompletedEvent);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelProgressDeletedEvent,
                OnLevelDeletedEvent);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.LevelLikeEvent,
                OnLevelLikeEvent);
        }

        private void OnLevelLikeEvent(string eventid, object[] data)
        {
            LevelData levelData = data[0] as LevelData;
            if (favouriteWorks.Contains(levelData))
            {
                favouriteWorks.Remove(levelData);
            }
            else
            {
                favouriteWorks.Add(levelData);
            }
            listHandler.UpdateDataObjects(myWorksLevelDatas);
        }

        public override void OnShowing()
        {
            if (listHandler != null)
            {
                listHandler.Refresh();
            }
            
            emptyPanel.SetActive(myFinishedWorks.Count == 0 && myOnGoingWorks.Count == 0);
            notEmptyPanel.SetActive(myFinishedWorks.Count != 0 || myOnGoingWorks.Count != 0);
        }

        public void ShowTab(string tabName)
        {
            if (_tabName.Equals(tabName)) return;
            int index = tabName switch
            {
                "OnGoing" => 0,
                "Finished" => 1,
                "Favourite" => 2,
                _ => 0
            };
            _tabName = tabName;
            for (int i = 0; i < buttonSprites.Length; i++)
            {
                if(i == index) buttonSprites[i].SwitchToOnSprite();
                else buttonSprites[i].SwitchToOffSprite();
            }

            listHandler.UpdateDataObjects(myWorksLevelDatas);
        }

        #endregion

        #region Private Methods

        private void OnLevelPlayedEvent(string eventId, object[] data)
        {
            // Remove from finished
            if (myFinishedWorks.Contains(data[0] as LevelData)) myFinishedWorks.Remove(data[0] as LevelData);

            if (!myOnGoingWorks.Contains(data[0] as LevelData)) myOnGoingWorks.Add(data[0] as LevelData);

            // Update the list handler with the new list of level datas
            listHandler.UpdateDataObjects(myWorksLevelDatas);
        }

        private void OnLevelCompletedEvent(string eventId, object[] data)
        {
            LevelData levelData = data[0] as LevelData;

            // Remove the LevelData that was completed and re-insert it 
            if (_tabName.Equals("OnGoing"))
                myWorksLevelDatas.Remove(levelData);

            myFinishedWorks.Add(levelData);
            myOnGoingWorks.Remove(levelData);

            // Update the list handler with the new list of level datas
            listHandler.UpdateDataObjects(myWorksLevelDatas);
        }

        private void OnLevelDeletedEvent(string eventId, object[] data)
        {
            LevelData levelData = data[0] as LevelData;

            // Remove the deleted LevelData
            myWorksLevelDatas.Remove(levelData);

            if (myFinishedWorks.Contains(levelData))
            {
                myFinishedWorks.Remove(levelData);
                myOnGoingWorks.Add(levelData);
            }

            // Update the list handler with the new list of level datas
            listHandler.UpdateDataObjects(myWorksLevelDatas);
        }

        /// <summary>
        /// Clears then resets the list of library level items using the current active category index
        /// </summary>
        private void SetupLibraryList()
        {
            GameManager.Instance.GetMyWorksLevelDatas(out myFinishedWorks, out myOnGoingWorks, out favouriteWorks);


            if (listHandler == null)
            {
                listHandler = new RecyclableListHandler<LevelData>(myWorksLevelDatas, listItemPrefab,
                    listContainer.transform as RectTransform, listScrollRect);

                listHandler.OnListItemClicked = GameManager.Instance.LevelSelected;

                listHandler.Setup();
            }
            else
            {
                listHandler.UpdateDataObjects(myWorksLevelDatas);
            }
        }

        #endregion

        public void OnMMEvent(SelectImageEvent eventType)
        {
            if (GameManager.Instance.IsSelecting)
            {
                if(GameManager.Instance.SelectedLevels.Count > 0)
                    deleteButton.GetComponent<MMSpriteReplace>().SwitchToOnSprite();
                else
                {
                    deleteButton.GetComponent<MMSpriteReplace>().SwitchToOffSprite();
                }
            }
        }
    }
}