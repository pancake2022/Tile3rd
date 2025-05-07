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
        if (GameConfigManager.Tile2Storage.isShopUnlock == false)
        {
            GameConfigManager.Tile2Storage.ShopRefreshCD = DateTime.Now;
            GameConfigManager.Tile2Storage.isShopUnlock = true;
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
        bundleIcon = create_ui<BundleIcon>($"Home/{GameConfigManager.Tile2Storage.CurrentShop.IconPath}", "Panel").Init(GameConfigManager.Tile2Storage.CurrentShop);
    }
    public void GetShopCD()
    {
        DateTime startTime = GameConfigManager.Tile2Storage.ShopRefreshCD;
        DateTime endTime = DateTime.Now;
        float cdTime = GameConfigManager.GlobalConfig.Shop_CD;

        TimeSpan difftime = startTime.Subtract(endTime);
        diff = cdTime + MathF.Floor((float)(difftime.TotalSeconds));

        if (diff <= 0)
        {
            GameConfigManager.Tile2Storage.ShopRefreshCD = DateTime.Now;
            ChangeCurrentShop();
        }
    }
    public void ChangeCurrentShop()
    {
        var all_shop = GameConfigManager.GameConfigGroup.ShopConfigList;
        if (GameConfigManager.Tile2Storage.isnoADS)
            GameConfigManager.Tile2Storage.CurrentShop = all_shop.Find(a => a.ID == 2);
        else
        {
            if (GameConfigManager.Tile2Storage.CurrentShop.ID == 1)
                GameConfigManager.Tile2Storage.CurrentShop = all_shop.Find(a => a.ID == 2);
            else
                GameConfigManager.Tile2Storage.CurrentShop = all_shop.Find(a => a.ID == 1);
        }
        IconInit();
    }
    private void PopUI()
    {
        if (GameConfigManager.ShareDataGlobalConfig._shop_pop_cd >= 5)
        {
            if (GameConfigManager.Tile2Storage.CurrentShop.ID == 1)
            {
                if (GameConfigManager.ShareDataGlobalConfig._is_interstitial)
                {
                    GameConfigManager.ShareDataGlobalConfig._shop_pop_cd = 0;
                    _ui_manager.OpenWindow<ShopBundleUI_noADS>();
                }
            }
            if (GameConfigManager.Tile2Storage.CurrentShop.ID == 2)
            {
                if (GameConfigManager.Tile2Storage.BloomBuffTimes <= 0)
                {
                    GameConfigManager.ShareDataGlobalConfig._shop_pop_cd = 0;
                    _ui_manager.OpenWindow<ShopBundleUI_item>();
                }
            }
        }
    }
}