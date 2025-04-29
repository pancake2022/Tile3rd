using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class BundleItems : WindowUI
{
    public HomeUI Home;

    protected override void on_create()
    {
        register_button("Panel/item_bundle", on_clicked);
    }
    public BundleItems Init(HomeUI home)
    {
        Home = home;
        IconInit();
        return this;
    }
    public void IconInit()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        GetCurrentBundle();
        
        if (Home.currentBundle != null)
        {
            
            if (tile2storage.BundleRV[Home.currentBundle.ID] == false)
                IconShow();
            else
                gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(false);
    }
    public void GetCurrentBundle()
    {
        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        var all_bundle = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().BundleConfigList;
        var all_story = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().StoryConfigList;

        var story = all_story.Find(a => a.Type == 1 && makeoverStorage.StoryCondition[a.ID] == 1);
        if (story != null)
            Home.currentBundle = all_bundle.Find(a => a.Type == 1 && a.ShowStoryID == story.ID);
    }
    private void IconShow()
    {
        var icon = find_component<Image>("Panel/item_bundle/coin");
        var text = find_component<Text>("Panel/item_bundle/title");
        icon.sprite = _ui_manager.FindSprite($"{Home.currentBundle.Pack}", $"{Home.currentBundle.Icon}", true);
        text.text = Home.currentBundle.Name;
    }

    private void on_clicked()
    {
        _ui_manager.OpenWindow<BundleItemsUI>();
    }
    public void GetCurrentBundle_Sign()
    {
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        var all_bundle = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().BundleConfigList;
        var all_story = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().StoryConfigList;

        Home.currentBundle = all_bundle.Find(a => a.Type == 2 && a.ID == shareDataGlobalConfig._sign_reward_id);
    }
}