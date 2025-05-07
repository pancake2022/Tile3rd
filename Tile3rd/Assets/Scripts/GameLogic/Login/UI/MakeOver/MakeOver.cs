using CSFramework;
using UnityEngine;
using UnityEngine.UI;
//using System;
using System.Collections.Generic;

public class MakeOver : WindowUI
{
    public HomeUI Home;
    public MakeOver_Touch makeOver_Touch;
    public MakeOver_Select makeOver_Select;
    public MakeOver_Image makeOver_Image;
    public MakeOver_CatImage makeOver_CatImage;
    public MakeOver_Tips makeOver_Tips;
    public List<TouchPointConfig> CurrentStoryTouchList;
    public List<MakeOverConfig> CurrentStoryImageList;

    public int story_Reward_Condition;
    private bool alltouchunlock;
    private bool allimageunlock;

    public MakeOver Init(HomeUI home)//PanelUI的初始化
    {
        Home = home;
        makeOver_Touch.InitTouchButtonList();
        makeOver_Image.InitImageButtonList();
        makeOver_CatImage.InitCatButton();
        MakeOverUI_Init();
        return this;
    }
    protected override void on_create()
    {
        //创建list
        InitTouchList();
        InitImageList();

        //挂脚本
        makeOver_Touch = create_ui<MakeOver_Touch>("TouchPoint").Init(this);
        makeOver_Select = create_ui<MakeOver_Select>("MakeOver/UI_MakeOver_Select", "Select/select").Init(this);
        makeOver_Image = create_ui<MakeOver_Image>("Image").Init(this);
        makeOver_CatImage = create_ui<MakeOver_CatImage>("Image").Init(this);
        makeOver_Tips = create_ui<MakeOver_Tips>("Tips").Init(this);

        //按钮
        CloseButtonShow(false);
        register_button("Button/close", on_close_clicked);
    }

    public void InitTouchList()
    {
        var all_touch_config = GameConfigManager.GameConfigGroup.TouchPointConfigList;
        CurrentStoryTouchList = new List<TouchPointConfig>();
        foreach (var touch in all_touch_config)
        {
            if (touch.StoryID == GameConfigManager.MakeOverStorage.CurrentStoryID)
                CurrentStoryTouchList.Add(touch);
        }
    }
    public void InitImageList()
    {
        var all_image_config = GameConfigManager.GameConfigGroup.MakeOverConfigList;
        CurrentStoryImageList = new List<MakeOverConfig>();
        foreach (var image in all_image_config)
        {
            if (image.StoryID == GameConfigManager.MakeOverStorage.CurrentStoryID)
                CurrentStoryImageList.Add(image);
        }
    }

    //当前makoverstory的状态
    public void CurrentStoryCondition()
    {
        if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 0)
            Debug.Log("上一个story完成时，就把下一个story值置为1");
        if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 1)//touch（未领奖）
            StoryTouchFinish();
        if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 2)//touch（已领奖）image（未领奖）
            StoryImageFinish();
        if (GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] == 3)//image完成（已领奖）
            Debug.Log("此图已完美");
    }
    //全部touch解锁
    private void StoryTouchFinish()
    {
        foreach (var item in CurrentStoryTouchList)
        {
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[item.ID] <= 1)
            {
                alltouchunlock = false;
                break;
            }
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[item.ID] >= 2)
                alltouchunlock = true;
        }
        if (alltouchunlock)
        {
            story_Reward_Condition = 1;//0未解锁/1已解锁/2touch完成且领完奖励/3image完成
            _ui_manager.OpenWindow<Story_Reward_UI>();
        }
    }
    private void StoryImageFinish()//当前的剧情是否完成
    {
        //需要处理
        //判断ImageCount里的item是否true
        foreach (var item in CurrentStoryImageList)
        {
            if (item.ImageCount == true)
            {
                if (GameConfigManager.MakeOverStorage.ImageUnlock[item.ID] == false)
                {
                    allimageunlock = false;
                    break;
                }
                else
                    allimageunlock = true;
            }
        }
        if (allimageunlock)
        {
            story_Reward_Condition = 2;
            _ui_manager.OpenWindow<Story_Reward_UI>();
        }
    }

    //ui的初始状态
    private void MakeOverUI_Init()
    {
        SetTouchUIShow(true);
        SetSelectUIShow(false);
    }
    public void SetTouchUIShow(bool value)
    {
        var touch = find_component<RectTransform>("TouchPoint");
        touch.SetActive(value);
    }
    public void SetSelectUIShow(bool value)
    {
        var select = find_component<RectTransform>("Select");
        select.SetActive(value);
    }
    public void MakeOverUI_OnSelect()
    {
        Home.HomePanelShow(false);
        SetTouchUIShow(false);
        SetSelectUIShow(true);
        makeOver_Select.SelectUIShow(true,true);
    }
    public void MakeOverUI_SelectClose()
    {
        Home.HomePanelShow(true);
        SetTouchUIShow(true);
        SetSelectUIShow(false);
        makeOver_Select.SelectUIShow(false,false);
        Home.SetSort();
    }

    //找猫或换图的close
    public void CloseButtonShow(bool value)
    {
        var close = find_component<RectTransform>("Button/close");
        close.SetActive(value);
    }
    private void on_close_clicked()
    {
        CloseButtonShow(false);
        makeOver_Tips.TipsInit(false);
        makeOver_Tips.ButtonInit(false);
        Home.HomePanelShow(true);
        Home.catQuest.InitCatQuest();
    }
    public void GetSignStory()
    {
        //解锁并进入该story
        GameConfigManager.MakeOverStorage.CurrentStoryID = GameConfigManager.ShareDataGlobalConfig._sign_reward_id;
        GameConfigManager.MakeOverStorage.StoryCondition[GameConfigManager.MakeOverStorage.CurrentStoryID] = 1;
    }
    public void MakeoverRefresh()
    {
        Home.MakeOverInit();
        Home.DefaultAnimSet();
        Home.catQuest.RefreshCatQuest();
        Home.storyIcon.StoryTipInit();
    }
}
