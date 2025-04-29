using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ReviveUI : WindowUI
{
    public static new string DefaultPrefabPath = "Game/UI_Panel_revive";
    private GameUI game_ui;
    private ShareDataGlobalConfig shareDataGlobalConfig;
    private CommonStorage commonStorage;
    private Tile2Storage tile2Storage;
    private LevelStorage levelStorage;
    private GameConfigGroup gameConfigGroup;
    private GlobalConfig globalconfig;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_level_failed");
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
        tile2Storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        globalconfig = gameConfigGroup.GlobalConfigList[0];

        game_ui = _ui_manager.FindWindow<GameUI>();
        game_ui.GamePause();

        register_button("Panel/Button/close", on_close_clicked);
        register_button("Panel/Button/playon", on_life_clicked);
        register_button("Panel/Button/playout", on_rv_clicked);

        //停止音乐
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._game_music_id);
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._game_music_bloom);

        UIShow();
    }

    private void on_life_clicked()
    {
        shareDataGlobalConfig.itemlist[0]++;
        commonStorage.Item_Life--;
        PlayOut();
    }
    public void PlayOut()
    {
        play_sound("sound_spend_coins");
        game_ui.gameItemGroupUI.Revive();
        Close();

        //复活给双倍钻石掉落buff
        if (levelStorage.LevelCount > globalconfig.Unlock_Revive_BloomBuff)
        {
            game_ui.ReviveBloomMusic();
            game_ui.gameRewardItem.BloomBuff = true;
            game_ui.gameRewardItem.BloomTimes = game_ui.gameRewardItem.BloomTimes + globalconfig.Bloom_Times_Life;
        }
        else
            game_ui.SetGameMusic();
    }
    private void on_rv_clicked()
    {
        ADSManager.TriggerADSShow_Reward("Revive_Life");
    }

    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
        //if (tile2Storage.WinStreakCount >= 1)
        if (shareDataGlobalConfig._is_winstreak) 
        {
            shareDataGlobalConfig._winstreak_notice_type = 2;
            _ui_manager.OpenWindow<DailyTask_NoticeUI_WinStreak>();
        }
        else
            _ui_manager.OpenWindow<RetryUI>();
    }
    private void UIShow()
    {
        CoinBar();
        RewardShow();
        ButtonInit();
    }
    private void RewardShow()
    {
        //显示item数量
        var item_1 = find_component<RectTransform>("Panel/Picture/reward_1");
        var item_2 = find_component<RectTransform>("Panel/Picture/reward_2");
        var textitem_1 = find_component<Text>("Panel/Picture/reward_1/Text");
        var textitem_2 = find_component<Text>("Panel/Picture/reward_2/Text_2");
        var game_ui = _ui_manager.FindWindow<GameUI>();
        textitem_1.text = "x3";
        textitem_2.text = "+" + globalconfig.Bloom_Times_Life.ToString();

        //前3关不显示bloom
        if(levelStorage.LevelCount > 3)
        {
            item_1.SetActive(true);
            item_2.SetActive(true);
        }
        else
        {
            item_1.SetActive(true);
            item_2.SetActive(false);
        }
    }
    private void ButtonInit()
    {
        var playon = find_component<RectTransform>("Panel/Button/playon");
        var playout = find_component<RectTransform>("Panel/Button/playout");
        playon.SetActive(false);
        playout.SetActive(false);

        if (commonStorage.Item_Life >= 1)
            playon.SetActive(true);
        else
            playout.SetActive(true);
    }
    private void CoinBar()
    {
        //金币栏显示
        var Revive_Num = find_component<Text>("ItemBar/Text");
        Revive_Num.text = commonStorage.Item_Life.ToString();
    }
}