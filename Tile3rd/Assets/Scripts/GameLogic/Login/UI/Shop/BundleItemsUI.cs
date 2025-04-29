using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class BundleItemsUI : WindowUI
{
    public static new string DefaultPrefabPath = "Shop/UI_Bundle_items";
    private HomeUI home_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        home_ui = _ui_manager.FindWindow<HomeUI>();
        shareDataGlobalConfig._storybundle_check = false;
        register_button("Panel/Button_close", on_close_clicked);
        register_button("Panel/Button_rv", on_RV_clicked);
        register_button("Panel/Button_ok", on_OK_clicked);

        TitleInit();
        BackInit();
        ButtonInit();
        RewardInit();
    }
    private void TitleInit()
    {
        var Title = find_component<Text>("Panel/Title/Text");
        Title.text = home_ui.currentBundle.Name;
    }
    private void BackInit()
    {
        //图片默认关闭
        var bg1 = find_component<RectTransform>("Panel/BG/1");
        var bg2 = find_component<RectTransform>("Panel/BG/2");
        bg1.SetActive(false);
        bg2.SetActive(false);
        if (home_ui.currentBundle.Type == 1)
            bg2.SetActive(true);
        if (home_ui.currentBundle.Type == 2)
            bg1.SetActive(true);
    }
    private void ButtonInit()
    {
        var button_rv = find_component<RectTransform>("Panel/Button_rv");
        var button_claim = find_component<RectTransform>("Panel/Button_ok");
        button_rv.SetActive(false);
        button_claim.SetActive(false);
        if (home_ui.currentBundle.Type == 1)
            button_rv.SetActive(true);
        if (home_ui.currentBundle.Type == 2)
            button_claim.SetActive(true);
    }
    private void RewardInit()
    {
        var all_item = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().ItemConfigList;
        var finditem1 = all_item.Find(a => a.ID == home_ui.currentBundle.Item1ID);
        var finditem2 = all_item.Find(a => a.ID == home_ui.currentBundle.Item2ID);
        var finditem3 = all_item.Find(a => a.ID == home_ui.currentBundle.Item3ID);

        //道具默认不显示
        var item = find_component<RectTransform>("Panel/Reward");
        for (int i = 0; i < item.childCount; i++)
        {
            var pic = item.transform.GetChild(i);
            pic.SetActive(false);
        }

        //道具显示
        var item1 = find_component<RectTransform>("Panel/Reward/item_1");
        var item2 = find_component<RectTransform>("Panel/Reward/item_2");
        var item3 = find_component<RectTransform>("Panel/Reward/item_3");
        if (home_ui.currentBundle.Item1Num > 0)
        {
            item1.SetActive(true);
            var image1 = find_component<Image>("Panel/Reward/item_1");
            var text1 = find_component<Text>("Panel/Reward/item_1/Text");
            image1.sprite = _ui_manager.FindSprite($"{finditem1.Pack}", $"{finditem1.Icon}", true);
            text1.text = "x " + home_ui.currentBundle.Item1Num.ToString();
        }
            
        if (home_ui.currentBundle.Item2Num > 0)
        {
            item2.SetActive(true);
            var image2 = find_component<Image>("Panel/Reward/item_2");
            var text2 = find_component<Text>("Panel/Reward/item_2/Text");
            image2.sprite = _ui_manager.FindSprite($"{finditem2.Pack}", $"{finditem2.Icon}", true);
            text2.text = "x " + home_ui.currentBundle.Item2Num.ToString();
        }
            
        if (home_ui.currentBundle.Item3Num > 0)
        {
            item3.SetActive(true);
            var image3 = find_component<Image>("Panel/Reward/item_3");
            var text3 = find_component<Text>("Panel/Reward/item_3/Text");
            image3.sprite = _ui_manager.FindSprite($"{finditem3.Pack}", $"{finditem3.Icon}", true);
            text3.text = "x " + home_ui.currentBundle.Item3Num.ToString();
        }
    }

    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        home_ui.bundleItem.IconInit();
        Close();
    }
    private void on_RV_clicked()
    {
        play_sound("sound_button_click");
        ADSManager.TriggerADSShow_Reward("Bundle_Item");
    }
    private void on_OK_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
    }
}