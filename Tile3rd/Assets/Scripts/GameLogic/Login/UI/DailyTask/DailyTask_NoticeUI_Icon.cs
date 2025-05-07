using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DailyTask_NoticeUI_Icon : WindowUI
{
    public static new string DefaultPrefabPath = "DailyTask/UI_DailyTask_Notice_Icon";
    private HomeUI home_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        register_button("Panel/DailyTask/Button/button_ok", on_dailytask_close_clicked);
        register_button("Panel/DailyTask/Button/button_claim", on_dailytask_claim_clicked);
        home_ui = _ui_manager.FindWindow<HomeUI>();
        IconShow();
        ConditionInit();
    }
    private void Update()
    {
        ShowCountDown();
    }
    private void ShowCountDown()
    {
        home_ui.dailyTask_icon.GetCountDown();
        var timeText = find_component<Text>("Panel/DailyTask/Guide/Guide_1/countdown/Text");
        if (home_ui.dailyTask_icon.diff > 3600)
            timeText.text = $"{home_ui.dailyTask_icon.hour}h{home_ui.dailyTask_icon.min}m";
        if (home_ui.dailyTask_icon.diff >= 60 && home_ui.dailyTask_icon.diff <= 3600)
            timeText.text = $"{home_ui.dailyTask_icon.min}m{home_ui.dailyTask_icon.sec}";
        if (home_ui.dailyTask_icon.diff < 60)
            timeText.text = $"{home_ui.dailyTask_icon.sec}";
    }
    private void IconShow()
    {
        //icon
        var makeoverlist = GameConfigManager.GameConfigGroup.MakeOverConfigList;
        var chainlist = GameConfigManager.GameConfigGroup.DailyTaskChainConfigList;
        var currentchain = chainlist.Find(a => a.ID == GameConfigManager.Tile2Storage.CurrentDailyTaskChainID);
        var currentitem = makeoverlist.Find(a => a.ID == currentchain.MakeOverImageID);

        var taskBG = find_component<Image>("Panel/DailyTask/Icon/icon/Background");
        var taskIcon = find_component<Image>("Panel/DailyTask/Icon/icon/Fill Area/Fill");
        taskBG.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
        taskIcon.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);

        //进度
        int sliderValue = 0;
        foreach (var item in currentchain.ChainList)
        {
            if (GameConfigManager.Tile2Storage.DailyTaskCondition[item] == 3)
                sliderValue++;
        }
        var slider = find_component<Slider>("Panel/DailyTask/Icon/icon");
        slider.value = sliderValue;
        slider.maxValue = currentchain.ChainList.Count;

    }
    private void ConditionInit()
    {
        var iconlock = find_component<RectTransform>("Panel/DailyTask/Icon/lock");
        var desc = find_component<Text>("Panel/DailyTask/Icon/decs/Text");
        iconlock.SetActive(false);

        var allchild = find_component<RectTransform>("Panel/DailyTask/Guide");
        foreach (Transform child in allchild)
            child.SetActive(false);
        var guide1 = find_component<RectTransform>("Panel/DailyTask/Guide/Guide_1");
        var guide2 = find_component<RectTransform>("Panel/DailyTask/Guide/Guide_2");
        var guide3 = find_component<RectTransform>("Panel/DailyTask/Guide/Guide_3");

        var button_ok = find_component<RectTransform>("Panel/DailyTask/Button/button_ok");
        var button_claim = find_component<RectTransform>("Panel/DailyTask/Button/button_claim");
        button_ok.SetActive(false);
        button_claim.SetActive(false);

        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 0)
        {
            iconlock.SetActive(true);
            guide1.SetActive(true);
            button_ok.SetActive(true);
            desc.text = "                    to unlock";
        }
            
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 1)
        {
            guide2.SetActive(true);
            button_ok.SetActive(true);
            desc.text = "Complete task to get rewards";
        }
            
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 2)
        {
            guide3.SetActive(true);
            button_claim.SetActive(true);
            desc.text = "Wow! Rewards unlocked";
        } 
    }
    private void on_dailytask_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        home_ui.dailyTask_hint.currentTask.ShowGuide();
    }
    private void on_dailytask_claim_clicked()
    {
        play_sound("sound_panel_closing");
        Close();

        //刷新到下一个任务链
        GameConfigManager.Tile2Storage.DailyTaskChainCondition[2] = 0;
        GameConfigManager.Tile2Storage.DailyTaskChainStartTime = DateTime.Now;

        //领取奖励
        UnlockRewardImage();

        //刷新icon
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        home_ui.dailyTask_icon.ChainFinish();
    }
    private void UnlockRewardImage()
    {
        var all_dailychain = GameConfigManager.GameConfigGroup.DailyTaskChainConfigList;
        var all_image = GameConfigManager.GameConfigGroup.MakeOverConfigList;
        var currentchain = all_dailychain.Find(a => a.ID == GameConfigManager.Tile2Storage.CurrentDailyTaskChainID);
        var rewardimage = all_image.Find(a => a.ID == currentchain.MakeOverImageID);

        //刷新到对应的story
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        GameConfigManager.MakeOverStorage.CurrentStoryID = rewardimage.StoryID;
        home_ui.MakeOverInit();
        home_ui.storyIcon.StoryTipInit();

        //自动点击touch
        home_ui.makeOver.MakeOverUI_OnSelect();
        home_ui.makeOver.makeOver_Select.LoveBarInit();

        //解锁家具
        var touch = home_ui.makeOver.CurrentStoryTouchList.Find(a => a.ImageIDList.Contains(currentchain.MakeOverImageID));
        foreach (var item in touch.ImageIDList)
        {
            var data = home_ui.makeOver.CurrentStoryImageList.Find(a => a.ID == item);
            if (data.ID == currentchain.MakeOverImageID)
            {
                //data.isUse = true;
                //data.isUnlock = true;
                GameConfigManager.MakeOverStorage.ImageUse[data.ID] = true;
                GameConfigManager.MakeOverStorage.ImageUnlock[data.ID] = true;
                home_ui.makeOver.makeOver_Select.SetTouchUnlock(data);
                GameConfigManager.Tile2Storage.LoveLevelExpUp = data.LoveExp;
                home_ui.makeOver.makeOver_Select.GetCatID();
                home_ui.makeOver.makeOver_Image.InitImageButtonList();
                home_ui.makeOver.makeOver_Select.AnimType = 2;
                home_ui.makeOver.makeOver_Select.SelectAnim();
            }
            else
            {
                data.isUse = false;
                GameConfigManager.MakeOverStorage.ImageUse[data.ID] = false;
            }
        }
    }
}