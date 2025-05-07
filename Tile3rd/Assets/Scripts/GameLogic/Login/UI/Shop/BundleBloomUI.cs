using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class BundleBloomUI : WindowUI
{
    public static new string DefaultPrefabPath = "Shop/UI_Bundle_bloom";

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");

        register_button("Panel/Button_close", on_close_clicked);
        register_button("Panel/Button_buy", on_buy_clicked);
        register_button("Panel/Button_rv", on_rv_clicked);

        ButtonInit();
    }
    private void ButtonInit()
    {
        var green = find_component<RectTransform>("Panel/Button_buy");
        var blue = find_component<RectTransform>("Panel/Button_rv");
        var close = find_component<RectTransform>("Panel/Button_close");
        green.SetActive(false);
        blue.SetActive(false);
        close.SetActive(true);

        if (GameConfigManager.Tile2Storage.BloomBuffFirst)
            blue.SetActive(true);
        else
        {
            green.SetActive(true);
            close.SetActive(false);
        }
    }

    private void on_close_clicked()
    {
        Close();
        play_sound("sound_panel_closing");
        BloomUICheck();
    }

    private void on_buy_clicked()
    {
        Close();
        play_sound("sound_button_click");
        GetBloom();
    }
    private void on_rv_clicked()
    {
        Close();
        play_sound("sound_button_click");
        ADSManager.TriggerADSShow_Reward("Bundle_Bloom");
    }
    public void GetBloom()
    {
        var home_ui = _ui_manager.FindWindow<HomeUI>();

        GameConfigManager.Tile2Storage.BloomBuffFirst = true;
        GameConfigManager.Tile2Storage.BloomBuffTimes = GameConfigManager.GlobalConfig.Bloom_Bunlde_BloomTimes;
        home_ui.BloomBuffInit();
        if (home_ui.signIcon != null)
            home_ui.signIcon.RefreshSignIconButton();
        BloomUICheck();
    }
    private void BloomUICheck()
    {
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        home_ui.levelChest._bloombuff_check = true;
        home_ui.levelChest.ButtonInit();
        home_ui.BundleBloomInit();
    }
}