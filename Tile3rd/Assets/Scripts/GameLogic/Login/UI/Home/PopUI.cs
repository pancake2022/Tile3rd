using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PopUI : BaseUI
{
    public HomeUI Home;

    public PopUI Init(HomeUI home)
    {
        Home = home;
        Creat();
        PopWindow();
        return this;
    }
    protected override void on_create()
    {

    }
    //创建pop列表 0.签到/1.谷歌评分/2.商店礼包/3.广告礼包/4.bloom广告
    private void Creat()
    {
        for (int i = 0; i < 5; i++)
        {
            if (!GameConfigManager.ShareDataGlobalConfig._pop_ui.ContainsKey(i))
                GameConfigManager.ShareDataGlobalConfig._pop_ui.Add(i, false);
        }
    }
    private void PopCondition()
    {
        //签到弹出条件
        Condition_SignUI();
        Condition_GoogleReview();
        Condition_Shop();
        Condition_RVBundle();
        Condition_BloomRV();
    }
    private void Condition_SignUI()
    {
        if (GameConfigManager.LevelStorage.LevelCount >= GameConfigManager.GlobalConfig.Unlock_Sign)
        {
            if (GameConfigManager.Tile2Storage.SignCondition[0] == 1)
                GameConfigManager.ShareDataGlobalConfig._pop_ui[0] = true;
        }
    }
    private void Condition_GoogleReview()
    {
        if (GameConfigManager.LevelStorage.LevelCount >= GameConfigManager.GlobalConfig.Unlock_GoogleReview)
        {
            if (GameConfigManager.ShareDataGlobalConfig._is_interstitial == false)
            {
                if (GameConfigManager.CommonStorage.Android_Reviewed <= 0)
                    GameConfigManager.ShareDataGlobalConfig._pop_ui[1] = true;
            }
        }
    }
    private void Condition_Shop()
    {
        if (GameConfigManager.Tile2Storage.isShopUnlock)
        {
            if (GameConfigManager.ShareDataGlobalConfig._shop_pop_cd >= 5)
            {
                if (GameConfigManager.Tile2Storage.CurrentShop.ID == 1)
                {
                    if (GameConfigManager.ShareDataGlobalConfig._is_interstitial)
                        GameConfigManager.ShareDataGlobalConfig._pop_ui[2] = true;
                }
                if (GameConfigManager.Tile2Storage.CurrentShop.ID == 2)
                {
                    if (GameConfigManager.Tile2Storage.BloomBuffTimes <= 0)
                        GameConfigManager.ShareDataGlobalConfig._pop_ui[2] = true;
                }
            }
        }
    }
    private void Condition_RVBundle()
    {
        if (GameConfigManager.LevelStorage.LevelCount > GameConfigManager.GlobalConfig.Unlock_NewBundle)
        {
            Home.bundleItem.GetCurrentBundle();
            if (Home.currentBundle != null)
            {
                if (GameConfigManager.Tile2Storage.BundleRV[Home.currentBundle.ID] == false && GameConfigManager.ShareDataGlobalConfig._storybundle_check)
                    GameConfigManager.ShareDataGlobalConfig._pop_ui[3] = true;
            }
        }
    }
    private void Condition_BloomRV()
    {
        if (GameConfigManager.LevelStorage.LevelCount >= GameConfigManager.GlobalConfig.Unlock_BloomBundle)
        {
            if (GameConfigManager.Tile2Storage.BloomBuffTimes <= 0)
                GameConfigManager.ShareDataGlobalConfig._pop_ui[4] = true;
        }
    }


    private void PopWindow()
    {
        PopCondition();
        //签到
        if (GameConfigManager.ShareDataGlobalConfig._pop_ui[0] == true)
        {
            GameConfigManager.ShareDataGlobalConfig._pop_ui[0] = false;
            Pop_SignUI();
        }
        //评分
        else if (GameConfigManager.ShareDataGlobalConfig._pop_ui[1] == true)
        {
            GameConfigManager.ShareDataGlobalConfig._pop_ui[1] = false;
            Pop_GoogleReview();
        }
        //商店礼包
        else if (GameConfigManager.ShareDataGlobalConfig._pop_ui[2] == true)
        {
            GameConfigManager.ShareDataGlobalConfig._pop_ui[2] = false;
            Pop_Shop();
        }
        //广告礼包
        else if (GameConfigManager.ShareDataGlobalConfig._pop_ui[3] == true) 
        {
            GameConfigManager.ShareDataGlobalConfig._pop_ui[3] = false;
            Pop_RVBundle();
        }
        //bloom广告
        else if (GameConfigManager.ShareDataGlobalConfig._pop_ui[4] == true) 
        {
            GameConfigManager.ShareDataGlobalConfig._pop_ui[4] = false;
            Pop_RVbloom();
        }
    }
    private void Pop_SignUI()
    {
        _ui_manager.OpenWindow<SignUI>();
    }
    private void Pop_GoogleReview()
    {
        //Home.googleReview.ActiveGoogleReview();
        GameConfigManager.CommonStorage.Android_Reviewed = 10;
    }
    private void Pop_Shop()
    {
        if (GameConfigManager.Tile2Storage.CurrentShop.ID == 1)
            _ui_manager.OpenWindow<ShopBundleUI_noADS>();
        if (GameConfigManager.Tile2Storage.CurrentShop.ID == 2)
            _ui_manager.OpenWindow<ShopBundleUI_item>();
        GameConfigManager.ShareDataGlobalConfig._shop_pop_cd = 0;
    }
    private void Pop_RVBundle()
    {
        _ui_manager.OpenWindow<BundleItemsUI>();
    }
    private void Pop_RVbloom()
    {
        _ui_manager.OpenWindow<BundleBloomUI>();
    }
}