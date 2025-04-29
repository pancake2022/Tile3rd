using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class OutItemRV : WindowUI
{
    public static new string DefaultPrefabPath = "Shop/UI_Panel_outItem_RV";
    private GameUI game_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        game_ui = _ui_manager.FindWindow<GameUI>();
        game_ui.GamePause();

        register_button("Panel/Button/close", on_close_clicked);
        register_button("Panel/Button/ADS", on_ADS_clicked);
        TitleInit();
        PictureInit();
    }
    private void TitleInit()
    {
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        var text = find_component<Text>("Panel/Title/Text");
        if (shareDataGlobalConfig._bundle_type_id == 3)
            text.text = "Out Removes?";
        if (shareDataGlobalConfig._bundle_type_id == 4)
            text.text = "Out Recalls?";
        if (shareDataGlobalConfig._bundle_type_id == 5)
            text.text = "Out Blooms?";
    }
    private void PictureInit()
    {
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];

        var outremove = find_component<RectTransform>("Panel/Picture/outremove");
        var outrecall = find_component<RectTransform>("Panel/Picture/outrecall");
        var outbloom = find_component<RectTransform>("Panel/Picture/outbloom");
        var text = find_component<Text>("Panel/Picture/text/Text");
        outremove.SetActive(false);
        outrecall.SetActive(false);
        outbloom.SetActive(false);

        if (shareDataGlobalConfig._bundle_type_id == 3)
        {
            outremove.SetActive(true);
            text.text = "+ " + globalconfig.RV_Reward_Remove.ToString();
        }
        if (shareDataGlobalConfig._bundle_type_id == 4)
        {
            outrecall.SetActive(true);
            text.text = "+ " + globalconfig.RV_Reward_Recall.ToString();
        }
        if (shareDataGlobalConfig._bundle_type_id == 5)
        {
            outbloom.SetActive(true);
            text.text = "+ " + globalconfig.RV_Reward_Bloom.ToString();
        }
    }
    private void on_close_clicked()
    {
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        play_sound("sound_panel_closing");
        game_ui.GameActiveDelay();
        Close();

        //道具为0时，每3关判断局内主动弹出outui
        shareDataGlobalConfig._game_outitem_jump = 3;
    }
    private void on_ADS_clicked()
    {
        play_sound("sound_button_click");
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        if (shareDataGlobalConfig._bundle_type_id == 3)
            ADSManager.TriggerADSShow_Reward("Item_Remove");
        if (shareDataGlobalConfig._bundle_type_id == 4)
            ADSManager.TriggerADSShow_Reward("Item_Recall");
        if (shareDataGlobalConfig._bundle_type_id == 5)
            ADSManager.TriggerADSShow_Reward("Item_Bloom");
    }
}