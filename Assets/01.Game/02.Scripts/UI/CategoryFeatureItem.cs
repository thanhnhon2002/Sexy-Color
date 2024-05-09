using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BBG;
using BBG.PictureColoring;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryFeatureItem : MonoBehaviour
{
    [SerializeField] private Transform       categoryFeatureParent;
    [SerializeField] private GameObject      categoryFeaturePrefab;
    [SerializeField] private string          superCategoryName;
    [SerializeField] private TextMeshProUGUI superCategoryText;
    [SerializeField] private Button seeAllButton;
    
    private List<CategoryData> _categoryDatas;

    private void OnEnable()
    {
        if (seeAllButton != null)
        {
            seeAllButton.onClick.AddListener(OnSeeAllButtonClicked);
        }
    }

    private void OnSeeAllButtonClicked()
    {
        PopupManager.Instance.Show("supercategory_listing", new []{superCategoryName});
    }

    private void Start()
    {
        Populate(superCategoryName);
    }

    public void Populate(string categoryName)
    {
        superCategoryName = categoryName;
        _categoryDatas = GameManager.Instance.SuperCategories
            .FirstOrDefault(obj => obj.displayName.Equals(superCategoryName)).categoryDatas;
        if(superCategoryText != null)
            superCategoryText.text = superCategoryName;
        categoryFeatureParent.MMDestroyAllChildren();
        foreach (var categoryData in _categoryDatas)
        {
            var item = Instantiate(categoryFeaturePrefab, categoryFeatureParent).GetComponent<DiscoverCategoryListItem>();
            item.Populate(categoryData);
        }
    }

}
