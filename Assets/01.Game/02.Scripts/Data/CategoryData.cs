using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace BBG.PictureColoring
{
    [System.Serializable]
    public class CategoryData
    {
        public string          displayName;
        public string          description;
        public List<LevelData> levels;
        public Sprite          categoryImage;
        public bool            isVIP;
        public bool            isStoryMode;
        [MMInspectorGroup("Story Info", true, 22)]
        [MMCondition("isStoryMode", true)]
        public string          storyTitle;
        [MMCondition("isStoryMode", true)]
        public string                             storyDescription;
    }
    
    [Serializable]
    public class SuperCategoryData
    {
        public string       displayName;
        public List<string> categories;
        public List<CategoryData> categoryDatas;
    }
}