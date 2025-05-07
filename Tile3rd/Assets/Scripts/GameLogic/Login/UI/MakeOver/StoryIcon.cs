using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StoryIcon : BaseUI
{
    public HomeUI Home;
    private RectTransform tip;
    private RectTransform guide;
    private List<StoryConfig> all_story;
    private List<TouchPointConfig> all_touch;
    private List<MakeOverConfig> all_image;

    public StoryIcon Init(HomeUI home)
    {
        Home = home;
        StartStoryGuide();
        StoryTipInit();
        return this;
    }
    protected override void on_create()
    {
        all_story = GameConfigManager.GameConfigGroup.StoryConfigList;
        all_touch = GameConfigManager.GameConfigGroup.TouchPointConfigList;
        all_image = GameConfigManager.GameConfigGroup.MakeOverConfigList;

        register_button("button", on_story_clicked);
        tip = find_component<RectTransform>("tips");
        guide = find_component<RectTransform>("guide");
        tip.SetActive(false);
        guide.SetActive(false);
    }
    public void StoryTipInit()
    {
        if (GameConfigManager.MakeOverStorage.ImageUnlock[GameConfigManager.GlobalConfig.Unlock_StoryIcon])
        {
            StoryTipRefresh();
            StoryIconGuide();
        }
    }
    private void StoryTipRefresh()
    {
        if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 1)
        {
            var touch = all_touch.Find(a => GameConfigManager.MakeOverStorage.TouchPointCondition[a.ID] == 1);
            if (all_touch.Contains(touch))
            {
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType == 1 && GameConfigManager.CommonStorage.Flower >= a.BuyPrice);
                    if (all_image.Contains(image))
                        tip.SetActive(false);
                    else
                        OtherStory();
                }
            }
            else
                OtherStory();
        }
        if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 2)
        {
            var touch = all_touch.Find(a => GameConfigManager.MakeOverStorage.TouchPointCondition[a.ID] == 1 && a.StoryID != GameConfigManager.MakeOverStorage.CurrentStoryID);
            if (touch != null)
            {
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType == 1 && GameConfigManager.CommonStorage.Flower >= a.BuyPrice);
                    if (all_image.Contains(image))
                        tip.SetActive(true);
                    else
                        OtherStory();
                }
            }
            else
                OtherStory();
        }
        if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 3)
        {
            
            var touch = all_touch.Find(a => GameConfigManager.MakeOverStorage.TouchPointCondition[a.ID] == 1 && a.StoryID != GameConfigManager.MakeOverStorage.CurrentStoryID);
            if (touch != null)
            {
                
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType == 1 && GameConfigManager.CommonStorage.Flower >= a.BuyPrice);
                    if (all_image.Contains(image))
                        tip.SetActive(true);
                    else
                        OtherStory();
                }
            }
            else
                OtherStory();
        }
    }
    private void OtherStory()
    {
        var otherimage = all_image.Find(a =>
        a.StoryID != GameConfigManager.MakeOverStorage.CurrentStoryID
        && GameConfigManager.MakeOverStorage.StoryCondition[a.StoryID] == 2
        && a.BuyType == 1
        && GameConfigManager.MakeOverStorage.ImageUnlock[a.ID] == false
        && GameConfigManager.CommonStorage.Flower >= a.SecondPrice);

        if (otherimage != null)
            tip.SetActive(true);
        else
            tip.SetActive(false);
    }
    private void on_story_clicked()
    {
        _ui_manager.OpenWindow<StoryUI>();
    }
    private void StartStoryGuide()
    {
        if (GameConfigManager.MakeOverStorage.StoryGuide.ContainsKey(1))
            StoryTipInit();
        else
            GameConfigManager.MakeOverStorage.StoryGuide.Add(1, 1);
    }
    private void StoryIconGuide()
    {
        guide.SetActive(false);
        if (GameConfigManager.MakeOverStorage.StoryGuide[1] == 1)
            guide.SetActive(true);
    }
}