using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Story_Reward_UI : WindowUI
{
    public static new string DefaultPrefabPath = "MakeOver/UI_Story_Reward";
    private CommonStorage commonStorage;
    private Tile2Storage tile2storage;
    private MakeOverStorage makeoverStorage;
    private List<ItemConfig> itemlist;
    private List<StoryConfig> allstory;
    private StoryConfig currentstory;
    private HomeUI home_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");

        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
        tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        itemlist = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().ItemConfigList;
        allstory = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().StoryConfigList;
        currentstory = allstory.Find(a => a.ID == makeoverStorage.CurrentStoryID);
        home_ui = _ui_manager.FindWindow<HomeUI>();

        _ui_manager.Framework.AudioManager.PlaySound("sound_level_win");
        _ui_manager.Framework.AudioManager.PlaySound("sound_chest_appear");
        register_button("Panel/Button/button_claim", on_claim_clicked);
        RewardInit();
    }
    private void RewardInit()
    {
        //显示道具/道具数量
        //分为touch奖励
        //image奖励
        var itemicon = find_component<Image>($"Panel/UI_reward/item_1/Image");
        var itemnum = find_component<Text>($"Panel/UI_reward/item_1/Image/Text");

        if (home_ui.makeOver.story_Reward_Condition == 1)//touch完成
        {
            var item = itemlist.Find(a => a.ID == currentstory.TouchRewardID);
            itemicon.sprite = _ui_manager.FindSprite($"{item.Pack}", $"{item.Icon}", true);
            itemnum.text = $"{currentstory.TouchRewardNum}";
        }

        if (home_ui.makeOver.story_Reward_Condition == 2)//image完成/目前只能给道具类奖励
        {
            var item = itemlist.Find(a => a.ID == currentstory.ImageRewardID);
            itemicon.sprite = _ui_manager.FindSprite($"{item.Pack}", $"{item.Icon}", true);
            if (item.Type == 1)
                itemnum.text = $"{currentstory.ImageRewardNum}";
        }
    }

    private void on_claim_clicked()
    {
        play_sound("sound_button_click");

        if (makeoverStorage.CurrentStoryID >= allstory.Count)
        {
            if (makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] == 3)
                Debug.Log("敬请期待");
            else
            {
                if (home_ui.makeOver.story_Reward_Condition == 1)//touch完成
                    makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] = 2;
                if (home_ui.makeOver.story_Reward_Condition == 2)//image完成
                    makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] = 3;
            }
        }
        else
        {
            if (home_ui.makeOver.story_Reward_Condition == 1)//touch完成
            {
                ItemClaim(currentstory.TouchRewardID, currentstory.TouchRewardNum);
                makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] = 2;
                //获取下一个剧情story
                var nextstory = allstory.Find(a => a.Type == 1 && makeoverStorage.StoryCondition[a.ID] == 0);
                if (nextstory != null) 
                {
                    makeoverStorage.CurrentStoryID = nextstory.ID;
                    makeoverStorage.UnlockMaxStoryID = makeoverStorage.CurrentStoryID;
                    makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] = 1;
                }
            }
            if (home_ui.makeOver.story_Reward_Condition == 2)//image完成
            {
                ItemClaim(currentstory.ImageRewardID, currentstory.ImageRewardNum);
                makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] = 3;
                makeoverStorage.CurrentStoryID = makeoverStorage.UnlockMaxStoryID;
            }
        }
        Close();
        home_ui.MakeOverInit();
        home_ui.SystemUnlock();
    }

    private void ItemClaim(int itemID, int itemNum)
    {
        foreach (var item in itemlist)
        {
            if (itemID == 1)
            {
                commonStorage.Flower = commonStorage.Flower + itemNum;
                return;
            }
                
            if (itemID == 2)
            {
                commonStorage.Item_Remove = commonStorage.Item_Remove + itemNum;
                return;
            }
                
            if (itemID == 3)
            {
                commonStorage.Item_Recall = commonStorage.Item_Recall + itemNum;
                return;
            }
                
            if (itemID == 4)
            {
                commonStorage.Item_Bloom = commonStorage.Item_Bloom + itemNum;
                return;
            }
                
            if (itemID == 5)
            {
                commonStorage.Item_Life = commonStorage.Item_Life + itemNum;
                return;
            }
        }
    }
}