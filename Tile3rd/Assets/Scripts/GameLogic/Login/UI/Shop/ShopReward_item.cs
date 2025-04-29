using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShopReward_item : WindowUI
{
    public static new string DefaultPrefabPath = "Reward/UI_Reward_shop_item";

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
        item1_count.text = $"x1";

        //recall
        var item2 = find_component<Image>("Panel/UI_reward_2/item_2");
        var item2_count = find_component<Text>("Panel/UI_reward_2/item_2/Text");
        item2.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_back", true);
        item2_count.text = $"x3";

        //remove
        var item3 = find_component<Image>("Panel/UI_reward_2/item_3");
        var item3_count = find_component<Text>("Panel/UI_reward_2/item_3/Text");
        item3.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_delete", true);
        item3_count.text = $"x3";

        //flower
        var item4_count = find_component<Text>("Panel/UI_reward_2/item_4/Text");
        item4_count.text = $"x5";
    }
    private void GetReward()
    {
        var commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        commonStorage.Item_Life = commonStorage.Item_Life + 1;
        commonStorage.Item_Recall = commonStorage.Item_Recall + 3;
        commonStorage.Item_Remove = commonStorage.Item_Remove + 3;
        tile2storage.BloomAllTimes = tile2storage.BloomAllTimes + 5;

        //补满bloombuff
        tile2storage.BloomBuffTimes = 5;
    }
    private void RefreshUI()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        home_ui.BloomBuffInit();
        home_ui.levelChest.ButtonInit();
        home_ui.BundleBloomInit();
        tile2storage.ShopRefreshCD = DateTime.Now;
        home_ui.shopBundleIcon.ChangeCurrentShop();
        home_ui.shopBundleIcon.IconInit();
    }
}
