using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class TrendingLevelItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        public UnityEvent<LevelData> OnTrendingLevelClickedEvent;
        
        private LevelData _levelData;
        private Button _button;

        public void Populate(LevelData levelData)
        {
            _levelData   = levelData;
            image.sprite = _levelData.levelImagePreview ?  levelData.levelImage : _levelData.levelImagePreview;
        }

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            OnTrendingLevelClickedEvent?.Invoke(_levelData);
        }
    }
}