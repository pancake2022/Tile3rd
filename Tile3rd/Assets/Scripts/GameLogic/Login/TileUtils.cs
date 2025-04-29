using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TileUtils
{
    //拿到当前关的Config的方法
    public static CSFramework.LevelConfig GetCurrentLevelConfig(int currentLevelID, ConfigManager configManager)
    {
        var config = configManager.SingleConfigGroup<GameConfigGroup>().LevelConfigList.Find(a => a.ID == currentLevelID);
        return config ?? configManager.SingleConfigGroup<GameConfigGroup>().LevelConfigList.FirstElement();
    }

    //拿到下一关的Config的方法
    public static CSFramework.LevelConfig GetNextLevelConfig(int currentLevelID, ConfigManager configManager)
    {
        var nextLevelID = currentLevelID + 1;
        return configManager.SingleConfigGroup<GameConfigGroup>().LevelConfigList.Find(a => a.ID == nextLevelID);
    }
}