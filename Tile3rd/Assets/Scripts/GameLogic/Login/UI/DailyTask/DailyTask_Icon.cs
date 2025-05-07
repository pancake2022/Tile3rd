using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DailyTask_Icon : BaseUI
{
    public class CurrentChain : BaseUI
    {
        public DailyTaskChainConfig Chain;
        private int sliderValue;
        private RectTransform chain_icon;
        private RectTransform chain_lock;
        private RectTransform chain_countdown;
        private RectTransform chain_progress;
        private RectTransform button_icon;
        private RectTransform button_claim;
        private RectTransform button_claim_lock;
        private bool chainFinish;

        protected override void on_create()
        {
            //创建按钮
            register_button("DailyTask/button_icon", on_icon_clicked);
            register_button("DailyTask/button_claim", on_claim_clicked);
            register_button("DailyTask/button_claim_lock", on_claim_lock_clicked);

            //prefab初始化
            chain_icon = find_component<RectTransform>("DailyTask/icon");
            chain_lock = find_component<RectTransform>("DailyTask/lock");
            chain_countdown = find_component<RectTransform>("DailyTask/countdown");
            chain_progress = find_component<RectTransform>("DailyTask/progress");
            button_icon = find_component<RectTransform>("DailyTask/button_icon");
            button_claim = find_component<RectTransform>("DailyTask/button_claim");
            button_claim_lock = find_component<RectTransform>("DailyTask/button_claim_lock");

            var allchild = find_component<RectTransform>("DailyTask");
            foreach (Transform child in allchild)
                child.SetActive(false);
        }
        public void on_icon_clicked()
        {
            _ui_manager.OpenWindow<DailyTask_NoticeUI_Icon>();
        }
        public void on_claim_lock_clicked()
        {
            StartCoroutine(WaitCheck_tip());
        }
        private IEnumerator WaitCheck_tip()
        {
            var tip = find_component<RectTransform>("DailyTask/button_claim_lock/tip");
            tip.SetActive(true);
            yield return new WaitForSeconds(1f);
            tip.SetActive(false);
        }
        public void on_claim_clicked()
        {
            _ui_manager.OpenWindow<DailyTask_NoticeUI_Icon>();
        }
        private void Icon()
        {
            chain_icon.SetActive(true);
            button_icon.SetActive(true);
            var imagelist = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().MakeOverConfigList;
            foreach (var item in imagelist)
            {
                if (Chain.MakeOverImageID == item.ID)
                {
                    var chainBG = find_component<Image>("DailyTask/icon/Background");
                    var chainIcon = find_component<Image>("DailyTask/icon/Fill Area/Fill");
                    chainBG.sprite = _ui_manager.FindSprite($"{item.Pack}", $"{item.Icon}", true);
                    chainIcon.sprite = _ui_manager.FindSprite($"{item.Pack}", $"{item.Icon}", true);
                }
            }
        }
        private void Slider()
        {
            sliderValue = 0;
            foreach (var item in Chain.ChainList)
            {
                if (GameConfigManager.Tile2Storage.DailyTaskCondition[item] == 3)
                    sliderValue++;
            }

            //进度条
            var sliderIcon = find_component<Slider>("DailyTask/icon");
            var sliderProgress = find_component<Slider>("DailyTask/progress");
            var sliderProgressText = find_component<Text>("DailyTask/progress/Fill Area/Text");
            sliderIcon.value = sliderValue;
            sliderIcon.maxValue = Chain.ChainList.Count;
            sliderProgress.value = sliderValue;
            sliderProgress.maxValue = Chain.ChainList.Count;
            sliderProgressText.text = $"{sliderProgress.value}/{Chain.ChainList.Count}";
        }
        public void showLock()
        {
            chain_lock.SetActive(true);
        }
        public void ShowCountDown()
        {
            chain_countdown.SetActive(true);
        }
        public void ShowProgress()
        {
            chain_progress.SetActive(true);

            foreach (var item in Chain.ChainList)
            {
                if (GameConfigManager.Tile2Storage.DailyTaskCondition[item] == 3)
                    chainFinish = true;
                else
                    chainFinish = false;
            }
            if (chainFinish)
                GameConfigManager.Tile2Storage.DailyTaskChainCondition[Chain.ID] = 2;
        }
        public void ShowButton()
        {
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[Chain.UnlockTouchID] >= 2)
                button_claim.SetActive(true);
            else
                button_claim_lock.SetActive(true);
        }
        public void ChainFinish()
        {
            button_claim.SetActive(false);
        }
        public CurrentChain Init(DailyTaskChainConfig chain)
        {
            Chain = chain;
            Icon();
            Slider();
            return this;
        }
    }

    public HomeUI Home;
    private CurrentChain currentChain;
    private DailyTaskChainConfig chainData;
    private int sliderValue;
    private bool chainFinish;
    public float diff;
    public float hour;
    public float min;
    public float sec;

    public DailyTask_Icon Init(HomeUI home)
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
    }
    private void Update()
    {
        ShowCountDown();
    }
    private void GetCurrentChain()
    {
        var chainlist = GameConfigManager.GameConfigGroup.DailyTaskChainConfigList;
        chainData = chainlist.Find(a => a.ID == GameConfigManager.Tile2Storage.CurrentDailyTaskChainID);
        currentChain = create_ui<CurrentChain>("Panel");
        currentChain.Init(chainData);
    }
    public DailyTask_Icon InitDailyTask_Icon()
    {
        RefreshDailyTask_Icon();
        return this;
    }
    private void RefreshDailyTask_Icon()
    {
        GetCurrentChain();
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 0)
            DailyTaskChainCondition_0();
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 1)
            DailyTaskChainCondition_1();
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 2)
            DailyTaskChainCondition_2();
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 3)
            DailyTaskChainCondition_3();
    }
    
    private void DailyTaskChainCondition_0()
    {
        currentChain.ShowCountDown();
        currentChain.showLock();
    }
    private void DailyTaskChainCondition_1()
    {
        currentChain.ShowProgress();
    }
    private void DailyTaskChainCondition_2()
    {
        currentChain.ShowButton();
    }
    private void DailyTaskChainCondition_3()
    {
        currentChain.ChainFinish();
        GetNextChainID();
    }

    public void GetCountDown()
    {
        if (GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] == 0)
        {
            //领奖刷新任务链时 - 记录starttime
            //用当前时间与starttime对比，判断是否超过了cd时间
            var endTime = DateTime.Now;
            TimeSpan difftime = GameConfigManager.Tile2Storage.DailyTaskChainStartTime.Subtract(endTime);
            diff = currentChain.Chain.UnlockCD + MathF.Floor((float)(difftime.TotalSeconds));
            hour = MathF.Floor(diff / 3600);
            min = MathF.Floor((diff - hour * 3600) / 60);
            sec = MathF.Floor((diff - hour * 3600 - min * 60));

            //cd结束后切换到下一个状态
            if (diff <= 0)
            {
                GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] = 1;
                RefreshDailyTask_Icon();
                Home.DailyTaskInit();
            }
        }
    }
    private void ShowCountDown()
    {
        GetCountDown();
        var timeText = find_component<Text>("Panel/DailyTask/countdown/Text");
        if (diff > 3600)
            timeText.text = $"{hour}h{min}m";
        if (diff >= 60 && diff <= 3600)
            timeText.text = $"{min}m{sec}";
        if (diff < 60)
            timeText.text = $"{sec}";
    }
    public void ChainFinish()
    {
        GameConfigManager.Tile2Storage.DailyTaskChainCondition[GameConfigManager.Tile2Storage.CurrentDailyTaskChainID] = 3;
        RefreshDailyTask_Icon();
    }
    private void GetNextChainID()
    {
        var chainList = GameConfigManager.GameConfigGroup.DailyTaskChainConfigList;
        int index = chainList.FindIndex(a => a.ID == GameConfigManager.Tile2Storage.CurrentDailyTaskChainID);
        var lastChain = chainList[chainList.Count - 1];

        if (GameConfigManager.Tile2Storage.CurrentDailyTaskChainID < lastChain.ID)
        {
            var nextChain = chainList[index + 1];
            GameConfigManager.Tile2Storage.CurrentDailyTaskChainID = nextChain.ID;
            RefreshDailyTask_Icon();
        }
        else
            Home.DailyChainClear();
    }

    //private void ShowInit()
    //{
    //    var dailytask = find_component<RectTransform>("Panel/DailyTask");
    //    foreach (Transform child in dailytask)
    //    {
    //        if (child.name == "icon")
    //            child.SetActive(true);
    //        else if (child.name == "button_icon")
    //            child.SetActive(true);
    //        else
    //            child.SetActive(false);
    //    }
    //}
}
