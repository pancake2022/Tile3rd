using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ShopBundleIcon : WindowUI
{
    public class BundleIcon : BaseUI
    {
        private ShopConfig shopConfig;
        protected override void on_create()
        {
            register_button("Panel/item_bundle", on_clicked);
        }
        private void Update()
        {
            ShowCD();
        }
        private void on_clicked()
        {
            if (shopConfig.ID == 1)
                _ui_manager.OpenWindow<ShopBundleUI_noADS>();
            if (shopConfig.ID == 2)
                _ui_manager.OpenWindow<ShopBundleUI_item>();
        }
        private void ShowCD()
        {
            var home_ui = _ui_manager.FindWindow<HomeUI>();
            home_ui.shopBundleIcon.GetShopCD();
            var hour = MathF.Floor(home_ui.shopBundleIcon.diff / 3600);
            var min = MathF.Floor((home_ui.shopBundleIcon.diff - hour * 3600) / 60);
            var sec = MathF.Floor((home_ui.shopBundleIcon.diff - hour * 3600 - min * 60));

            var desc = find_component<Text>("Panel/item_bundle/countdown/Text");
            if (hour >= 1)
                desc.text = $"{hour}h{min}m";
            if (hour < 1 && min >= 1)
                desc.text = $"{min}m{sec}s";
            if (min < 1)
                desc.text = $"{sec}s";
        }
        public BundleIcon Init(ShopConfig shopconfig)
        {
            shopConfig = shopconfig;
            return this;
        }
    }

    private HomeUI Home;
    private BundleIcon bundleIcon;
    private Tile2Storage tile2storage;
    public float diff;

    protected override void on_create()
    {
        tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
    }
    public ShopBundleIcon Init(HomeUI home)
    {
        Home = home;
        FirstUnlock();
        IconInit();
        //PopUI();
        return this;
    }
    private void FirstUnlock()
    {
        if (tile2storage.isShopUnlock == false)
        {
            tile2storage.ShopRefreshCD = DateTime.Now;
            tile2storage.isShopUnlock = true;
        }
    }
    public void IconInit()
    {
        gameObject.SetActive(true);
        ClearBundleIcon();
        RfreshBundleIcon();
    }
    private void ClearBundleIcon()
    {
        if (bundleIcon != null)
            destroy_ui(bundleIcon);
    }
    private void RfreshBundleIcon()
    {
        bundleIcon = create_ui<BundleIcon>($"Home/{tile2storage.CurrentShop.IconPath}", "Panel").Init(tile2storage.CurrentShop);
    }
    public void GetShopCD()
    {
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];

        DateTime startTime = tile2storage.ShopRefreshCD;
        DateTime endTime = DateTime.Now;
        float cdTime = globalconfig.Shop_CD;

        TimeSpan difftime = startTime.Subtract(endTime);
        diff = cdTime + MathF.Floor((float)(difftime.TotalSeconds));

        if (diff <= 0)
        {
            tile2storage.ShopRefreshCD = DateTime.Now;
            ChangeCurrentShop();
        }
    }
    public void ChangeCurrentShop()
    {
        var all_shop = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().ShopConfigList;
        if (tile2storage.isnoADS)
            tile2storage.CurrentShop = all_shop.Find(a => a.ID == 2);
        else
        {
            if (tile2storage.CurrentShop.ID == 1)
                tile2storage.CurrentShop = all_shop.Find(a => a.ID == 2);
            else
                tile2storage.CurrentShop = all_shop.Find(a => a.ID == 1);
        }
        IconInit();
    }
    private void PopUI()
    {
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        if (shareDataGlobalConfig._shop_pop_cd >= 5)
        {
            if (tile2storage.CurrentShop.ID == 1)
            {
                if (shareDataGlobalConfig._is_interstitial)
                {
                    shareDataGlobalConfig._shop_pop_cd = 0;
                    _ui_manager.OpenWindow<ShopBundleUI_noADS>();
                }
            }
            if (tile2storage.CurrentShop.ID == 2)
            {
                if (tile2storage.BloomBuffTimes <= 0)
                {
                    shareDataGlobalConfig._shop_pop_cd = 0;
                    _ui_manager.OpenWindow<ShopBundleUI_item>();
                }
            }
        }
    }
}