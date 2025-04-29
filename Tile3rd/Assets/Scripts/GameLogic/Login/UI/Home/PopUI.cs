using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PopUI : BaseUI
{
    public HomeUI Home;

    private CommonStorage commonStorage;
    private Tile2Storage tile2storage;
    private LevelStorage levelStorage;
    private ShareDataGlobalConfig shareDataGlobalConfig;
    private GameConfigGroup gameConfigGroup;
    private GlobalConfig globalconfig;

    public PopUI Init(HomeUI home)
    {
        Home = home;
        Creat();
        PopWindow();
        return this;
    }
    protected override void on_create()
    {
        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
        tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        globalconfig = gameConfigGroup.GlobalConfigList[0];
    }
    //创建pop列表 0.签到/1.谷歌评分/2.商店礼包/3.广告礼包/4.bloom广告
    private void Creat()
    {
        for (int i = 0; i < 5; i++)
        {
            if (!shareDataGlobalConfig._pop_ui.ContainsKey(i))
                shareDataGlobalConfig._pop_ui.Add(i, false);
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
        if (levelStorage.LevelCount >= globalconfig.Unlock_Sign)
        {
            if (tile2storage.SignCondition[0] == 1)
                shareDataGlobalConfig._pop_ui[0] = true;
        }
    }
    private void Condition_GoogleReview()
    {
        
        ////解锁判断
        //if (levelStorage.LevelCount == globalconfig.Unlock_GoogleReview)
        //{
        //    if (commonStorage.Android_Reviewed <= 0)
        //    {
        //        shareDataGlobalConfig._pop_ui[1] = true;
        //        commonStorage.Android_Reviewed = 10;
        //    }
        //}

        ////后续判断
        //if (levelStorage.LevelCount > globalconfig.Unlock_GoogleReview)
        //{
        //    var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        //    if (shareDataGlobalConfig._is_interstitial == false)
        //    {
        //        if (commonStorage.Android_Reviewed <= 0)
        //        {
        //            shareDataGlobalConfig._pop_ui[1] = true;
        //            commonStorage.Android_Reviewed = 10;
        //        }
        //    }
        //}
        if (levelStorage.LevelCount >= globalconfig.Unlock_GoogleReview)
        {
            if (shareDataGlobalConfig._is_interstitial == false)
            {
                if (commonStorage.Android_Reviewed <= 0)
                    shareDataGlobalConfig._pop_ui[1] = true;
            }
        }
    }
    private void Condition_Shop()
    {
        if (tile2storage.isShopUnlock)
        {
            if (shareDataGlobalConfig._shop_pop_cd >= 5)
            {
                if (tile2storage.CurrentShop.ID == 1)
                {
                    if (shareDataGlobalConfig._is_interstitial)
                        shareDataGlobalConfig._pop_ui[2] = true;
                }
                if (tile2storage.CurrentShop.ID == 2)
                {
                    if (tile2storage.BloomBuffTimes <= 0)
                        shareDataGlobalConfig._pop_ui[2] = true;
                }
            }
        }
    }
    private void Condition_RVBundle()
    {
        if (levelStorage.LevelCount > globalconfig.Unlock_NewBundle)
        {
            Home.bundleItem.GetCurrentBundle();
            if (Home.currentBundle != null)
            {
                if (tile2storage.BundleRV[Home.currentBundle.ID] == false && shareDataGlobalConfig._storybundle_check)
                    shareDataGlobalConfig._pop_ui[3] = true;
            }
        }
    }
    private void Condition_BloomRV()
    {
        if (levelStorage.LevelCount >= globalconfig.Unlock_BloomBundle)
        {
            if (tile2storage.BloomBuffTimes <= 0)
                shareDataGlobalConfig._pop_ui[4] = true;
        }
    }


    private void PopWindow()
    {
        PopCondition();
        //签到
        if (shareDataGlobalConfig._pop_ui[0] == true)
        {
            shareDataGlobalConfig._pop_ui[0] = false;
            Pop_SignUI();
        }
        //评分
        else if (shareDataGlobalConfig._pop_ui[1] == true)
        {
            shareDataGlobalConfig._pop_ui[1] = false;
            Pop_GoogleReview();
        }
        //商店礼包
        else if (shareDataGlobalConfig._pop_ui[2] == true)
        {
            shareDataGlobalConfig._pop_ui[2] = false;
            Pop_Shop();
        }
        //广告礼包
        else if (shareDataGlobalConfig._pop_ui[3] == true) 
        {
            shareDataGlobalConfig._pop_ui[3] = false;
            Pop_RVBundle();
        }
        //bloom广告
        else if (shareDataGlobalConfig._pop_ui[4] == true) 
        {
            shareDataGlobalConfig._pop_ui[4] = false;
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
        commonStorage.Android_Reviewed = 10;
    }
    private void Pop_Shop()
    {
        if (tile2storage.CurrentShop.ID == 1)
            _ui_manager.OpenWindow<ShopBundleUI_noADS>();
        if (tile2storage.CurrentShop.ID == 2)
            _ui_manager.OpenWindow<ShopBundleUI_item>();
        shareDataGlobalConfig._shop_pop_cd = 0;
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