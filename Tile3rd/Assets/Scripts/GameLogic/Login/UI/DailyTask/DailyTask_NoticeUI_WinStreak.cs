using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DailyTask_NoticeUI_WinStreak : WindowUI
{
    public static new string DefaultPrefabPath = "DailyTask/UI_DailyTask_Notice_WinStreak";
    private GameUI game_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        game_ui = _ui_manager.FindWindow<GameUI>();
        game_ui.GamePause();

        register_button("Panel/WinStreak/Button/close", on_close_clicked);
        register_button("Panel/WinStreak/Button/revive_life", on_life_clicked);
        register_button("Panel/WinStreak/Button/revive_rv", on_rv_clicked);
        register_button("Panel/WinStreak/Button/play_on", on_playon_clicked);

        IconShow();
        LifeBar();
        ButtonShow();
    }
    private void IconShow()
    {
        //icon显示
        var itemlist = GameConfigManager.GameConfigGroup.ItemConfigList;
        var tasklist = GameConfigManager.GameConfigGroup.DailyTaskConfigList;
        var currenttask = tasklist.Find(a => a.ID == GameConfigManager.Tile2Storage.CurrentDailyTaskID);
        var currentitem = itemlist.Find(a => a.ID == currenttask.RewardID);

        var taskBG = find_component<Image>("Panel/WinStreak/Guide/Guide_1/DailyTask/icon/Background");
        var taskIcon = find_component<Image>("Panel/WinStreak/Guide/Guide_1/DailyTask/icon/Fill Area/Fill");
        var rewardText = find_component<Text>("Panel/WinStreak/Guide/Guide_1/DailyTask/icon/Fill Area/Text");
        taskBG.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
        taskIcon.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
        rewardText.text = $"+{currenttask.RewardCount}";

        var taskBG2 = find_component<Image>("Panel/WinStreak/Guide/Guide_2/DailyTask/icon/Background");
        var taskIcon2 = find_component<Image>("Panel/WinStreak/Guide/Guide_2/DailyTask/icon/Fill Area/Fill");
        var rewardText2 = find_component<Text>("Panel/WinStreak/Guide/Guide_2/DailyTask/icon/Fill Area/Text");
        taskBG2.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
        taskIcon2.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
        rewardText2.text = $"+{currenttask.RewardCount}";

        //显示进度
        var slider = find_component<Slider>("Panel/WinStreak/Guide/Guide_1/DailyTask/icon");
        slider.value = GameConfigManager.Tile2Storage.WinStreakCount + 1;
        slider.maxValue = currenttask.TaskCount;

        //picture按钮显示
        var button1 = find_component<RectTransform>("Panel/WinStreak/Guide/Guide_1/DailyTask/describe/start");
        var button2 = find_component<RectTransform>("Panel/WinStreak/Guide/Guide_1/DailyTask/describe/claim");
        button1.SetActive(false);
        button2.SetActive(false);
        if (slider.value >= currenttask.TaskCount)
            button2.SetActive(true);
        else
            button1.SetActive(true);
    }
    private void LifeBar()
    {
        var lifecount = find_component<Text>("Panel/ItemBar/Text");
        lifecount.text = $"{GameConfigManager.CommonStorage.Item_Life}";
    }
    private void ButtonShow()
    {
        //按钮显示
        var lifeicon = find_component<RectTransform>("Panel/WinStreak/Guide/Icon");
        var button_life = find_component<RectTransform>("Panel/WinStreak/Button/revive_life");
        var button_rv = find_component<RectTransform>("Panel/WinStreak/Button/revive_rv");
        var button_playon = find_component<RectTransform>("Panel/WinStreak/Button/play_on");
        lifeicon.SetActive(false);
        button_life.SetActive(false);
        button_rv.SetActive(false);
        button_playon.SetActive(false);

        //通过gamesetting点进来
        if (GameConfigManager.ShareDataGlobalConfig._winstreak_notice_type == 1) 
            button_playon.SetActive(true);

        //通过revive点进来
        if (GameConfigManager.ShareDataGlobalConfig._winstreak_notice_type == 2)
        {
            if (GameConfigManager.CommonStorage.Item_Life >= 1)
            {
                button_life.SetActive(true);
                lifeicon.SetActive(true);
            }
            else
                button_rv.SetActive(true);
        }
    }
    private void on_playon_clicked()
    {
        play_sound("sound_button_click");
        Close();
        game_ui.GameActiveDelay();
    }
    private void on_close_clicked()
    {
        //重置winstreak值
        play_sound("sound_level_failed");
        GameConfigManager.Tile2Storage.WinStreakCount = 0;
        Close();

        //从setting进入
        if (GameConfigManager.ShareDataGlobalConfig._winstreak_notice_type == 1)
        {
            _ui_manager.TryCloseWindow<GameUI>();
            _ui_manager.OpenWindow<HomeUI>();
        }
        //从revive进入
        if (GameConfigManager.ShareDataGlobalConfig._winstreak_notice_type == 2)
            _ui_manager.OpenWindow<RetryUI>();
    }
    private void on_life_clicked()
    {
        play_sound("sound_spend_coins");
        GameConfigManager.CommonStorage.Item_Life--;
        GameConfigManager.ShareDataGlobalConfig.itemlist[0]++;
        PlayOn();
    }
    public void PlayOn()
    {
        game_ui.gameItemGroupUI.Revive();
        Close();

        //复活给双倍钻石掉落buff
        if (GameConfigManager.LevelStorage.LevelCount >= GameConfigManager.GlobalConfig.Unlock_Revive_BloomBuff) 
        {
            game_ui.ReviveBloomMusic();
            game_ui.gameRewardItem.BloomBuff = true;
            game_ui.gameRewardItem.BloomTimes = game_ui.gameRewardItem.BloomTimes + GameConfigManager.GlobalConfig.Bloom_Times_Life;
        }
        else
            game_ui.SetGameMusic();
    }
    private void on_rv_clicked()
    {
        play_sound("sound_button_click");
        ADSManager.TriggerADSShow_Reward("Revive_WinStreak");
    }
}