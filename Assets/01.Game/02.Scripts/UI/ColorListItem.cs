using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
	public class ColorListItem : ClickableListItem
	{
		#region Inspector Variables

		[SerializeField] private Image      colorImage   = null;
		[SerializeField] private Text       numberText   = null;
		[SerializeField] private GameObject completedObj = null;
		[SerializeField] private GameObject selectedObj  = null;
		[SerializeField] private MMF_Player finishFeedback;
		[SerializeField] private Image     coloredProgress;

		public bool IsComplete { get; private set; }

		private CanvasGroup canvasGroup;
		#endregion

		private void OnEnable()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		#region Public Methods

		public void Setup(Color color, int number)
		{
			colorImage.color	= color;
			numberText.text		= number.ToString();

			numberText.enabled = true;

			selectedObj.SetActive(false);
			completedObj.SetActive(false);
			canvasGroup.alpha = 1;
			gameObject.SetActive(true);
			if (finishFeedback.FeedbacksList.FirstOrDefault(obj => obj.GetType() == typeof(MMF_NVPreset)) != null)
			{
				finishFeedback.FeedbacksList.FirstOrDefault(obj => obj.GetType() == typeof(MMF_NVPreset)).Active = PlayerPrefs.GetInt("Vibration_Setting",0)==1;
			}

			IsComplete = false;
		}

		public void SetSelected(bool isSelected)
		{
			selectedObj.SetActive(isSelected);
			if(isSelected) transform.localScale = Vector3.one * 1.1f;
			else transform.localScale = Vector3.one;
		}

		public void SetCompleted()
		{
			IsComplete         = true;
			numberText.enabled = false;
			// completedObj.SetActive(true);
			finishFeedback?.PlayFeedbacks();
			canvasGroup.DOFade(0, 0.5f).OnComplete(()=>gameObject.SetActive(false));
		}

		public void SetProgress(float progress)
		{
			coloredProgress.DOFillAmount(progress, .5f) ;
		}

		#endregion
	}
}
