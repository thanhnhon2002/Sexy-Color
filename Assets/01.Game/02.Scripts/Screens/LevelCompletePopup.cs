using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class LevelCompletePopup : MonoBehaviour
    {
        [SerializeField] private GameObject restartConfirmPopup;
        [SerializeField] private GameObject deleteConfirmPopup;
        [SerializeField] private Image completePicture;

        private LevelData _levelData;

        public void Setup(LevelData levelData)
        {
            _levelData = levelData;
            completePicture.sprite = levelData.levelImage;
        }
        
        public void ShowRestartConfirmPopup()
        {
            restartConfirmPopup.SetActive(true);
        }

        public void OnRestartButtonClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        public void ShowDeleteConfirmPopup()
        {
            deleteConfirmPopup.SetActive(true);
        }
        
        public void DeleteLevel()
        {
            GameManager.Instance.DeleteLevelSaveData(_levelData);
        }

        public void OnContinueButtonClicked()
        {
            MaxMediationWrapper.Instance.ShowInterstitital(1, "inter_continue");
        }

    }
}