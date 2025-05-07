using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SettingUI : WindowUI
{
    public static new string DefaultPrefabPath = "Panel/UI_Panel_setting";

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");

        register_button("Panel/Button_close", on_close_clicked);
        register_button("Panel/Button_home", on_close_clicked);
        ToggleInit();
    }
    private void ToggleInit()
    {
        //控制音乐按钮显示
        if (GameConfigManager.CommonStorage.MusicOpen == true)
            register_toggle("Panel/Button_music", on_music_clicked).isOn = true;
        else
            register_toggle("Panel/Button_music", on_music_clicked).isOn = false;
        //控制音效按钮显示
        if (GameConfigManager.CommonStorage.SoundOpen == true)
            register_toggle("Panel/Button_sound", on_sound_clicked).isOn = true;
        else
            register_toggle("Panel/Button_sound", on_sound_clicked).isOn = false;
    }

    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
    }
    private void on_music_clicked(bool selected)
    {
        play_sound("sound_button_click");
        if (selected) 
        {
            GameConfigManager.CommonStorage.MusicOpen = true;
            _ui_manager.Framework.AudioManager.SetMusicOpen(true);
        }
        else
        {
            GameConfigManager.CommonStorage.MusicOpen = false;
            _ui_manager.Framework.AudioManager.SetMusicOpen(false);
        }
    }
    private void on_sound_clicked(bool selected)
    {
        play_sound("sound_button_click");
        if (selected) 
        {
            GameConfigManager.CommonStorage.SoundOpen = true;
            _ui_manager.Framework.AudioManager.SetSoundOpen(true);
        }
        else
        {
            GameConfigManager.CommonStorage.SoundOpen = false;
            _ui_manager.Framework.AudioManager.SetSoundOpen(false);
        }
    }
}