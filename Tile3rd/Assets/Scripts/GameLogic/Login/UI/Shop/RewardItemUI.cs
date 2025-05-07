using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RewardItemUI : WindowUI
{
    public static new string DefaultPrefabPath = "Reward/UI_Reward_Item";
    private HomeUI home_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        home_ui = _ui_manager.FindWindow<HomeUI>();

        register_button("Panel/Button/button_claim", on_claim_clicked);
    }
    protected override void on_open()
    {
        StartCoroutine(WaitCheck_sound());
        ItemInit();
        ItemShow();
        GetReward();
    }
    private IEnumerator WaitCheck_sound()
    {
        _ui_manager.Framework.AudioManager.PlaySound("sound_level_win");
        _ui_manager.Framework.AudioManager.PlaySound("sound_chest_open");
        yield return new WaitForSeconds(0.3f);
        _ui_manager.Framework.AudioManager.PlaySound("sound_chest_appear");
    }
    private void ItemInit()
    {
        //图片默认关闭
        var bg = find_component<RectTransform>("Panel/UI_reward");
        for (int i = 0; i < bg.childCount; i++)
        {
            var pic = bg.transform.GetChild(i);
            pic.SetActive(false);
        }
    }
    //如果道具不为0，则显示道具
    private void ItemShow()
    {
        var all_item = GameConfigManager.GameConfigGroup.ItemConfigList;
        var finditem1 = all_item.Find(a => a.ID == home_ui.currentBundle.Item1ID);
        var finditem2 = all_item.Find(a => a.ID == home_ui.currentBundle.Item2ID);
        var finditem3 = all_item.Find(a => a.ID == home_ui.currentBundle.Item3ID);

        //道具显示
        var item1 = find_component<RectTransform>("Panel/UI_reward/item_1");
        var item2 = find_component<RectTransform>("Panel/UI_reward/item_2");
        var item3 = find_component<RectTransform>("Panel/UI_reward/item_3");
        var image1 = find_component<Image>("Panel/UI_reward/item_1/Image");
        var image2 = find_component<Image>("Panel/UI_reward/item_2/Image");
        var image3 = find_component<Image>("Panel/UI_reward/item_3/Image");
        var text1 = find_component<Text>("Panel/UI_reward/item_1/Image/Text");
        var text2 = find_component<Text>("Panel/UI_reward/item_2/Image/Text");
        var text3 = find_component<Text>("Panel/UI_reward/item_3/Image/Text");

        if (home_ui.currentBundle.Item1Num > 0)
        {
            item1.SetActive(true);
            image1.sprite = _ui_manager.FindSprite($"{finditem1.Pack}", $"{finditem1.Icon}", true);
            text1.text = "x " + home_ui.currentBundle.Item1Num.ToString();
        }

        if (home_ui.currentBundle.Item2Num > 0)
        {
            item2.SetActive(true);
            image2.sprite = _ui_manager.FindSprite($"{finditem2.Pack}", $"{finditem2.Icon}", true);
            text2.text = "x " + home_ui.currentBundle.Item2Num.ToString();
        }

        if (home_ui.currentBundle.Item3Num > 0)
        {
            item3.SetActive(true);
            image3.sprite = _ui_manager.FindSprite($"{finditem3.Pack}", $"{finditem3.Icon}", true);
            text3.text = "x " + home_ui.currentBundle.Item3Num.ToString();
        }
    }
    private void GetReward()
    {
        GameConfigManager.Tile2Storage.BundleRV[home_ui.currentBundle.ID] = true;
        GameConfigManager.CommonStorage.Item_Recall = GameConfigManager.CommonStorage.Item_Recall + home_ui.currentBundle.Item1Num;
        GameConfigManager.CommonStorage.Item_Remove = GameConfigManager.CommonStorage.Item_Remove + home_ui.currentBundle.Item2Num;
        GameConfigManager.CommonStorage.Item_Bloom = GameConfigManager.CommonStorage.Item_Bloom + home_ui.currentBundle.Item3Num;
    }
    private void on_claim_clicked()
    {
        play_sound("sound_button_click");
        home_ui.bundleItem.IconInit();
        Close();
    }
}
