using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NoticeUI : WindowUI
{
    public static new string DefaultPrefabPath = "Panel/UI_Panel_notice";
    private Tile2Storage tile2storage;
    private ShareDataGlobalConfig shareDataGlobalConfig;
    private HomeUI home_ui;

    protected override void on_create()
    {
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        Property.CommonAnimationTransform = transform.Find("Panel");
        tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        home_ui = _ui_manager.FindWindow<HomeUI>();
        register_button("Panel/Button/close", on_close_clicked);
        register_button("Panel/Button/button", on_button_clicked);
        PictureInit();
        ButtonInit();
    }
    private void PictureInit()
    {
        var levelchest = find_component<RectTransform>("Panel/Picture/levelchest");
        var outgem = find_component<RectTransform>("Panel/Picture/outgem");
        var catquest = find_component<RectTransform>("Panel/Picture/catquest");
        var tileunlock = find_component<RectTransform>("Panel/Picture/tileunlock");
        var storyunlock = find_component<RectTransform>("Panel/Picture/storyunlock");
        levelchest.SetActive(false);
        outgem.SetActive(false);
        catquest.SetActive(false);
        tileunlock.SetActive(false);
        storyunlock.SetActive(false);

        if (shareDataGlobalConfig._notice_id == 1)
            levelchest.SetActive(true);
        if (shareDataGlobalConfig._notice_id == 2)
            outgem.SetActive(true);
        if (shareDataGlobalConfig._notice_id == 3)
        {
            catquest.SetActive(true);
            GetCatQuestImage();
        }
        if (shareDataGlobalConfig._notice_id == 4)
        {
            tileunlock.SetActive(true);
            GetTileIcon();
        }
        if (shareDataGlobalConfig._notice_id == 5)
        {
            tileunlock.SetActive(true);
            GetSignTileIcon();
        }
        if (shareDataGlobalConfig._notice_id == 6)
        {
            storyunlock.SetActive(true);
            GetStoryPic();
        }
    }
    private void ButtonInit()
    {
        var close = find_component<RectTransform>("Panel/Button/close");
        var text = find_component<Text>("Panel/Button/button/Text");
        close.SetActive(true);

        if (shareDataGlobalConfig._notice_id == 1)
            text.text = "Play";
        if (shareDataGlobalConfig._notice_id == 2)
            text.text = "Play";
        if (shareDataGlobalConfig._notice_id == 3)
            text.text = "Play";
        if (shareDataGlobalConfig._notice_id == 4)
            text.text = "Use Now";
        if (shareDataGlobalConfig._notice_id == 5)
        {
            text.text = "Use Now";
            close.SetActive(false);
        }
        if (shareDataGlobalConfig._notice_id == 6)
        {
            text.text = "View";
            close.SetActive(false);
        }
    }
    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();

        //猫的任务显示
        //if (shareDataGlobalConfig._notice_id == 3)
        //    home_ui.catQuest.questButton.SetShow(true);
    }
    private void on_button_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
        if (shareDataGlobalConfig._notice_id == 1)
            home_ui.playUI.on_play_clicked();
        if (shareDataGlobalConfig._notice_id == 2)
            home_ui.playUI.on_play_clicked();
        if (shareDataGlobalConfig._notice_id == 3)
            home_ui.playUI.on_play_clicked();
        if (shareDataGlobalConfig._notice_id == 4)
            home_ui.levelChest.collectionunlock();
        if (shareDataGlobalConfig._notice_id == 5)
            tile2storage.CurrentTileID = 101;
        if (shareDataGlobalConfig._notice_id == 6)
            home_ui.makeOver.MakeoverRefresh();
    }
    //猫的图片
    private void GetCatQuestImage()
    {
        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        var all_image = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().MakeOverConfigList;
        var image = all_image.Find(a => a.ID == makeoverStorage.CurrentQuest.MakeOverImageID);
        var icon = find_component<Image>("Panel/Picture/catquest/picture/Image");
        var text = find_component<Text>("Panel/Picture/catquest/text/Text");
        icon.sprite = _ui_manager.FindSprite($"{image.Pack}", $"{image.Icon}", true);
        text.text = makeoverStorage.CurrentQuest.DescStart;
    }
    //宝箱牌
    private void GetTileIcon()
    {
        var icon = find_component<Image>("Panel/Picture/tileunlock/picture/Image");
        icon.sprite = _ui_manager.FindSprite($"{home_ui.collection.currentTile.IconPack}", $"{home_ui.collection.currentTile.Icon}", true);
    }
    //每日签到牌
    private void GetSignTileIcon()
    {
        var collectionlist = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().CollectionConfigList;
        var tile = collectionlist.Find(a => a.ID == shareDataGlobalConfig._sign_reward_id);
        var icon = find_component<Image>("Panel/Picture/tileunlock/picture/Image");
        icon.sprite = _ui_manager.FindSprite($"{tile.IconPack}", $"{tile.Icon}", true);
    }
    //解锁story的图
    private void GetStoryPic()
    {
        var storylist = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().StoryConfigList;
        var story = storylist.Find(a => a.ID == shareDataGlobalConfig._sign_reward_id);
        var icon = find_component<Image>("Panel/Picture/tileunlock/picture/Image");
        icon.sprite = _ui_manager.FindSprite($"{story.Pack}", $"{story.Back}", true);
    }
}