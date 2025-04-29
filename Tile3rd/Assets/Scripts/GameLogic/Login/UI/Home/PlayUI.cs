using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayUI : BaseUI
{
    public HomeUI Home;
    private RectTransform tip;
    private RectTransform guide;

    private LevelStorage levelStorage;
    private MakeOverStorage makeoverStorage;
    private ShareDataGlobalConfig shareDataGlobalConfig;

    public PlayUI Init(HomeUI home)
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        register_button("Panel", on_play_clicked);

        //初始化引导
        tip = find_component<RectTransform>("Panel/tip");
        guide = find_component<RectTransform>("Panel/guide");
        tip.SetActive(false);
        guide.SetActive(false);

        var playbutton = find_component<Text>("Panel/Text");
        playbutton.text = levelStorage.LevelCount.ToString();
        
        ShowGuide();        
    }
    public void on_play_clicked()
    {
        shareDataGlobalConfig._level_condition = 1;

        //剧情处理
        //shareDataGlobalConfig._story_game_out = true;
        if (levelStorage.LevelCount == 1)
        {
            if (makeoverStorage.TouchPointCondition[2] >= 2) 
                GoGame();
            else
                StartCoroutine(WaitCheck_tip());
        }
        else if (levelStorage.LevelCount == 4)
        {
            if (makeoverStorage.TouchPointCondition[5] >= 2) 
                GoGame();
            else
                StartCoroutine(WaitCheck_tip());
        }
        else
            GoGame();
    }
    private void GoGame()
    {
        play_sound("sound_button_click");
        _ui_manager.OpenWindow<GameUI>().Init(Home.currentPanel);
        _ui_manager.TryCloseWindow<HomeUI>();
    }
    private IEnumerator WaitCheck_tip()
    {
        tip.SetActive(true);
        yield return new WaitForSeconds(1f);
        tip.SetActive(false);
    }
    public void ShowGuide()
    {
        if (levelStorage.LevelCount == 1)
        {
            if (makeoverStorage.TouchPointCondition[2] >= 2) 
                guide.SetActive(true);
        }
        if (levelStorage.LevelCount == 2)
        {
            if (makeoverStorage.TouchPointCondition[3] >= 2)
                guide.SetActive(true);
        }
        if (levelStorage.LevelCount == 3)
            guide.SetActive(true);
    }
}