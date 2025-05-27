using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class VersionUpdate : BaseUI
{
    public LoginUI loginUI;
    private Tile2Storage tile2Storage;
    private LevelStorage levelStorage;
    private MakeOverStorage makeoverStorage;
    private GameConfigGroup gameConfigGroup;
    private GlobalConfig globalConfig;

    public VersionUpdate Init(LoginUI login)
    {
        loginUI = login;
        return this;
    }
    protected override void on_create()
    {
        tile2Storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        globalConfig = gameConfigGroup.GlobalConfigList[0];

        Version();
    }
    private void Version()
    {
        if (tile2Storage.Version < globalConfig.Version) 
        {
            tile2Storage.Version = globalConfig.Version;
            startmakeover();
            startmakeoverUnlock();
            starttouch();
            startstory();
            startcat();
            startcatquest();
            starttile();
            startbundle();
            startdailytaskchain();
            startdailytask();
            startsigncondition();
            startsignCD();
            startsignlevelCD();
            gamelevelcondition();
            //customercondition();
            //ordercondition();
            //Test();

            if (makeoverStorage.StoryCondition[globalConfig.Story_Num_Last] >= 2)
                makeoverStorage.UnlockMaxStoryID = globalConfig.Story_Num_Last + 1;

            Debug.Log("有版本更新");
        }
        else
            Debug.Log("无版本更新");
    }
    private void startmakeover()
    {
        var all_data = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().MakeOverConfigList;
        foreach (var data in all_data)
        {
            if (!makeoverStorage.ImageUse.ContainsKey(data.ID))
                makeoverStorage.ImageUse.Add(data.ID, data.isUse);
        }
    }
    private void startmakeoverUnlock()
    {
        var all_data = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().MakeOverConfigList;
        foreach (var data in all_data)
        {
            if (!makeoverStorage.ImageUnlock.ContainsKey(data.ID))
                makeoverStorage.ImageUnlock.Add(data.ID, data.isUnlock);
        }
    }
    private void starttouch()
    {
        var all_touchpoint = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().TouchPointConfigList;
        foreach (var touch in all_touchpoint)
        {
            if (!makeoverStorage.TouchPointCondition.ContainsKey(touch.ID))
            {
                makeoverStorage.TouchPointCondition.Add(touch.ID, 0);
                foreach (var touchcell in touch.Unlock)
                {
                    if (touchcell == 0)
                        makeoverStorage.TouchPointCondition[touch.ID] = 3;
                }
            }
        }
    }
    private void startstory()
    {
        var all_story = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().StoryConfigList;
        foreach (var story in all_story)
        {
            if (!makeoverStorage.StoryCondition.ContainsKey(story.ID))
                makeoverStorage.StoryCondition.Add(story.ID, 0);
        }
    }
    private void startcat()
    {
        var all_data = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().MakeOverConfigList;
        foreach (var data in all_data)
        {
            if (!makeoverStorage.CurrentCatID.ContainsKey(data.StoryID))
            {
                if (data.BuyType == 0)
                    makeoverStorage.CurrentCatID.Add(data.StoryID, data.CatID);
            }
        }
    }
    private void startcatquest()
    {
        var all_quest = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().QuestConfigList;
        foreach (var quest in all_quest)
        {
            if (!makeoverStorage.CatQuestCondition.ContainsKey(quest.ID))
                makeoverStorage.CatQuestCondition.Add(quest.ID, 0);
        }
    }
    private void starttile()
    {
        var all_tile = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().CollectionConfigList;
        int singTileID;
        foreach (var data in all_tile)
        {
            if (!tile2Storage.TileUnlock.ContainsKey(data.ID))
            {
                tile2Storage.TileUnlock.Add(data.ID, false);

                //第二套牌开始，记录每张单独的牌的状态
                singTileID = data.ID * 100;
                for (int i = 0; i < globalConfig.M3_MaxTypeCount; i++)
                {
                    singTileID++;
                    if (data.Type == 2)
                        tile2Storage.TileSingleUnlock.Add(singTileID, false);
                }
            }
        }
    }
    private void startbundle()
    {
        var all_bundle = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().BundleConfigList;
        foreach (var bundle in all_bundle)
        {
            if (!tile2Storage.BundleRV.ContainsKey(bundle.ID))
                tile2Storage.BundleRV.Add(bundle.ID, false);
        }
    }
    private void startdailytaskchain()
    {
        var all_dailytaskchain = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().DailyTaskChainConfigList;
        foreach (var dailytaskchain in all_dailytaskchain)
        {
            if (!tile2Storage.DailyTaskChainCondition.ContainsKey(dailytaskchain.ID))
                tile2Storage.DailyTaskChainCondition.Add(dailytaskchain.ID, 0);
        }
    }
    private void startdailytask()
    {
        var all_dailytask = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().DailyTaskConfigList;
        foreach (var dailytask in all_dailytask)
        {
            if (!tile2Storage.DailyTaskCondition.ContainsKey(dailytask.ID))
                tile2Storage.DailyTaskCondition.Add(dailytask.ID, 0);
        }
    }
    private void startsigncondition()
    {
        for (int i = 0; i < 8; i++)
        {
            if (!tile2Storage.SignCondition.ContainsKey(i))
                tile2Storage.SignCondition.Add(i, 0);//0-签到状态//1-7个奖励的状态
        }
    }
    private void startsignCD()
    {
        for (int i = 0; i < 11; i++)
        {
            if (!tile2Storage.SignCD.ContainsKey(i))
                tile2Storage.SignCD.Add(i, DateTime.Now);//0签到按钮的CD//1-7每日cd//8-签到CD//10-11奖励1和4的cd
        }
    }
    private void startsignlevelCD()
    {
        var all_sign = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().SignConfigList;
        foreach (var sign in all_sign)
        {
            if (!tile2Storage.SignLevelCD.ContainsKey(sign.ID))
                tile2Storage.SignLevelCD.Add(sign.ID, 0);
        }
    }
    private void gamelevelcondition()
    {
        var all_gamelevel = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().GameLevelConfigList;
        foreach (var gamelevel in all_gamelevel)
        {
            if (!levelStorage.GameLevel_Condition.ContainsKey(gamelevel.ID))
                levelStorage.GameLevel_Condition.Add(gamelevel.ID, false);
        }
    }
    //private void customercondition()
    //{
    //    var all_customer = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().CustomerConfigList;
    //    foreach (var customer in all_customer)
    //    {
    //        if (!levelStorage.Customer_Condition.ContainsKey(customer.ID))
    //            levelStorage.Customer_Condition.Add(customer.ID, 0);
    //    }
    //}
    //private void ordercondition()
    //{
    //    var all_order = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().CustomerOrderConfigList;
    //    foreach (var order in all_order)
    //    {
    //        if (!levelStorage.Order_Condition.ContainsKey(order.ID))
    //            levelStorage.Order_Condition.Add(order.ID, 0);
    //    }
    //}
}
