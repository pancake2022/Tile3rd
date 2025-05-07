using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MakeOver_Select : BaseUI
{
    public class SelectButton : BaseUI
    {
        public MakeOverConfig Data;
        public Action<SelectButton> ClickCalback;
        private RectTransform icon;
        private Image iconImage;
        private RectTransform tipMark;
        private RectTransform isUseMark;
        private RectTransform locked;

        protected override void on_create()
        {
            icon = find_component<RectTransform>("icon");
            iconImage = find_component<Image>("icon");
            tipMark = find_component<RectTransform>("tipmark");
            locked = find_component<RectTransform>("locked");
            isUseMark = find_component<RectTransform>("selectmark");
            register_button(on_clicked);
        }
        private void on_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        private void SetIcon()
        {
            iconImage.sprite = _ui_manager.FindSprite($"{Data.Pack}", $"{Data.Icon}", true);
            iconImage.SetNativeSize();
        }
        public void SetSelected(bool value)
        {
            if (value == true) 
            {
                icon.localPosition = new Vector3(0, 20, 0);
                icon.localScale = new Vector3(1.1f, 1.1f);
                this.transform.localScale = new Vector3(1f, 1f);
            }
            if (value == false) 
            {
                icon.localPosition = new Vector3(0, 0, 0);
                icon.localScale = new Vector3(0.9f, 0.9f);
                this.transform.localScale = new Vector3(0.9f, 0.9f);
            }
        }
        public void SetLock(bool value)
        {
            locked.SetActive(value);
        }
        public void SetUsed(bool value)
        {
            isUseMark.SetActive(value);
        }
        public void SetTip(bool value)
        {
            tipMark.SetActive(value);
        }
        private void SetShow()
        {
            //任务家具需要解锁以后才显示
            if (Data.BuyType != 4)
                Show();
            if (GameConfigManager.MakeOverStorage.ImageUnlock[Data.ID])
                Show();
        }
        public SelectButton Init(MakeOverConfig data, Action<SelectButton> click_callback)
        {
            Data = data;
            ClickCalback = click_callback;

            SetShow();
            SetSelected(false);
            SetTip(false);
            SetUsed(false);
            SetIcon();
            SetLock(false);
            return this;
        }
    }

    public MakeOver makeOver;
    public SelectButton selectButton;
    public List<SelectButton> selectButtonList;
    public MakeOverConfig CurrentPanel;
    private RectTransform selectButton_rt;
    private GameObject selectButton_temp;
    private LoveLevel loveLevel;
    public Vector3 buttonPosition;
    public int AnimType;
    public bool AnimFlyHeart;

    public MakeOver_Select Init(MakeOver makeover)//PanelUI的初始化
    {
        makeOver = makeover;
        return this;
    }
    protected override void on_create()
    {
        register_button("Panel/SelectButton/Button/close", on_close_clicked);
        register_button("Panel/SelectButton/Button/button/image/use", on_use_clicked);
        register_button("Panel/SelectButton/Button/button/image/buy", on_buy_clicked);
        register_button("Panel/SelectButton/Button/button/image/watch", on_watch_clicked);

        selectButton_rt = find_component<RectTransform>("Panel/SelectButton/Scroll View/Viewport/Content");
        selectButton_temp = find_component<RectTransform>("ButtonTemplate", selectButton_rt).gameObject;
        selectButton_temp.SetActive(false);

        selectButtonList = new List<SelectButton>();
        CurrentPanel = null;
    }
    
    private void Update()
    {
        //货币 - 实时数量
        var Flower_Num = find_component<Text>("Panel/UI_Top/coin_bar/cointext");
        Flower_Num.text = GameConfigManager.CommonStorage.Flower.ToString();
    }

    public MakeOver_Select InitSelectList()
    {
        //点touch或image时进行init
        LoveBarInit();
        RefreshSelectButtonList();
        return this;
    }
    //好感度进度条
    public void LoveBarInit()
    {
        loveLevel = create_ui<LoveLevel>("Panel/UI_Top/level_bar");
    }
    private void RefreshSelectButtonList()
    {
        ClearSelectButtonList();
        CurrentPanel = null;
        var currenttouchlist = makeOver.makeOver_Touch.currentTouchList;
        bool isDefault = true;
        
        foreach (var makeoverdata in currenttouchlist)
        {
            selectButton = create_ui<SelectButton>(selectButton_temp, selectButton_rt);
            selectButton.Init(makeoverdata, p => on_panel_selected(p.Data));
            selectButtonList.Add(selectButton);

            if (GameConfigManager.MakeOverStorage.ImageUse[makeoverdata.ID]) 
            {
                on_panel_selected(makeoverdata);
                selectButton.SetUsed(true);
                isDefault = false;
            }
            if (GameConfigManager.MakeOverStorage.ImageUnlock[makeoverdata.ID] == false) 
            {
                selectButton.SetLock(true);
                if (GameConfigManager.MakeOverStorage.TouchPointCondition[makeOver.makeOver_Touch.CurrentTouch.ID] == 1)
                {
                    if (GameConfigManager.CommonStorage.Flower >= makeoverdata.BuyPrice) 
                        selectButton.SetTip(true);
                }
                if (GameConfigManager.MakeOverStorage.TouchPointCondition[makeOver.makeOver_Touch.CurrentTouch.ID] == 2)
                {
                    if (GameConfigManager.CommonStorage.Flower >= makeoverdata.SecondPrice) 
                        selectButton.SetTip(true);
                }
            }
        }
        
        //默认选中第一个
        if (isDefault)
            on_panel_selected(currenttouchlist[0]);
    }
    private void ClearSelectButtonList()
    {
        foreach (var select_button in selectButtonList)
            destroy_ui(select_button);
        selectButtonList.Clear();
    }
    
    //UI按钮操作 - 选中
    private void on_panel_selected(MakeOverConfig data)
    {
        play_sound("sound_tile_click");
        //当前的data是未选中的data
        if (CurrentPanel != data)
        {
            var current_select_button = selectButtonList.Find(a => a.Data == CurrentPanel);
            if (current_select_button)
            {
                current_select_button.SetSelected(false);
                makeOver.makeOver_Image.Select_ImageShow(false);
            }
            //点击select图标时
            CurrentPanel = data;
            current_select_button = selectButtonList.Find(a => a.Data == CurrentPanel);
            if (current_select_button)
            {
                current_select_button.SetSelected(true);
                makeOver.makeOver_Image.Select_ImageShow(true);
                makeOver.makeOver_Image.lerpCondition = 1;

                //如果第一次购买（当前touch未解锁）
                if (GameConfigManager.MakeOverStorage.TouchPointCondition[makeOver.makeOver_Touch.CurrentTouch.ID] == 1)
                    ButtonShow(data.BuyType, $"{ data.BuyPrice}");
                //如果第二次购买（当前touch已解锁）
                if (GameConfigManager.MakeOverStorage.TouchPointCondition[makeOver.makeOver_Touch.CurrentTouch.ID] == 2)
                    ButtonShow(data.BuyType, $"{ data.SecondPrice}");
                //(当前touch全部解锁)
                if (GameConfigManager.MakeOverStorage.TouchPointCondition[makeOver.makeOver_Touch.CurrentTouch.ID] == 3)
                    ButtonShow(3, $"");
            }

            //隐藏猫
            if (CurrentPanel.CatSelectHide && makeOver.makeOver_CatImage.catButton != null)
                makeOver.makeOver_CatImage.catButton.CatShow(false);
        }
    }

    //按钮的状态显示
    private void ButtonShow(int type, string Text)
    {
        var button = find_component<RectTransform>("Panel/SelectButton/Button/button");
        var button_use = find_component<RectTransform>("Panel/SelectButton/Button/button/image/use");
        var button_buy = find_component<RectTransform>("Panel/SelectButton/Button/button/image/buy");
        var button_buyText = find_component<Text>("Panel/SelectButton/Button/button/image/buy/Text");
        var button_watch = find_component<RectTransform>("Panel/SelectButton/Button/button/image/watch");
        button.SetActive(true);
        button_use.SetActive(false);
        button_buy.SetActive(false);
        button_watch.SetActive(false);
        button_buyText.text = $"{Text}";

        if (GameConfigManager.MakeOverStorage.ImageUnlock[CurrentPanel.ID])
            button_use.SetActive(true);
        else if (type == 1)
            button_buy.SetActive(true);
        else if (type == 2)
            button_watch.SetActive(true);
        else if (type == 3)
            button_use.SetActive(true);
    }
    //UI按钮操作 - 关闭
    public void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        makeOver.MakeOverUI_SelectClose();
        makeOver.makeOver_Image.InitImageButtonList();
        makeOver.Home.DefaultAnimSet();
        makeOver.makeOver_Image.lerpCondition = 2;
    }
    //UI按钮操作 - 购买
    private void on_buy_clicked()
    {
        play_sound("sound_button_click");
        if (GameConfigManager.MakeOverStorage.TouchPointCondition[makeOver.makeOver_Touch.CurrentTouch.ID] == 1)
        {
            if (GameConfigManager.CommonStorage.Flower >= CurrentPanel.BuyPrice)
            {
                GameConfigManager.CommonStorage.Flower = GameConfigManager.CommonStorage.Flower - CurrentPanel.BuyPrice;
                BuyRefresh();
            }
            else
                ShowNotice();
        }
        else if (GameConfigManager.MakeOverStorage.TouchPointCondition[makeOver.makeOver_Touch.CurrentTouch.ID] == 2)
        {
            if (GameConfigManager.CommonStorage.Flower >= CurrentPanel.SecondPrice)
            {
                GameConfigManager.CommonStorage.Flower = GameConfigManager.CommonStorage.Flower - CurrentPanel.SecondPrice;
                BuyRefresh();
            }
            else
                ShowNotice();
        }
    }
    private void BuyRefresh()
    {
        BuyGetImage();
        UseGetImage();
        AnimType = 1;
        SelectAnim();
    }
    private void ShowNotice()
    {
        GameConfigManager.ShareDataGlobalConfig._notice_id = 2;
        _ui_manager.OpenWindow<NoticeUI>();
    }
    //UI按钮操作 - 看广告
    private void on_watch_clicked()
    {
        play_sound("sound_button_click");
        Debug.Log("还未处理广告");
        //BuyGetImage();
        //UseGetImage();
        //AnimType = 2;
        //SelectAnim();
    }
    //UI按钮操作 - 使用
    private void on_use_clicked()
    {
        play_sound("sound_button_click");
        UseGetImage();
        AnimType = 3;
        SelectAnim();
    }
    //数据更新
    private void BuyGetImage()
    {
        SetTouchUnlock(CurrentPanel);
        SetImageUnlock();
        GetLoveExp();
        makeOver.Home.storyIcon.StoryTipInit();
        if (makeOver.Home.dailyTask_hint != null)
            makeOver.Home.dailyTask_hint.InitDailyTask_Hint();
    }
    private void UseGetImage()
    {
        SetUse();
        GetCatID();
    }
    public void SetTouchUnlock(MakeOverConfig current)
    {
        CurrentPanel = current;
        var item = makeOver.CurrentStoryTouchList.Find(a => a.ImageIDList.Contains(CurrentPanel.ID));
        GameConfigManager.MakeOverStorage.TouchPointCondition[item.ID] = 2;
        makeOver.makeOver_Touch.InitTouchButtonList();
    }
    private void SetImageUnlock()
    {
        GameConfigManager.MakeOverStorage.ImageUnlock[CurrentPanel.ID] = true;
        makeOver.makeOver_Touch.InitTouchButtonList();
    }
    private void GetLoveExp()
    {
        GameConfigManager.Tile2Storage.LoveLevelExpUp = CurrentPanel.LoveExp;
    }
    private void SetUse()
    {
        foreach (var item in makeOver.makeOver_Touch.currentTouchList)
            GameConfigManager.MakeOverStorage.ImageUse[item.ID] = false;
        GameConfigManager.MakeOverStorage.ImageUse[CurrentPanel.ID] = true;
    }
    public void GetCatID()
    {
        GameConfigManager.MakeOverStorage.CurrentCatID[CurrentPanel.StoryID] = CurrentPanel.CatID;
        //makeOver.makeOver_CatImage.CatAnimName = CurrentPanel.CatAnim;
    }

    public void SelectAnim()
    {
        GameConfigManager.ShareDataGlobalConfig._love_exp_pause = true;//需要处理
        _ui_manager.OpenWindow<MaskUI>();
        //buy
        if (AnimType == 1)
        {
            play_sound("story_flower");
            FlyHeart();
            FlyAnim(4);
        }
        //watch
        if (AnimType == 2)
        {
            FlyHeart();
            makeOver.makeOver_Image.SetMakeoverAnim(CurrentPanel);
            SelectUIShow(true, false);
        }
        //use
        if (AnimType == 3)
        {
            AnimFlyHeart = false;
            makeOver.makeOver_Image.SetMakeoverAnim(CurrentPanel);
            SelectUIShow(true, false);
        }
    }
    private void FlyHeart()
    {
        if (CurrentPanel.LoveExp == 0)
            AnimFlyHeart = false;
        else
            AnimFlyHeart = true;
    }
    public void GetButtonPosition()
    {
        var button_p = find_component<RectTransform>("Panel/SelectButton/Button/button");
        buttonPosition = button_p.localPosition;
    }
    //飞行动画/2.飞心/4.飞花
    public void FlyAnim(int value)
    {
        GameConfigManager.ShareDataGlobalConfig._home_fly = value;
        makeOver.Home.home_rewarditemfly();
    }

    //UI
    public void SelectUIShow(bool topUI, bool bottomUI)
    {
        var top = find_component<RectTransform>("Panel/UI_Top");
        var bottom = find_component<RectTransform>("Panel/SelectButton");
        top.SetActive(topUI);
        bottom.SetActive(bottomUI);
    }
}
