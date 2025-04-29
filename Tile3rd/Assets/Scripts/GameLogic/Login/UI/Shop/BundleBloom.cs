using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundleBloom : BaseUI
{
    public HomeUI Home;
    public int diff;

    public BundleBloom Init(HomeUI home)
    {
        Home = home;
        return this;
    }

    protected override void on_create()
    {
        register_button("Panel/item_bloom/button_claim", on_claim_clicked);
        register_button("Panel/item_bloom/button_rv", on_claim_clicked);
        ButtonInit();
        NoADSIconInit();
    }
    private void ButtonInit()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var green = find_component<RectTransform>("Panel/item_bloom/button_claim");
        var blue = find_component<RectTransform>("Panel/item_bloom/button_rv");
        green.SetActive(false);
        blue.SetActive(false);

        if (tile2storage.BloomBuffFirst)
            blue.SetActive(true);
        else
            green.SetActive(true);
    }
    //初始化去广告礼包
    private void NoADSIconInit()
    {
        var noADS_cd = find_component<RectTransform>("Panel/item_bloom/countdown");
        var noADS_claim = find_component<RectTransform>("Panel/item_bloom/button_claim");
        noADS_cd.transform.SetActive(false);
        noADS_claim.transform.SetActive(true);
    }

    private void on_claim_clicked()
    {
        _ui_manager.OpenWindow<BundleBloomUI>();
    }
    private void on_rv_clicked()
    {
        _ui_manager.OpenWindow<BundleBloomUI>();
    }
}