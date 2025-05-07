using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CatQuestRewardUI : WindowUI
{
    public static new string DefaultPrefabPath = "MakeOver/UI_CatQuest_Reward";
    public Image rewardIcon;
    public Text rewardCount;
    public RectTransform rewardtextshow;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_level_win");

        register_button("Panel/Button/claim", on_ok_clicked);
        IconInit();
    }
    private void on_ok_clicked()
    {
        play_sound("sound_button_click");
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        home_ui.catQuest.on_panel_selected(home_ui.catQuest.questButton.questConfig);
        home_ui.catQuest.CatQuestActive(false);
        Close();
    }
    private void IconInit()
    {
        var all_quest = GameConfigManager.GameConfigGroup.QuestConfigList;
        var all_image = GameConfigManager.GameConfigGroup.MakeOverConfigList;
        var quest = all_quest.Find(a => a.ID == GameConfigManager.MakeOverStorage.CurrentQuest.ID);
        var image = all_image.Find(a => a.ID == quest.MakeOverImageID);
        var icon = find_component<Image>("Panel/UI_reward/item_1/Image");
        var text = find_component<Text>("Panel/UI_reward/item_1/Text");
        icon.sprite = _ui_manager.FindSprite($"{image.Pack}", $"{image.Icon}", true);
        text.text = quest.DescFinish;
    }
}