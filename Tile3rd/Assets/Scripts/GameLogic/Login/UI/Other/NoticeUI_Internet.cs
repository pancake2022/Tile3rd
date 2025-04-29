using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NoticeUI_Internet : WindowUI
{
    public static new string DefaultPrefabPath = "Panel/UI_Panel_internet";
    private GameUI game_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        game_ui = _ui_manager.FindWindow<GameUI>();
        if (game_ui != null)
            game_ui.GamePause();

        register_button("Panel/Button/OK", on_close_clicked);
    }

    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        if (game_ui != null)
            game_ui.GameActiveDelay();
        Close();
    }
}