using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
	public class CategoryListItem : ClickableListItem
	{
		#region Inspector Variables

		[SerializeField] private TextMeshProUGUI categoryText     = null;
		[SerializeField] private Color defaultColorText;
		[SerializeField] private Color selectedColorText;
		[SerializeField] private GameObject      categoryActive   = null;
		[SerializeField] private GameObject      categoryDeactive = null;
		[SerializeField] private GameObject      vipIcon = null;

		#endregion

		#region Public Methods

		public void Setup(string displayText,bool isVIP = false)
		{
			categoryText.text = displayText;
			vipIcon.SetActive(isVIP);
			categoryText.color = defaultColorText;
		}

		public void SetSelected(bool isSelected)
		{
			categoryActive.SetActive(isSelected);
			categoryDeactive.SetActive(!isSelected);
			categoryText.color = isSelected ? selectedColorText : defaultColorText;
		}

		#endregion
	}
}
