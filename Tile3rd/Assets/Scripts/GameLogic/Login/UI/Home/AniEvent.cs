using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AniEvent : WindowUI
{
    private HomeUI home_ui;
    protected override void on_create()
    {
        home_ui = _ui_manager.FindWindow<HomeUI>();
    }
    //建造动画结束 - 播放猫动画
    public void MakeoverToCat()
    {
        home_ui.makeOver.makeOver_CatImage.InitCatButton();
        if (home_ui.makeOver.makeOver_CatImage.catButton != null)
            home_ui.makeOver.makeOver_CatImage.catButton.CatAnim();
    }
    //猫动画结束
    public void on_close()
    {
        if (home_ui.makeOver.makeOver_Select.AnimFlyHeart)
            StartCoroutine(WaitCheck_heartFly());
        else
            StartCoroutine(WaitCheck_noHeart());

        home_ui.makeOver.makeOver_Image.InitImageButtonList();
        home_ui.playUI.ShowGuide();
        home_ui.storyIcon.StoryTipInit();
    }
    private IEnumerator WaitCheck_heartFly()
    {
        home_ui.makeOver.makeOver_Select.FlyAnim(2);
        yield return new WaitForSeconds(1.5f);
        home_ui.makeOver.makeOver_Image.lerpCondition = 2;
        home_ui.DefaultAnimSet();
    }
    private IEnumerator WaitCheck_noHeart()
    {
        yield return new WaitForSeconds(0.3f);
        home_ui.makeOver.makeOver_Image.lerpCondition = 2;
        yield return new WaitForSeconds(1f);
        home_ui.DefaultAnimSet();

        if (home_ui.catQuest != null)
            home_ui.catQuest.InitCatQuest();
        home_ui.makeOver.CurrentStoryCondition();
        home_ui.makeOver.MakeOverUI_SelectClose();
        _ui_manager.TryCloseWindow<MaskUI>();
    }
    //弹出心动画结束
    public void FlyHeart()
    {
        home_ui.homeRewardFly.flyType = 4;
    }
}
