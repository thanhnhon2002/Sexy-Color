using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
	public class DiscoverScreen : Screen
	{
		[SerializeField] private DiscoverCategoryListItem storyBoard;
		
		private DiscoverCategoryListItem[] _items;

		private void Awake()
		{
			_items = GetComponentsInChildren<DiscoverCategoryListItem>();
			// var categoryData = GameManager.Instance.Categories.FirstOrDefault(obj => obj.isStoryMode);
			// storyBoard.Populate(categoryData);
		}

		public override void OnShowing()
		{
			base.OnShowing();
			_items.ToList().ForEach(obj => obj.LoadData());
		}

		public void OnPremiumButtonClicked()
		{
			PopupManager.Instance.Show("vip_purchase_popup");
		}
	}
}
