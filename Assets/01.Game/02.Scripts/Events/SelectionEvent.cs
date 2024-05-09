using BBG.PictureColoring;

public struct SelectImageEvent
{
    public LevelData LevelData;
    public SelectImageEvent(LevelData levelData)
    {
        LevelData = levelData;
    }
}