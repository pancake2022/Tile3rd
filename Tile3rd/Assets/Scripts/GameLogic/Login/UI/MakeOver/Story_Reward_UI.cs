using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Story_Reward_UI : WindowUI
{
    public static new string DefaultPrefabPath = "MakeOver/UI_Story_Reward";
    private List<ItemConfig> itemlist;
    private List<StoryConfig> allstory;
    private StoryConfig currentstory;
    private HomeUI home_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");

        itemlist = GameConfigManager.GameConfigGroup.ItemConfigList;
        allstory = GameConfigManager.GameConfigGroup.StoryConfigList;
        currentstory = allstory.Find(a => a.ID == GameConfigManager.MakeOverStorage.CurrentStoryID);
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

        if (GameConfigManager.MakeOverStorage.CurrentStoryID >= allstory.Count)
        {
            if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 3)
                Debug.Log("敬请期待");
            else
            {
                if (home_ui.makeOver.story_Reward_Condition == 1)//touch完成
                    GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] = 2;
                if (home_ui.makeOver.story_Reward_Condition == 2)//image完成
                    GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] = 3;
            }
        }
        else
        {
            if (home_ui.makeOver.story_Reward_Condition == 1)//touch完成
            {
                GameConfigManager.GiveItem(currentstory.TouchRewardID, currentstory.TouchRewardNum);
                GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] = 2;
                //获取下一个剧情story
                var nextstory = allstory.Find(a => a.Type == 1 && GameConfigManager.MakeOverStorage.StoryCondition[a.ID] == 0);
                if (nextstory != null) 
                {
                    GameConfigManager.MakeOverStorage.CurrentStoryID = nextstory.ID;
                    GameConfigManager.MakeOverStorage.UnlockMaxStoryID = GameConfigManager.MakeOverStorage.CurrentStoryID;
                    GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] = 1;
                }
            }
            if (home_ui.makeOver.story_Reward_Condition == 2)//image完成
            {
                GameConfigManager.GiveItem(currentstory.ImageRewardID, currentstory.ImageRewardNum);
                GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] = 3;
                GameConfigManager.MakeOverStorage.CurrentStoryID = GameConfigManager.MakeOverStorage.UnlockMaxStoryID;
            }
        }
        Close();
        home_ui.MakeOverInit();
        home_ui.SystemUnlock();
    }
}