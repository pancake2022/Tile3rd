using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RetryUI : WindowUI
{
    public static new string DefaultPrefabPath = "Game/UI_Panel_retry";
    private ShareDataGlobalConfig shareDataGlobalConfig;
    private GameUI game_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        game_ui = _ui_manager.FindWindow<GameUI>();
        game_ui.GamePause();

        register_button("Panel/Button_home", on_home_clicked);
        register_button("Panel/Button_retry", on_retry_clicked);
    }

    private void on_home_clicked()
    {
        play_sound("sound_panel_closing");
        _ui_manager.TryCloseWindow<GameUI>();
        _ui_manager.OpenWindow<HomeUI>();
        Close();
    }

    //retry按钮
    private void on_retry_clicked()
    {
        //音乐音效
        play_sound("sound_button_click");
        game_ui.SetGameMusic();

        //重制牌局
        game_ui._panel_ui.RetryReset();
        game_ui.Init(game_ui._panel_ui.Panel);

        game_ui.gameRewardItem.BloomBuff = false;
        game_ui.gameRewardItem.BloomTimes = 0;
        game_ui.gameRewardItem.game_reward_item_1 = 0;

        if (game_ui.level.Type == 1)
        {
            game_ui.tileRandom.Condition = 1;
            game_ui.tileRandom.Condition_Count = 0;
            game_ui.tileRandom.ChangeConditionShow();
        }
        Close();
        game_ui.GameActiveDelay();
    }
}