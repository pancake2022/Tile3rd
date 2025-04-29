using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGuideUI : BaseUI
{
    public GameUI gameUI;
    private MakeOverStorage makeoverStorage;
    private CommonStorage commonStorage;
    private LevelStorage levelStorage;
    private GameConfigGroup gameConfigGroup;
    private GlobalConfig globalconfig;
    private bool isGuide = false;

    protected override void on_create()
    {
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();//获取通用关卡存档
        gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        globalconfig = gameConfigGroup.GlobalConfigList[0];

        //新手引导相关
        ShowInit();
    }
    public GameGuideUI Init(GameUI game)
    {
        gameUI = game;
        Guide_FirstStep();
        Guide_Match();
        Guide_ItemGroup();
        gameUI.gameItemGroupUI.CallBack_Item(p => Item());
        return this;
    }
    private void ShowInit()
    {
        var allchild = find_component<RectTransform>("Panel");
        foreach (Transform child in allchild)
            child.SetActive(false);
    }
    public void Guide_FirstStep()
    {
        var taphand1 = find_component<RectTransform>("Panel/guide1");
        taphand1.SetActive(false);
        if (gameUI._panel_ui.Panel.ID == 2024001)
        {
            taphand1.SetActive(true);
            if (gameUI._panel_ui.CollectionUI.CollectedCellUIList.Count >= 1)
                taphand1.SetActive(false);
        }
    }
    //新手引导
    private void Guide_Match()
    {
        var guide_level1 = find_component<RectTransform>("Panel/level1");
        var guide_level1_Text = find_component<Text>("Panel/level1/Text");
        var questguide1 = find_component<RectTransform>("Panel/questlevel_1");
        var questguide2 = find_component<RectTransform>("Panel/questlevel_2");
        var questguide3 = find_component<RectTransform>("Panel/questlevel_3");
        var questguide4 = find_component<RectTransform>("Panel/questlevel_4");
        //var questguide5 = find_component<RectTransform>("Panel/questlevel_5");
        var randomtile = find_component<RectTransform>("Panel/randomtile");
        //level1的引导
        if (gameUI._panel_ui.Panel.ID == 2024001)
        {
            guide_level1.SetActive(true);
            if (gameUI.leftCell > 18)
                guide_level1_Text.text = "Tap tile to collect";
            if (gameUI.leftCell <= 18 && gameUI.leftCell > 15)
                guide_level1_Text.text = "Match 3 tiles to remove";
            if (gameUI.leftCell <= 15 && gameUI.leftCell > 12)
                guide_level1_Text.text = "Remove all tiles to win";
            if (gameUI.leftCell <= 12 && gameUI.leftCell > 9)
                guide_level1_Text.text = "Good work!";
            if (gameUI.leftCell <= 9 && gameUI.leftCell > 6)
                guide_level1_Text.text = "Keep matching!";
            if (gameUI.leftCell <= 6 && gameUI.leftCell > 3)
                guide_level1_Text.text = "Almost win!";
            if (gameUI.leftCell <= 3)
                guide_level1_Text.text = "Almost!!";
            if (gameUI.leftCell == 0)
                guide_level1_Text.text = "Wow, You did it!";
        }
        //level2的引导
        if (gameUI._panel_ui.Panel.ID == 2024002)
        {
            guide_level1.SetActive(true);
            if (gameUI.leftCell > 30)
                guide_level1_Text.text = "Match 3 tiles to remove";
            if (gameUI.leftCell <= 30 && gameUI.leftCell > 24)
                guide_level1_Text.text = "Remove all tiles to win";
            if (gameUI.leftCell <= 24 && gameUI.leftCell > 18)
                guide_level1_Text.text = "You did very well";
            if (gameUI.leftCell <= 18 && gameUI.leftCell > 6)
                guide_level1_Text.text = "Keep matching!";
            if (gameUI.leftCell <= 9)
                guide_level1_Text.text = "Almost win!!";
            if (gameUI.leftCell == 0)
                guide_level1_Text.text = "You Win!";
        }
        //任务1的关卡引导
        if (gameUI._panel_ui.Panel.ID == 2024901)
        {
            if (gameUI.leftCell <= 39 && gameUI.leftCell > 36)
                questguide1.SetActive(true);
            if (gameUI.leftCell <= 36 && gameUI.leftCell > 30)
                questguide2.SetActive(true);
            if (gameUI.leftCell <= 30 && gameUI.leftCell > 24)
                questguide3.SetActive(true);
            if (gameUI.leftCell <= 24 && gameUI.leftCell > 15)
                questguide4.SetActive(true);
        }
        //catrandomtile引导
        if (gameUI._panel_ui.Panel.ID == 2024013)
        {
            if (gameUI.leftCell > gameUI.totalCell - 3) 
                randomtile.SetActive(true);
        }
    }
    private void Guide_ItemGroup()
    {
        var guide_level2 = find_component<RectTransform>("Panel/level2");
        if (gameUI._panel_ui.Panel.ID == 2024002)
        {
            if (commonStorage.Item_Recall == 3)
                guide_level2.transform.SetActive(true);
            else
                guide_level2.transform.SetActive(false);
        }
        var guide_level3 = find_component<RectTransform>("Panel/level3");
        if (gameUI._panel_ui.Panel.ID == 2024003)
        {
            if (commonStorage.Item_Remove == 3)
                guide_level3.transform.SetActive(true);
            else
                guide_level3.transform.SetActive(false);
        }
        var guide_level4 = find_component<RectTransform>("Panel/level4");
        if (gameUI._panel_ui.Panel.ID == 2024004)
        {
            if (gameUI.gameRewardItem.BloomTimes <= 0)
                guide_level4.transform.SetActive(true);
            else
                guide_level4.transform.SetActive(false);
        }
        var taphand2 = find_component<RectTransform>("Panel/guide2");
        var questguide5 = find_component<RectTransform>("Panel/questlevel_5");
        if (gameUI._panel_ui.Panel.ID == 2024005)
        {
            if (gameUI.gameRewardItem.BloomTimes <= 5 && isGuide == false) 
            {
                questguide5.SetActive(true);
                taphand2.SetActive(true);
                isGuide = true;
            }
        }
    }
    //public void level12give1bloom()//在道具group里给会快一点
    public void Match()
    {
        ShowInit();
        Guide_Match();
        Guide_ItemGroup();
        gameUI.gameItemGroupUI.BloomTipsRefresh();
    }
    private void Item()
    {
        ShowInit();
        Guide_ItemGroup();
        Guide_Match();
    }
}