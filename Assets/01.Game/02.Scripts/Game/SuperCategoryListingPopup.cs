using UnityEngine;

namespace BBG.PictureColoring
{
    public class SuperCategoryListingPopup:Popup
    {
        [SerializeField] private CategoryFeatureItem categoryParent;
        public override void OnShowing(object[] inData)
        {
            string categoryName = inData[0].ToString();
            if (categoryName != "")
            {
                categoryParent.Populate(categoryName);
            }
            base.OnShowing(inData);
        }
    }
}