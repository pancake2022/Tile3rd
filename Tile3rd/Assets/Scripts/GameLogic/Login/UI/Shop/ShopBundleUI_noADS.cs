using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ShopBundleUI_noADS : WindowUI
{
    public static new string DefaultPrefabPath = "Shop/UI_Bundle_shop_noADS";
    private ShopConfig shopConfig;
    private HomeUI home_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        register_button("Panel/Button_close", on_close_clicked);
        register_button("Panel/Button_ok", on_buy_clicked);

        RewardShow();
        PriceShow();
    }
    private void RewardShow()
    {
        //相当于9个广告
        //life
        var item1 = find_component<Image>("Panel/Reward_1/item_1");
        var item1_count = find_component<Text>("Panel/Reward_1/item_1/Text");
        item1.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_life", true);
        item1_count.text = $"x3";

        //recall
        var item2 = find_component<Image>("Panel/Reward_1/item_2");
        var item2_count = find_component<Text>("Panel/Reward_1/item_2/Text");
        item2.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_back", true);
        item2_count.text = $"x5";

        //remove
        var item3 = find_component<Image>("Panel/Reward_1/item_3");
        var item3_count = find_component<Text>("Panel/Reward_1/item_3/Text");
        item3.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_delete", true);
        item3_count.text = $"x5";

        //flower
        var item4 = find_component<Image>("Panel/Reward_1/item_4");
        var item4_count = find_component<Text>("Panel/Reward_1/item_4/Text");
        item4.sprite = _ui_manager.FindSprite($"M3Reward", $"icon_game_bloom", true);
        item4_count.text = $"x8";
    }
    private void PriceShow()
    {
        var all_shop = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().ShopConfigList;
        var shop = all_shop.Find(a => a.ID == 1);
        var price = find_component<Text>("Panel/Button_ok/Text");
        price.text = shop.Price.ToString();
    }
    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
    }
    private void on_buy_clicked()
    {
        play_sound("sound_button_click");
        _ui_manager.OpenWindow<ShopReward_noADS>();
        Close();
    }
}