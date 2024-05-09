using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
	public class MainScreenSubNavButton : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private GameObject buttonActive   = null;
		[SerializeField] private GameObject buttonDeActive = null;

		#endregion

		#region Unity Methods

		public void SetSelected(bool isSelected)
		{
			buttonActive.SetActive(isSelected);
			buttonDeActive.SetActive(!isSelected);
		}

		#endregion
	}
}
