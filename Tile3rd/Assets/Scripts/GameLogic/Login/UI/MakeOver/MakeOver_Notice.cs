using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MakeOver_Notice : WindowUI
{
    public static new string DefaultPrefabPath = "MakeOver/UI_MakeOver_Notice";

    protected override void on_create()
    {
        play_sound("sound_panel_opening");
        Property.CommonAnimationTransform = transform.Find("Panel");
        register_button("Panel/Button/close", on_close_clicked);
        register_button("Panel/Button/ok", on_ok_clicked);
        PictureInit();
        PictureShow();
    }
    private void PictureInit()
    {
        var picture = find_component<RectTransform>("Panel/Picture");
        for (int i = 0; i < picture.childCount; i++)
        {
            var pic = picture.transform.GetChild(i);
            pic.SetActive(false);
        }
    }
    private void PictureShow()
    {
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        var picture = find_component<RectTransform>("Panel/Picture");
        Transform[] allpic = GetComponentsInChildren<Transform>(picture);
        foreach (var item in allpic)
        {
            //控制点image打开select还是其他
            if (home_ui.makeOver.makeOver_Image.fromImageClick)
            {
                if (item.name == home_ui.makeOver.makeOver_Image.CurrentImage.ID.ToString())
                    item.SetActive(true);
            }
            else
            {
                if (home_ui.makeOver.makeOver_Select.CurrentPanel.ID.ToString() == item.name)
                    item.SetActive(true);
            }
        }
    }
    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        home_ui.makeOver.makeOver_Image.fromImageClick = false;
    }
    private void on_ok_clicked()
    {
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        if (home_ui.makeOver.makeOver_Image.fromImageClick == false)
        {
            home_ui.makeOver.makeOver_Image.SetMakeoverAnim(home_ui.makeOver.makeOver_Select.CurrentPanel);
            home_ui.makeOver.makeOver_Select.AnimFlyHeart = true;
            home_ui.makeOver.makeOver_Select.SelectUIShow(true, false);
        }
        on_close_clicked();
    }
}