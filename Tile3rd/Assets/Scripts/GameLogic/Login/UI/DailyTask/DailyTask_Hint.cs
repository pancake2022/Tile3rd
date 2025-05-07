using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DailyTask_Hint : BaseUI
{
    public class CurrentTask : BaseUI
    {
        public DailyTaskConfig Task;
        private Action<CurrentTask> ClickCalback;

        public List<ItemConfig> itemlist;
        public ItemConfig currentitem;
        private RectTransform winstreak;
        private RectTransform findcat;
        private RectTransform guide;
        private RectTransform winstreak_desc;
        private RectTransform winstreak_claim;
        private RectTransform findcat_desc;
        private RectTransform findcat_claim;
        private RectTransform findcat_iconcat;
        private RectTransform findcat_iconreward;
        public int button_type;

        protected override void on_create()
        {
            winstreak = find_component<RectTransform>("bubble/taskType/WinStreak");
            findcat = find_component<RectTransform>("bubble/taskType/FindCat");
            guide = find_component<RectTransform>("guide");
            winstreak.SetActive(false);
            findcat.SetActive(false);
            guide.SetActive(false);
        }
        public CurrentTask Init(DailyTaskConfig task, Action<CurrentTask> click_callback)
        {
            Task = task;
            ClickCalback = click_callback;
            InitTaskType();
            return this;
        }
        public void InitTaskType()
        {
            //itemlist = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().ItemConfigList;
            itemlist = GameConfigManager.GameConfigGroup.ItemConfigList;
            currentitem = itemlist.Find(a => a.ID == Task.RewardID);
            if (Task.TaskType == 1)
                WinStreak();
            if (Task.TaskType == 2)
                FindCat();
        }
        private void WinStreak()
        {
            winstreak.SetActive(true);
            register_button("bubble/taskType/WinStreak/describe/start", winstreak_icon_clicked);
            register_button("bubble/taskType/WinStreak/button/claim", winstreak_claim_clicked);

            //定义transform
            winstreak_desc = find_component<RectTransform>("bubble/taskType/WinStreak/describe");
            winstreak_claim = find_component<RectTransform>("bubble/taskType/WinStreak/button");
            winstreak_desc.SetActive(false);
            winstreak_claim.SetActive(false);

            WinStreakIcon();
        }
        private void FindCat()
        {
            findcat.SetActive(true);
            register_button("bubble/taskType/FindCat/describe/start", findcat_icon_clicked);
            register_button("bubble/taskType/FindCat/button/claim", findcat_claim_clicked);

            findcat_desc = find_component<RectTransform>("bubble/taskType/FindCat/describe");
            findcat_claim = find_component<RectTransform>("bubble/taskType/FindCat/button");
            findcat_iconcat = find_component<RectTransform>("bubble/taskType/FindCat/icon/cat");
            findcat_iconreward = find_component<RectTransform>("bubble/taskType/FindCat/icon/reward");
            findcat_desc.SetActive(false);
            findcat_claim.SetActive(false);
            findcat_iconcat.SetActive(false);
            findcat_iconreward.SetActive(false);

            FindCatIcon();
        }
        public void Button_Desc()
        {
            findcat_iconcat.SetActive(true);
            winstreak_desc.SetActive(true);
            findcat_desc.SetActive(true);
        }
        public void Button_Claim()
        {
            findcat_iconreward.SetActive(true);
            winstreak_claim.SetActive(true);
            findcat_claim.SetActive(true);
        }
        public void winstreak_icon_clicked()
        {
            button_type = 1;
            ClickCalback?.Invoke(this);
        }
        public void winstreak_claim_clicked()
        {
            button_type = 2;
            ClickCalback?.Invoke(this);
        }
        public void findcat_icon_clicked()
        {
            button_type = 3;
            ClickCalback?.Invoke(this);
        }
        public void findcat_claim_clicked()
        {
            button_type = 4;
            ClickCalback?.Invoke(this);
        }
        public void ShowGuide()
        {
            guide.SetActive(true);
        }
        private void WinStreakIcon()
        {
            var taskBG = find_component<Image>("bubble/taskType/WinStreak/icon/Background");
            var taskIcon = find_component<Image>("bubble/taskType/WinStreak/icon/Fill Area/Fill");
            var rewardText = find_component<Text>("bubble/taskType/WinStreak/icon/Fill Area/Text");
            taskBG.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
            taskIcon.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
            rewardText.text = $"+{Task.RewardCount}";

            //slier
            var slider = find_component<Slider>("bubble/taskType/WinStreak/icon");
            slider.value = GameConfigManager.Tile2Storage.WinStreakCount;
            slider.maxValue = Task.TaskCount;
        }
        private void FindCatIcon()
        {
            var rewardImage = find_component<Image>("bubble/taskType/FindCat/icon/reward");
            var rewardText = find_component<Text>("bubble/taskType/FindCat/icon/reward/Text");
            rewardImage.sprite = _ui_manager.FindSprite($"{currentitem.Pack}", $"{currentitem.Icon}", true);
            rewardText.text = $"+{Task.RewardCount}";
        }
    }
    

    public HomeUI Home;
    public CurrentTask currentTask;
    private DailyTaskConfig taskData;

    public DailyTask_Hint Init(HomeUI home)
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        WinStreakCountInit();
    }
    private void GetCurrentTask()
    {
        var tasklist = GameConfigManager.GameConfigGroup.DailyTaskConfigList;
        taskData = tasklist.Find(a => a.ID == GameConfigManager.Tile2Storage.CurrentDailyTaskID);
        currentTask = create_ui<CurrentTask>("Panel");
        currentTask.Init(taskData, p => on_clicked(p.Task));
    }
    private void WinStreakCountInit()
    {
        if (GameConfigManager.Tile2Storage.WinStreakOffGame) 
            GameConfigManager.Tile2Storage.WinStreakCount = 0;
    }
    private void DailyTaskInit()
    {
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 1)
        {
            if (GameConfigManager.ShareDataGlobalConfig._is_catquest_active == false)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(false);
        GameConfigManager.ShareDataGlobalConfig._is_winstreak = false;
    }

    public DailyTask_Hint InitDailyTask_Hint()
    {
        RefreshDailyTask_Hint();
        return this;
    }
    private void RefreshDailyTask_Hint()
    {
        //dailytask的状态
        //需要跟猫任务相斥
        DailyTaskInit();
        GetCurrentTask();
        if (GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] == 0)
            DailyTaskCondition_0();
        if (GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] == 1)
            DailyTaskCondition_1();
        if (GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] == 2)
            DailyTaskCondition_2();
        if (GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] == 3)
            DailyTaskCondition_3();
    }

    private void DailyTaskCondition_0()
    {
        //任务未解锁
        //当前的任务链如果状态为1，则任务为已解锁
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[currentTask.Task.TaskChain] == 1)
            GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] = 1;
    }
    private void DailyTaskCondition_1()
    {
        //任务已解锁且连赢未完成
        currentTask.Button_Desc();

        //判断是否是连赢状态
        //只有当前任务在condition1的情况下，才会判断连赢（处理levelwin和revive）
        GameConfigManager.ShareDataGlobalConfig._is_winstreak = true;

        //condition1->2
        if (GameConfigManager.Tile2Storage.WinStreakCount >= currentTask.Task.TaskCount)
        {
            GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] = 2;
            GameConfigManager.ShareDataGlobalConfig._is_winstreak = false;
            RefreshDailyTask_Hint();
        }
    }
    private void DailyTaskCondition_2()
    {
        currentTask.Button_Claim();
    }
    private void DailyTaskCondition_3()
    {
        gameObject.SetActive(false);
        GetNextDailyTask();
    }
    
    private void on_clicked(DailyTaskConfig taskData)
    {
        if (currentTask.button_type == 1)
            Home.playUI.on_play_clicked();
        if (currentTask.button_type == 2)
            reward_claim_clicked();
        if (currentTask.button_type == 3)
            _ui_manager.OpenWindow<DailyTask_FindCatUI>();
        if (currentTask.button_type == 4)
            reward_claim_clicked();
    }
    private void reward_claim_clicked()
    {
        //设置当前任务状态
        GameConfigManager.Tile2Storage.DailyTaskCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskID] = 3;

        //获得奖励数据
        GameConfigManager.GiveItem(currentTask.Task.RewardID, currentTask.Task.RewardCount);

        //得奖动画
        Home.dailyTask_icon.InitDailyTask_Icon();
        GameConfigManager.ShareDataGlobalConfig._home_fly = 5;
        gameObject.SetActive(false);
        Home.home_rewarditemfly();

        //刷新touch
        Home.makeOver.makeOver_Touch.InitTouchButtonList();

        //winstreak归零
        if (currentTask.Task.TaskType == 1)
            GameConfigManager.Tile2Storage.WinStreakCount = 0;
        //if (currentTask.Task.TaskType == 2)
        //    Debug.Log("找猫领奖");

        //刷新storyicon
        Home.storyIcon.StoryTipInit();
    }

    private void GetNextDailyTask()
    {
        var taskList = GameConfigManager.GameConfigGroup.DailyTaskConfigList;
        int index = taskList.FindIndex(a => a.ID == GameConfigManager.Tile2Storage.CurrentDailyTaskID);
        var lastTask = taskList[taskList.Count - 1];

        if (GameConfigManager.Tile2Storage.CurrentDailyTaskID < lastTask.ID)
        {
            var nextTask = taskList[index + 1];
            GameConfigManager.Tile2Storage.CurrentDailyTaskID = nextTask.ID;
            InitDailyTask_Hint();
        }
        else
            Home.DailyTaskClear();
    }
}
