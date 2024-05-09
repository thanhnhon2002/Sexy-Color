using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
	public class SelectLevelPopup : Popup
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private RectTransform	pictureContainer	= null;
		[SerializeField] private PictureCreator	pictureCreator		= null;
		[SerializeField] private GameObject		loadingIndicator	= null;
		[SerializeField] private float			containerSize		= 0f;
		[Space]
		[SerializeField] private GameObject	continueButton		= null;
		[SerializeField] private GameObject	deleteButton		= null;
		[SerializeField] private GameObject	restartButton		= null;
		[SerializeField] private GameObject	unlockButton		= null;
		[SerializeField] private Text		unlockAmountText	= null;

		#endregion

		#region Member Variables

		private LevelData levelData;

		#endregion

		#region Public Methods

		public override void OnShowing(object[] inData)
		{
			base.OnShowing(inData);

			levelData = inData[0] as LevelData;

			bool isLocked = (bool)inData[1];

			bool isCompleted	= !isLocked && levelData.LevelSaveData.isCompleted;
			bool isPlaying		= !isLocked && !isCompleted && GameManager.Instance.IsLevelPlaying(levelData.Id);

			continueButton.SetActive(isPlaying);
			deleteButton.SetActive(isPlaying || isCompleted);
			restartButton.SetActive(isPlaying || isCompleted);
			// unlockButton.SetActive(isLocked);
			deleteButton.SetActive(false);
			unlockButton.SetActive(false);

			if (isLocked)
			{
				unlockAmountText.text = levelData.coinsToUnlock.ToString();
			}

			SetThumbnaiImage();
		}

		public override void OnHiding(bool cancelled)
		{
			base.OnHiding(cancelled);

			ReleaseLevel();
		}

		#endregion

		#region Private Methods

		private void SetThumbnaiImage()
		{
			bool loading = LoadManager.Instance.LoadLevel(levelData, OnLoadManagerFinished);

			if (loading)
			{
				loadingIndicator.SetActive(true);
			}
			else
			{
				SetupImages();

				loadingIndicator.SetActive(false);
			}
		}

		private void OnLoadManagerFinished(string levelId, bool success)
		{
			if (success && levelData != null && levelId == levelData.Id)
			{
				loadingIndicator.SetActive(false);

				SetupImages();
			}
		}

		private void SetupImages()
		{
			LevelFileData levelFileData = LoadManager.Instance.GetLevelFileData(levelData.Id);

			float imageWidth	= levelFileData.imageWidth;
			float imageHeight	= levelFileData.imageHeight;
			float xScale		= imageWidth >= imageHeight ? 1f : imageWidth / imageHeight;
			float yScale		= imageWidth <= imageHeight ? 1f : imageHeight / imageWidth;

			pictureContainer.sizeDelta = new Vector2(containerSize * xScale, containerSize * yScale);

			float pictureScale = Mathf.Min(pictureContainer.rect.width / imageWidth, pictureContainer.rect.height / imageHeight, 1f);

			pictureCreator.RectT.sizeDelta	= new Vector2(imageWidth, imageHeight);
			pictureCreator.RectT.localScale	= new Vector3(pictureScale, pictureScale, 1f);

			pictureCreator.Setup(levelData.Id, padding: 2);
		}

		private void ReleaseLevel()
		{
			if (levelData != null)
			{
				LoadManager.Instance.ReleaseLevel(levelData.Id);
				levelData = null;
			}
		}

		#endregion
	}
}
