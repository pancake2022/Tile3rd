using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayUI : BaseUI
{
    public HomeUI Home;
    private M3Panel currentPanel;
    private RectTransform tip;
    private RectTransform guide;

    public PlayUI Init(HomeUI home)
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        register_button("Panel", on_play_clicked);

        //初始化引导
        tip = find_component<RectTransform>("Panel/tip");
        guide = find_component<RectTransform>("Panel/guide");
        tip.SetActive(false);
        guide.SetActive(false);

        var playbutton = find_component<Text>("Panel/Text");
        playbutton.text = GameConfigManager.LevelStorage.LevelCount.ToString();

        CreatePanel();
        ShowGuide();        
    }
    public void on_play_clicked()
    {
        GameConfigManager.ShareDataGlobalConfig._level_condition = 1;

        //剧情处理
        //shareDataGlobalConfig._story_game_out = true;
        if (GameConfigManager.LevelStorage.LevelCount == 1)
        {
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[2] >= 2) 
                GoGame();
            else
                StartCoroutine(WaitCheck_tip());
        }
        else if (GameConfigManager.LevelStorage.LevelCount == 4)
        {
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[5] >= 2) 
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
        //_ui_manager.OpenWindow<GameUI>().Init(Home.currentPanel);
        _ui_manager.OpenWindow<GameUI>().Init(currentPanel);
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
        if (GameConfigManager.LevelStorage.LevelCount == 1)
        {
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[2] >= 2) 
                guide.SetActive(true);
        }
        if (GameConfigManager.LevelStorage.LevelCount == 2)
        {
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[3] >= 2)
                guide.SetActive(true);
        }
        if (GameConfigManager.LevelStorage.LevelCount == 3)
            guide.SetActive(true);
    }
    //创建关卡
    private void CreatePanel()
    {
        var gamelevel = GameConfigManager.GetGameLevel(GameConfigManager.LevelStorage.GameLevel_Condition);
        if (gamelevel != null)
        {
            GameConfigManager.LevelStorage.Current_GameLevel = gamelevel;
            var panel_config_ta = _ui_manager.Framework.ResourcesManager.LoadResource<TextAsset>
            ($"{M3Const.M3PanelConfigPath}/{gamelevel.PanelID}");

            if (panel_config_ta != null)
            {
                try
                {
                    currentPanel = JsonUtility.FromJson<M3Panel>(panel_config_ta.text);
                }
                catch (Exception e)
                {
                    CSFramework.Logger.Error(e);
                }
            }
        }

        else
            Debug.Log("没有关卡");
    }
}