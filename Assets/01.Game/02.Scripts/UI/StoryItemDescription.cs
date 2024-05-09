using System.Collections;
using System.Collections.Generic;
using BBG.PictureColoring;
using TMPro;
using UnityEngine;

public class StoryItemDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    
    public void Populate(LevelData levelData)
    {
        title.text = levelData.displayName;
        description.text = levelData.description;
    }
}
