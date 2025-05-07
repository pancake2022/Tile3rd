using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StoryUI : WindowUI
{
    public class StoryButton : BaseUI
    {
        public Action<StoryButton> ClickCalback;
        public StoryConfig storyConfig;
        private RectTransform condition_lock;
        private RectTransform condition_touch;
        private RectTransform condition_image;
        private RectTransform condition_complete;
        private Coffee.UIExtensions.UIEffect mater;
        private RectTransform button;
        private RectTransform tip;
        private RectTransform bubble;

        protected override void on_create()
        {
            mater = find_component<Coffee.UIExtensions.UIEffect>("Panel/BG/image");
            condition_lock = find_component<RectTransform>("Panel/Condition/lock");
            condition_touch = find_component<RectTransform>("Panel/Condition/touch");
            condition_image = find_component<RectTransform>("Panel/Condition/image");
            condition_complete = find_component<RectTransform>("Panel/Condition/complete");
            button = find_component<RectTransform>("Panel/Button");
            tip = find_component<RectTransform>("Panel/Button/tips");
            bubble = find_component<RectTransform>("Panel/Reward/Bubble");
            register_button("Panel/Button/Button", on_clicked);
        }
        private void SetBase()
        {
            SetTitle();
            SetFrame();
            SetBack();
            SetButton();
            tip.SetActive(false);
        }
        private void SetTitle()
        {
            var title = find_component<Text>("Panel/Title/Text");
            title.text = $"Chapter {storyConfig.Name}";
        }
        private void SetFrame()
        {
            var frame1 = find_component<RectTransform>("Panel/BackPanel/back1");
            var frame2 = find_component<RectTransform>("Panel/BackPanel/back2");
            frame1.SetActive(false);
            frame2.SetActive(false);
            if (storyConfig.Type == 1)
                frame1.SetActive(true);
            if (storyConfig.Type == 2)
                frame2.SetActive(true);
        }
        private void SetBack()
        {
            var back = find_component<Image>("Panel/BG/image");
            back.sprite = _ui_manager.FindSprite($"{storyConfig.Pack}", $"{storyConfig.Back}", true);
        }
        private void SetButton()
        {
            button.SetActive(true);
            //if (storyConfig.ID == makeoverStorage.CurrentStoryID)
            //    button.SetActive(false);
            if (GameConfigManager.MakeOverStorage.StoryCondition[storyConfig.ID] == 0)
                button.SetActive(false);
        }
        private void SetTips(int value)
        {
            //如果先进入选故事的界面，tip就会有问题
            var all_touch = GameConfigManager.GameConfigGroup.TouchPointConfigList;
            var all_image = GameConfigManager.GameConfigGroup.MakeOverConfigList;
            tip.SetActive(false);
            if (value == 1)
            {
                var touch = all_touch.Find(a => GameConfigManager.MakeOverStorage.TouchPointCondition[a.ID] == 1 && a.StoryID == storyConfig.ID);
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType == 1 && GameConfigManager.CommonStorage.Flower >= a.BuyPrice);
                    if (image != null) 
                        tip.SetActive(true);
                }
            }
            if (value == 2)
            {
                var touch = all_touch.Find(a => GameConfigManager.MakeOverStorage.TouchPointCondition[a.ID] == 2 && a.StoryID == storyConfig.ID);
                if (touch != null)
                {
                    foreach (var imageID in touch.ImageIDList)
                    {
                        var image = all_image.Find(a => a.ID == imageID);
                        if (GameConfigManager.MakeOverStorage.ImageUnlock[image.ID] == false && image.BuyType == 1)
                        {
                            if (GameConfigManager.CommonStorage.Flower >= image.SecondPrice)
                                tip.SetActive(true);
                        }
                    }
                }
            }
        }
        private void SetCondition()
        {
            SetBaseCondition();
            if (GameConfigManager.MakeOverStorage.StoryCondition[storyConfig.ID] == 0)
                SetLockCondition();
            if (GameConfigManager.MakeOverStorage.StoryCondition[storyConfig.ID] == 1)
                SetTouchCondition();
            if (GameConfigManager.MakeOverStorage.StoryCondition[storyConfig.ID] == 2)
                SetImageCondition();
            if (GameConfigManager.MakeOverStorage.StoryCondition[storyConfig.ID] == 3)
                SetCompleteCondition();
        }
        private void SetBaseCondition()
        {
            mater.enabled = false;
            condition_lock.SetActive(false);
            condition_touch.SetActive(false);
            condition_image.SetActive(false);
            condition_complete.SetActive(false);
        }
        private void SetLockCondition()
        {
            mater.enabled = true;
            condition_lock.SetActive(true);
        }
        private void SetTouchCondition()
        {
            condition_touch.SetActive(true);
            SetTouchSlider();
            SetTips(1);
        }
        private void SetImageCondition()
        {
            condition_image.SetActive(true);
            SetImageSlider();
            SetTips(2);
        }
        private void SetCompleteCondition()
        {
            condition_complete.SetActive(true);
        }
        private void SetImageReward()
        {
            var reward = find_component<RectTransform>("Panel/Reward");
            reward.SetActive(false);
            if (GameConfigManager.MakeOverStorage.StoryCondition[storyConfig.ID] == 2)
                reward.SetActive(true);

            //把宝箱设置成开关
            register_toggle("Panel/Reward/Icon", on_reward_clicked).isOn = true;
            bubble.SetActive(true);

            //bubble里的奖励显示
            var icon = find_component<Image>("Panel/Reward/Bubble/image/item");
            var text = find_component<Text>("Panel/Reward/Bubble/image/count");
            var itemlist = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().ItemConfigList;
            var item = itemlist.Find(a => a.ID == storyConfig.ImageRewardID);
            icon.sprite = _ui_manager.FindSprite($"{item.Pack}", $"{item.Icon}", true);
            text.text = $"+{storyConfig.ImageRewardNum}";
        }
        private void on_reward_clicked(bool selected)
        {
            play_sound("sound_button_click");
            var bubble = find_component<RectTransform>("Panel/Reward/Bubble");
            bubble.SetActive(true);
            if (selected)
                bubble.SetActive(true);
            else
                bubble.SetActive(false);
        }
        private void SetTouchSlider()
        {
            var touchlist = GameConfigManager.GameConfigGroup.TouchPointConfigList;
            var touchSlider = find_component<Slider>("Panel/Condition/touch/rate/Slider");
            var touchCount = find_component<Text>("Panel/Condition/touch/rate/Slider/Fill Area/Text");
            int touchSliderCurrent = 0;
            int touchSliderMax = 0;

            foreach (var touch in touchlist)
            {
                if (touch.StoryID == storyConfig.ID)
                {
                    touchSliderMax++;
                    if (GameConfigManager.MakeOverStorage.TouchPointCondition[touch.ID] >= 2) 
                        touchSliderCurrent++;
                }
            }
            touchSlider.value = touchSliderCurrent;
            touchSlider.maxValue = touchSliderMax;
            touchCount.text = $"{touchSliderCurrent}/{touchSliderMax}";
        }
        private void SetImageSlider()
        {
            var makeoverlist = GameConfigManager.GameConfigGroup.MakeOverConfigList;
            var imageSlider = find_component<Slider>("Panel/Condition/image/rate/Slider");
            var imageCount = find_component<Text>("Panel/Condition/image/rate/Slider/Fill Area/Text");
            int imageSliderCurrent = 0;
            int imageSliderMax = 0;

            foreach (var image in makeoverlist)
            {
                if (image.StoryID == storyConfig.ID)
                {
                    if (image.ImageCount)
                    {
                        imageSliderMax++;
                        if (GameConfigManager.MakeOverStorage.ImageUnlock[image.ID])
                            imageSliderCurrent++;
                    }
                }
            }
            imageSlider.value = imageSliderCurrent;
            imageSlider.maxValue = imageSliderMax;
            imageCount.text = $"{imageSliderCurrent}/{imageSliderMax}";
        }
        private void on_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        public StoryButton Init(StoryConfig story, Action<StoryButton> click_callback)
        {
            storyConfig = story;
            ClickCalback = click_callback;
            Show();
            SetBase();
            SetCondition();
            SetImageReward();
            return this;
        }
    }
    public class CommingSoon : BaseUI
    {
        public CommingSoon Init()
        {
            Show();
            return this;
        }
    }

    public static new string DefaultPrefabPath = "MakeOver/UI_Story";
    public StoryConfig currentStory;
    private RectTransform storyButton_rt;
    private GameObject storyButton_temp;
    private List<StoryConfig> storylist;
    private int storyScrollID = 1;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");

        register_button("Panel/UI_Top/Button_close", on_close_clicked);
        storyButton_rt = find_component<RectTransform>("Panel/Scroll View/Viewport/Content/UI_Middle");
        storyButton_temp = find_component<RectTransform>("StoryTemplate", storyButton_rt).gameObject;
        storyButton_temp.SetActive(false);

        var comming = find_component<RectTransform>("CommingSoon", storyButton_rt).gameObject;
        comming.SetActive(false);

        StorynInit();
    }
    private StoryUI StorynInit()
    {
        StoryBack();
        RefreshStoryButtonList();
        StoryScroll();
        StoryGuide();
        return this;
    }
    private void StoryBack()
    {
        var back = find_component<RectTransform>("BG");
        var currentstory = storylist.Find(a => a.ID == GameConfigManager.MakeOverStorage.CurrentStoryID);
        var background = create_ui<HomeBackground>($"MakeOverLevels/00_bg/{currentstory.HomeBack}", back);
        background.Init();
    }
    private void RefreshStoryButtonList()
    {
        foreach (var story in storylist)
        {
            var story_button = create_ui<StoryButton>(storyButton_temp, storyButton_rt);
            story_button.Init(story, p => on_panel_selected(p.storyConfig));
        }
        var comming = find_component<RectTransform>("CommingSoon", storyButton_rt).gameObject;
        var commingbutton = create_ui<CommingSoon>(comming, storyButton_rt);
        commingbutton.Init();
    }
    private void StoryScroll()
    {
        //有猫任务的时候，按照价格够判断
        var all_touch = GameConfigManager.GameConfigGroup.TouchPointConfigList;
        var all_image = GameConfigManager.GameConfigGroup.MakeOverConfigList;

        foreach (var story in storylist) 
        {
            if (GameConfigManager.MakeOverStorage.StoryCondition[story.ID] == 1)
            {
                var touch = all_touch.Find(a => GameConfigManager.MakeOverStorage.TouchPointCondition[a.ID] == 1);
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType != 4 && GameConfigManager.CommonStorage.Flower >= a.BuyPrice);
                    if (image != null) 
                    {
                        storyScrollID = touch.StoryID;
                        break;
                    }
                }
            }
            else if (GameConfigManager.MakeOverStorage.StoryCondition[story.ID] == 2)
            {
                var touch = all_touch.Find(a => GameConfigManager.MakeOverStorage.TouchPointCondition[a.ID] == 2);
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType != 4 && GameConfigManager.CommonStorage.Flower >= a.SecondPrice);
                    if (image != null)
                    {
                        storyScrollID = touch.StoryID;
                        break;
                    }
                    else
                    {
                        GetLastStory();
                        break;
                    }
                }
            }
            else if (GameConfigManager.MakeOverStorage.StoryCondition[story.ID] == 3)
                GetLastStory();
            if (GameConfigManager.MakeOverStorage.StoryGuide[1] == 1)
                GetFirstStory();
            //如果全部story的condition都为3，则story的scroll为最大值
            GetMaxStory();
        }
        if (storyScrollID > 1)
        {
            var scroll = find_component<RectTransform>("Panel/Scroll View/Viewport/Content");
            var v = scroll.localPosition;
            v.y = (storyScrollID - 1) * 430 - 300;
            scroll.localPosition = v;
        }
    }
    private void GetFirstStory()
    {
        storyScrollID = 1;
    }
    private void GetLastStory()
    {
        var story = storylist.Find(a => a != null && GameConfigManager.MakeOverStorage.StoryCondition[a.ID] == 1);
        if (story != null)
            storyScrollID = story.ID;
    }
    private void GetMaxStory()
    {
        var story = storylist.Find(a => a != null && GameConfigManager.MakeOverStorage.StoryCondition[a.ID] != 3);
        if (!storylist.Contains(story))
            storyScrollID = storylist.Count + 1;
    }
    private void on_panel_selected(StoryConfig storyConfig)
    {
        play_sound("sound_button_click");
        currentStory = storyConfig;

        var home_ui = _ui_manager.FindWindow<HomeUI>();
        GameConfigManager.MakeOverStorage.CurrentStoryID = currentStory.ID;
        home_ui.MakeOverInit();
        home_ui.DefaultAnimSet();
        home_ui.catQuest.RefreshCatQuest();
        home_ui.storyIcon.StoryTipInit();
        StoryGuideChange();
        Close();
    }
    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
        StoryGuideChange();
    }
    private void StoryGuide()
    {
        var guide = find_component<RectTransform>("Panel/Guide/Panel/guide_1");
        guide.SetActive(false);
        if (GameConfigManager.MakeOverStorage.ImageUnlock[GameConfigManager.GlobalConfig.Unlock_StoryIcon])
        {
            if (GameConfigManager.MakeOverStorage.StoryGuide[1] == 1)
                guide.SetActive(true);
        }
    }
    private void StoryGuideChange()
    {
        if (GameConfigManager.MakeOverStorage.ImageUnlock[GameConfigManager.GlobalConfig.Unlock_StoryIcon])
            GameConfigManager.MakeOverStorage.StoryGuide[1] = 2;
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        home_ui.storyIcon.StoryTipInit();
    }
}