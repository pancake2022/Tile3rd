using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShopReward_noADS : WindowUI
{
    public static new string DefaultPrefabPath = "Reward/UI_Reward_shop_noADS";

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_level_win");
        register_button("Panel/Button/button_claim", on_claim_clicked);
        RewardShow();
        GetReward();
    }
    private void on_claim_clicked()
    {
        play_sound("sound_button_click");
        RefreshUI();
        Close();
    }
    private void RewardShow()
    {
        var item1 = find_component<Image>("Panel/UI_reward_2/item_1");
        var item1_count = find_component<Text>("Panel/UI_reward_2/item_1/Text");
        item1.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_life", true);
        item1_count.text = $"x3";

        //recall
        var item2 = find_component<Image>("Panel/UI_reward_2/item_2");
        var item2_count = find_component<Text>("Panel/UI_reward_2/item_2/Text");
        item2.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_back", true);
        item2_count.text = $"x5";

        //remove
        var item3 = find_component<Image>("Panel/UI_reward_2/item_3");
        var item3_count = find_component<Text>("Panel/UI_reward_2/item_3/Text");
        item3.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_delete", true);
        item3_count.text = $"x5";

        //flower
        var item4 = find_component<Image>("Panel/UI_reward_2/item_4");
        var item4_count = find_component<Text>("Panel/UI_reward_2/item_4/Text");
        item4.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_bloom", true);
        item4_count.text = $"x8";
    }
    private void GetReward()
    {
        var commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];

        //设置此礼包已经被购买过
        tile2storage.isnoADS = true;
        globalconfig.Interstitial_CD_Initial = 0;

        //获得道具
        commonStorage.Item_Life = commonStorage.Item_Life + 3;
        commonStorage.Item_Recall = commonStorage.Item_Recall + 5;
        commonStorage.Item_Remove = commonStorage.Item_Remove + 5;
        commonStorage.Item_Bloom = commonStorage.Item_Bloom + 8;
    }
    private void RefreshUI()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        tile2storage.ShopRefreshCD = DateTime.Now;
        home_ui.shopBundleIcon.ChangeCurrentShop();
        home_ui.shopBundleIcon.IconInit();
    }
}
