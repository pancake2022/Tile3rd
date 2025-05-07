using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DailyTask_FindCatUI : WindowUI
{
    public class CurrentCat : BaseUI
    {
        public int catID;
        private Action<CurrentCat> ClickCalback;
        private RectTransform image0;
        private RectTransform image1;
        private RectTransform image2;

        protected override void on_create()
        {
            image0 = find_component<RectTransform>("image_0");
            image1 = find_component<RectTransform>("image_1");
            image2 = find_component<RectTransform>("image_2");
            image0.SetActive(false);
            image1.SetActive(true);
            image2.SetActive(false);
        }
        public void RereshCurrentCat()
        {
            register_button("image_1", on_clicked);

            //找到的猫进行显示
            if (GameConfigManager.Tile2Storage.FindCatCondition[catID] == true)
                image2.SetActive(true);
            else
                register_button("image_1", on_clicked);

            //提示中的hint进行显示
            if (GameConfigManager.Tile2Storage.FindCatHintCondition[catID] == 2)
                image0.SetActive(true);
            else
                image0.SetActive(false);
        }
        private void on_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        public CurrentCat Init(Action<CurrentCat> click_callback)
        {
            ClickCalback = click_callback;
            RereshCurrentCat();
            return this;
        }
    }

    public class CurrentFindCat : BaseUI
    {
        public CurrentCat currentCat;
        private Action<CurrentFindCat> ClickCalback;
        private HomeUI home_ui;
        private RectTransform allcat;
        public int minID = 0;//当前list的最小ID
        public int maxID = 0;//当前findcat里有几只猫

        protected override void on_create()
        {
            home_ui = _ui_manager.FindWindow<HomeUI>();
            allcat = find_component<RectTransform>("CatPoint");

            FindCatListUpdate();
            RefreshCatFind();
        }
        private void FindCatListUpdate()
        {
            foreach (RectTransform item in allcat)
            {
                maxID++;
                currentCat = create_ui<CurrentCat>($"CatPoint/{item.name}/panel");
                currentCat.catID = int.Parse($"{home_ui.dailyTask_hint.currentTask.Task.ID}{maxID}");
                minID = int.Parse($"{home_ui.dailyTask_hint.currentTask.Task.ID}1");

                if (GameConfigManager.Tile2Storage.FindCatCondition.ContainsKey(currentCat.catID))
                {
                    Debug.Log("没有数据更新");
                    continue;
                }
                else
                {
                    GameConfigManager.Tile2Storage.FindCatCondition.Add(currentCat.catID, false);
                    GameConfigManager.Tile2Storage.FindCatHintCondition.Add(currentCat.catID, 0);
                }
            }
        }
        public void RefreshCatFind()
        {
            maxID = 0;
            foreach (RectTransform item in allcat)
            {
                maxID++;
                currentCat = create_ui<CurrentCat>($"CatPoint/{item.name}/panel");
                currentCat.catID = int.Parse($"{home_ui.dailyTask_hint.currentTask.Task.ID}{maxID}");
                currentCat.Init(p => on_clicked(p));
            }
        }
        public void RVunlockCat()
        {
            foreach (var item in GameConfigManager.Tile2Storage.FindCatHintCondition)
            {
                if (item.Value == 0)
                {
                    GameConfigManager.Tile2Storage.FindCatHintCondition[item.Key] = 2;
                    break;
                }
            }
            RefreshCatFind();
        }
        private void on_clicked(CurrentCat currentCat)
        {
            //承上
            GameConfigManager.Tile2Storage.FindCatCondition[currentCat.catID] = true;
            GameConfigManager.Tile2Storage.FindCatHintCondition[currentCat.catID] = 3;
            currentCat.RereshCurrentCat();

            //启下
            ClickCalback?.Invoke(this);
        }
        public CurrentFindCat Init(Action<CurrentFindCat> click_callback)
        {
            ClickCalback = click_callback;
            return this;
        }
    }

    public static new string DefaultPrefabPath = "DailyTask/UI_DailyTask_FindCat";
    public CurrentFindCat currentFindCat;
    private HomeUI home_ui;
    private RectTransform buttonclose;
    private RectTransform buttonhint;
    private RectTransform buttonfinish;
    private int count = 0;
    private bool isHintFinish;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        home_ui = _ui_manager.FindWindow<HomeUI>();

        register_button("Panel/Button/close", on_close_clicked);
        register_button("Panel/Button/hint", on_hint_clicked);
        register_button("Panel/Button/finish", on_finish_clicked);

        var _findcat_rt = find_component<RectTransform>("Panel/FindCat");
        currentFindCat = create_ui<CurrentFindCat>($"DailyTask_FindCat/{home_ui.dailyTask_hint.currentTask.Task.FindCatPrefab}", _findcat_rt);
        currentFindCat.Init(p => on_clicked(currentFindCat));
        RefreshTask();
    }
    private void RefreshTask()
    {
        CountInit();
        HintInit();
        ButtonInit();
    }
    private void ButtonInit()
    {
        buttonclose = find_component<RectTransform>("Panel/Button/close");
        buttonhint = find_component<RectTransform>("Panel/Button/hint");
        buttonfinish = find_component<RectTransform>("Panel/Button/finish");
        buttonclose.SetActive(true);
        buttonhint.SetActive(true);
        buttonfinish.SetActive(false);

        //隐藏hint
        if (isHintFinish)
            buttonhint.SetActive(false);

        //完成
        if (count >= currentFindCat.maxID)
        {
            buttonclose.SetActive(false);
            buttonfinish.SetActive(true);
        }
    }
    private void CountInit()
    {
        count = 0;
        foreach (var item in GameConfigManager.Tile2Storage.FindCatCondition)
        {
            if (item.Key >= currentFindCat.minID) 
            {
                if (item.Value == true)
                    count++;
            }
        }
        var findcount = find_component<Text>("Panel/TaskAim/text");
        findcount.text = $"{count}/{currentFindCat.maxID}";
    }
    private void HintInit()
    {
        foreach (var item in GameConfigManager.Tile2Storage.FindCatHintCondition)
        {
            //用int值来做判断了
            if (item.Value >= 2)
                isHintFinish = true;
            else
            {
                isHintFinish = false;
                break;
            }
        }
    }

    //按钮
    private void on_clicked(CurrentFindCat currenfindtCat)
    {
        RefreshTask();
    }
    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
    }
    //RV按钮
    public void on_hint_clicked()
    {
        ADSManager.TriggerADSShow_Reward("FindCat_DailyTask");
    }
    public void Hint()
    {
        currentFindCat.RVunlockCat();
        currentFindCat.currentCat.RereshCurrentCat();
        RefreshTask();
    }
    private void on_finish_clicked()
    {
        GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] = 2;
        home_ui.dailyTask_hint.InitDailyTask_Hint();
        Close();
    }
}