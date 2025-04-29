using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MakeOver_Touch : BaseUI
{
    public class TouchButton : BaseUI
    {
        public TouchPointConfig TouchData;
        public Action<TouchButton> ClickCalback;
        private CommonStorage commonStorage;
        private MakeOverStorage makeoverStorage;
        private ShareDataGlobalConfig shareDataGlobalConfig;
        private List<MakeOverConfig> all_image;

        protected override void on_create()
        {
            commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
            makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
            shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
            all_image = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().MakeOverConfigList;

            register_button(on_clicked);
        }
        private void SetCondition()
        {
            SetBase();
            if (makeoverStorage.TouchPointCondition[TouchData.ID] == 0)
                SetCondition_Lock();
            if (makeoverStorage.TouchPointCondition[TouchData.ID] == 1)
                SetCondition_Touch();
            if (makeoverStorage.TouchPointCondition[TouchData.ID] == 2)
                SetCondition_Claw();
            if (makeoverStorage.TouchPointCondition[TouchData.ID] == 3)
                SetCondition_Complete();
        }
        private void SetBase()
        {
            gameObject.SetActive(false);
        }
        private void SetCondition_Lock()
        {
            gameObject.SetActive(false);

            foreach (var item in TouchData.Unlock) 
            {
                if (makeoverStorage.TouchPointCondition[item] >= 2)
                    makeoverStorage.TouchPointCondition[TouchData.ID] = 1;
            }
        }
        private void SetCondition_Touch()
        {
            SetPointShow(1);
            SetPointSprit(1);
            SetTips(1);

            if (makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] == 2)
            {
                if (makeoverStorage.TouchPointCondition[TouchData.ID] == 1)
                    makeoverStorage.TouchPointCondition[TouchData.ID] = 2;
            }
        }
        private void SetCondition_Claw()
        {
            SetPointShow(2);
            SetPointSprit(2);
            SetTips(2);

            int count = 0;
            foreach (var imageID in TouchData.ImageIDList)
            {
                var image = all_image.Find(a => a.ID == imageID);
                if (image.BuyType == 1)
                {
                    if (makeoverStorage.ImageUnlock[imageID] == false)
                        count++;
                }
            }
            if (count == 0)
                makeoverStorage.TouchPointCondition[TouchData.ID] = 3;
        }
        private void SetCondition_Complete()
        {
            gameObject.SetActive(false);
        }
        private void SetPointShow(int value)
        {
            if (value == 1)
            {
                if (TouchData.Type == 1)
                    gameObject.SetActive(true);
                if (TouchData.Type == 2)
                    gameObject.SetActive(false);
                if (shareDataGlobalConfig._is_catquest_active)
                    gameObject.SetActive(false);
            }
            if (value == 2)
            {
                if (makeoverStorage.StoryCondition[makeoverStorage.CurrentStoryID] == 2)
                    gameObject.SetActive(true);
            }
            //if (shareDataGlobalConfig._is_catquest_active)
            //    gameObject.SetActive(false);
        }
        private void SetPointSprit(int value)
        {
            var touch1 = find_component<RectTransform>("Panel/panel/touch_1");
            var touch2 = find_component<RectTransform>("Panel/panel/touch_2");
            touch1.SetActive(false);
            touch2.SetActive(false);
            if (value == 1)
                touch1.SetActive(true);
            if (value == 2)
                touch2.SetActive(true);
        }
        private void SetTips(int value)
        {
            var tip1 = find_component<RectTransform>("Panel/panel/tips_1");
            var tip2 = find_component<RectTransform>("Panel/panel/tips_2");
            tip1.SetActive(false);
            tip2.SetActive(false);
            foreach (var imageID in TouchData.ImageIDList)
            {
                var image = all_image.Find(a => a.ID == imageID && a.BuyType != 4);
                if (image != null) 
                {
                    if (value == 1)
                    {
                        if (commonStorage.Flower >= image.BuyPrice) 
                            tip1.SetActive(true);
                    }
                    if (value == 2)
                    {
                        if (commonStorage.Flower >= image.SecondPrice) 
                            tip2.SetActive(true);
                    }
                }
            }
        }
        private void on_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        public TouchButton Init(TouchPointConfig touchData, Action<TouchButton> click_callback)
        {
            TouchData = touchData;
            ClickCalback = click_callback;
            SetCondition();
            return this;
        }
    }

    public MakeOver makeOver;
    private RectTransform touchpoint;
    public TouchPointConfig CurrentTouch;
    public List<MakeOverConfig> currentTouchList;//声明布局的list
    public TouchButton touch_button;
    public List<TouchButton> touchButtonList;

    private CommonStorage commonStorage;
    private MakeOverStorage makeoverStorage;
    private LevelStorage levelStorage;
    
    protected override void on_create()
    {
        touchButtonList = new List<TouchButton>();
        CurrentTouch = null;

        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();//获取通用关卡存档
    }
    public MakeOver_Touch Init(MakeOver makeover)
    {
        makeOver = makeover;
        return this;
    }
    public MakeOver_Touch InitTouchButtonList()
    {
        ClearTouchButtonList();
        RefreshTouchButtonList();
        return this;
    }
    private void RefreshTouchButtonList()
    {
        var touchlist = makeOver.CurrentStoryTouchList;
        Guide();
        foreach (var touch in touchlist)
        {
            touchpoint = find_component<RectTransform>($"touchpoint/touch_{touch.PrefabID}");
            touch_button = create_ui<TouchButton>("MakeOver/UI_MakeOver_Touch", touchpoint);
            touch_button.Init(touch, p => on_touch_selected(p.TouchData));
            touchButtonList.Add(touch_button);
        }
    }
    public void ClearTouchButtonList()
    {
        foreach (var touch_button in touchButtonList)
            destroy_ui(touch_button);
        touchButtonList.Clear();
    }
    public void on_touch_selected(TouchPointConfig touch)
    {
        CurrentTouch = touch;
        currentTouchList = new List<MakeOverConfig>();
        foreach (var item in CurrentTouch.ImageIDList)
        {
            var makeoverdata = makeOver.CurrentStoryImageList.Find(a => a.ID == item);
            currentTouchList.Add(makeoverdata);
        }
        makeOver.makeOver_Select.InitSelectList();
        makeOver.MakeOverUI_OnSelect();
    }
    //新手引导
    private void Guide()
    {
        if (makeoverStorage.CurrentStoryID == 1)
        {
            var guide1 = find_component<RectTransform>("touchpoint/guide_1");
            var guide2 = find_component<RectTransform>("touchpoint/guide_2");
            guide1.SetActive(false);
            guide2.SetActive(false);
            if (makeoverStorage.TouchPointCondition[2] == 0)
                guide1.SetActive(true);
            if (makeoverStorage.TouchPointCondition[2] >= 2) 
            {
                if (makeoverStorage.TouchPointCondition[3] == 1 && levelStorage.LevelCount == 2)
                    guide2.SetActive(true);
            }
        }
    }
}
