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
    private CommonStorage commonStorage;
    private MakeOverStorage makeoverStorage;
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
        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        all_story = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().StoryConfigList;
        all_touch = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().TouchPointConfigList;
        all_image = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().MakeOverConfigList;

        register_button("button", on_story_clicked);
        tip = find_component<RectTransform>("tips");
        guide = find_component<RectTransform>("guide");
        tip.SetActive(false);
        guide.SetActive(false);
    }
    public void StoryTipInit()
    {
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        if (makeoverStorage.ImageUnlock[globalconfig.Unlock_StoryIcon])
        {
            StoryTipRefresh();
            StoryIconGuide();
        }
    }
    private void StoryTipRefresh()
    {
        if (makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] == 1)
        {
            var touch = all_touch.Find(a => makeoverStorage.TouchPointCondition[a.ID] == 1);
            if (all_touch.Contains(touch))
            {
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType == 1 && commonStorage.Flower >= a.BuyPrice);
                    if (all_image.Contains(image))
                        tip.SetActive(false);
                    else
                        OtherStory();
                }
            }
            else
                OtherStory();
        }
        if (makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] == 2)
        {
            var touch = all_touch.Find(a => makeoverStorage.TouchPointCondition[a.ID] == 1 && a.StoryID != makeoverStorage.CurrentStoryID);
            if (touch != null)
            {
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType == 1 && commonStorage.Flower >= a.BuyPrice);
                    if (all_image.Contains(image))
                        tip.SetActive(true);
                    else
                        OtherStory();
                }
            }
            else
                OtherStory();
        }
        if (makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] == 3)
        {
            
            var touch = all_touch.Find(a => makeoverStorage.TouchPointCondition[a.ID] == 1 && a.StoryID != makeoverStorage.CurrentStoryID);
            if (touch != null)
            {
                
                foreach (var imageID in touch.ImageIDList)
                {
                    var image = all_image.Find(a => a.ID == imageID && a.BuyType == 1 && commonStorage.Flower >= a.BuyPrice);
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
        a.StoryID != makeoverStorage.CurrentStoryID
        && makeoverStorage.StoryCondition[a.StoryID] == 2
        && a.BuyType == 1
        && makeoverStorage.ImageUnlock[a.ID] == false
        && commonStorage.Flower >= a.SecondPrice);

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
        if (makeoverStorage.StoryGuide.ContainsKey(1))
            StoryTipInit();
        else
            makeoverStorage.StoryGuide.Add(1, 1);
    }
    private void StoryIconGuide()
    {
        guide.SetActive(false);
        if (makeoverStorage.StoryGuide[1] == 1)
            guide.SetActive(true);
    }
}