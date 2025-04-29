using CSFramework;
using System.Collections;
using System;
using System.Collections.Generic;

public class LoginScene : BaseScene
{
    protected override IEnumerator on_init (params object[] param_list)
    {
        Framework.ShareDataManager.ClearAllData(); // 进入登录界面清除所有的共享数据
        initFlow();
        Framework.UIManager.OpenWindow<LoginUI>();
        yield return null;
    }
    
    /// <summary>
    /// 初始化流程
    /// </summary>
    private void initFlow ()
    {
        //判断是否新玩家
        var commonStorage = Framework.StorageManager.Storage<CommonStorage>();
        if (commonStorage.CheckCommonTagExecute(CommonTag.NewPlayer, initNewPlayer))
        {
            UnityEngine.Debug.Log("新玩家进入游戏");
        }
        else
        {
            UnityEngine.Debug.Log("老玩家进入游戏");
        }
    }

    /// <summary>
    /// 只有是新玩家才会执行这个方法
    /// </summary>
    private void initNewPlayer ()
    {
        // 获取全局配置
        var commonStorage = Framework.StorageManager.Storage<CommonStorage>();
        var tile2Storage = Framework.StorageManager.Storage<Tile2Storage>();
        var makeoverStorage = Framework.StorageManager.Storage<MakeOverStorage>();
        var gameConfigGroup = Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalConfig = gameConfigGroup.GlobalConfigList[0];

        //初始化关卡
        var levelStorage = Framework.StorageManager.Storage<LevelStorage>();
        var level1Config = gameConfigGroup.LevelConfigList[0];
        levelStorage.CurrentLevel = level1Config.ID;

        //初始化道具
        commonStorage.Flower = globalConfig.Flower_Initial;
        commonStorage.Item_Recall = globalConfig.Item_Recall_Initial;
        commonStorage.Item_Remove = globalConfig.Item_Remove_Initial;
        commonStorage.Item_Bloom = globalConfig.Item_Bloom_Initial;
        commonStorage.Item_Life = globalConfig.Item_Life_Initial;

        //初始化tile
        tile2Storage.CurrentTileID = gameConfigGroup.CollectionConfigList[0].ID;
        tile2Storage.TileUnlock[tile2Storage.CurrentTileID] = true;

        //初始化音乐&音效
        commonStorage.MusicOpen = true;
        commonStorage.SoundOpen = true;

        //初始化剧情ID
        var story1Config = gameConfigGroup.StoryConfigList[0];
        makeoverStorage.CurrentStoryID = story1Config.ID;
        makeoverStorage.StoryCondition.Add(makeoverStorage.CurrentStoryID, 1);
        makeoverStorage.CurrentCatID.Add(1, 0);//剧情/猫ID

        //初始化catquest的id
        makeoverStorage.CurrentQuest = gameConfigGroup.QuestConfigList[0];

        //初始化猫好感度等级
        tile2Storage.LoveLevelLevel = 1;

        //初始化dailytask任务链的状态
        var firstdailytaskchain = gameConfigGroup.DailyTaskChainConfigList[0];
        tile2Storage.CurrentDailyTaskChainID = firstdailytaskchain.ID;
        tile2Storage.DailyTaskChainCondition.Add(tile2Storage.CurrentDailyTaskChainID, 1);

        //初始化dailytask任务的状态
        var firstdailytask = gameConfigGroup.DailyTaskConfigList[0];
        tile2Storage.CurrentDailyTaskID = firstdailytask.ID;
        tile2Storage.DailyTaskCondition.Add(tile2Storage.CurrentDailyTaskID, 1);

        //初始化每日签到
        tile2Storage.SignCondition[0] = 1;
        tile2Storage.IsSignUnlock = false;

        //初始化商店礼包
        tile2Storage.CurrentShop = gameConfigGroup.ShopConfigList[0];

        //初始化倒计时
        //commonStorage.LeftTime = 0;
    }

    protected override IEnumerator on_cleanup()
    {
        yield return null;
    }
}