using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class LoveLevelRewardUI : WindowUI
{
    public static new string DefaultPrefabPath = "Reward/UI_Reward_lovelevel";

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_level_win");
        register_button("Panel/Button/claim", on_close_clicked);
        LoveLevelReward();
    }
    
    private void LoveLevelReward()
    {
        var rewardIcon = find_component<Image>("Panel/UI_reward/item_1/Image");
        var rewardCount = find_component<Text>("Panel/UI_reward/item_1/Image/Text");
        var rewardtextshow = find_component<RectTransform>("Panel/UI_reward/item_1/Image/Text");
        var lovelevel = find_component<Text>("Panel/UI_Title/Bubble/image/image/Text");
        rewardtextshow.SetActive(false);
        lovelevel.text = $"Lv{ GameConfigManager.Tile2Storage.LoveLevelLevel + 1}";

        var lovelevellist = GameConfigManager.GameConfigGroup.LoveLevelConfigList;
        var itemlist = GameConfigManager.GameConfigGroup.ItemConfigList;
        var tilelist = GameConfigManager.GameConfigGroup.CollectionConfigList;
        var currentlovelevel = lovelevellist.Find(a => a.Level == GameConfigManager.Tile2Storage.LoveLevelLevel);
        var currentitem = itemlist.Find(a => a.ID == currentlovelevel.RewardID);
        //给资源
        if (currentitem.Type == 1)
        {
            GameConfigManager.CommonStorage.Flower = GameConfigManager.CommonStorage.Flower + currentlovelevel.RewardNum;
            rewardIcon.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
            rewardtextshow.SetActive(true);
            rewardCount.text = "+ " + currentlovelevel.RewardNum.ToString();
        }
        //给tile
        if (currentitem.Type == 2)
        {
            //给奖励
            GameConfigManager.Tile2Storage.TileUnlock[currentitem.TileID] = true;

            //设置奖励显示
            GameConfigManager.Tile2Storage.CurrentTileID = currentitem.TileID;
            var tile = tilelist.Find(a => a.ID == currentitem.TileID);
            rewardIcon.sprite = _ui_manager.FindSprite($"{tile.IconPack}", $"{tile.Icon}", true);
            rewardtextshow.SetActive(false);
        }
    }
    private void on_close_clicked()
    {
        play_sound("sound_button_click");
        Close();
        GameConfigManager.Tile2Storage.LoveLevelLevel++;
        GameConfigManager.Tile2Storage.LoveLevelExp = GameConfigManager.Tile2Storage.LoveLevelExp - 100;
        GameConfigManager.ShareDataGlobalConfig._love_exp_pause = false;
    }
}