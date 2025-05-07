using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class SignIcon : BaseUI
{
    public class SignIconButton : BaseUI
    {
        public SignConfig Data;
        public Action<SignIconButton> ClickCalback;

        protected override void on_create()
        {
            register_button("Panel/Button/button_claim", on_clicked);
        }
        private void ShowInit()
        {
            var icon = find_component<Image>("Panel/Icon/icon");
            icon.sprite = _ui_manager.FindSprite($"{Data.IconPack}", $"{Data.Icon}", true);
            var Desc = find_component<RectTransform>("Panel/Icon/icon/Desc");
            var desc = find_component<Text>("Panel/Icon/icon/Desc");
            var Num = find_component<RectTransform>("Panel/Icon/Num");
            var num = find_component<Text>("Panel/Icon/Num");
            Desc.SetActive(false);
            Num.SetActive(false);
            if (Data.ID == 1 || Data.ID == 4)
            {
                Desc.SetActive(true);
                desc.text = $"Bloom\nAll";
            }
            if (Data.ID == 2)
            {
                Num.SetActive(true);
                num.text = $"x{Data.Reward_Num}";
            }
            if (Data.ID == 5)
            {
                Num.SetActive(true);
                num.text = $"x{GameConfigManager.Tile2Storage.SignCount * 10}";
            }
        }
        private void ConditionChange()
        {
            ShowInit();
            if (GameConfigManager.Tile2Storage.SignCondition[Data.ID] == 2)
                Condition_level();
            if (GameConfigManager.Tile2Storage.SignCondition[Data.ID] == 3)
                Condition_claim();
        }
        public void Condition_level()
        {
            var button_level = find_component<RectTransform>("Panel/Button/button_level");
            var button_claim = find_component<RectTransform>("Panel/Button/button_claim");
            button_level.SetActive(false);
            button_claim.SetActive(false);
            if (GameConfigManager.Tile2Storage.SignCondition[Data.ID] == 2)
                button_level.SetActive(true);
            if (GameConfigManager.Tile2Storage.SignCondition[Data.ID] == 3)
                button_claim.SetActive(true);

            var text = find_component<Text>("Panel/Button/button_level/Text");
            text.text = $"Win {GameConfigManager.Tile2Storage.SignLevelCD[Data.ID]} level";

            if (GameConfigManager.Tile2Storage.SignLevelCD[Data.ID] <= 0)
            {
                GameConfigManager.Tile2Storage.SignCondition[Data.ID] = 3;
                ConditionChange();
            }
        }
        private void Condition_claim()
        {
            var button_level = find_component<RectTransform>("Panel/Button/button_level");
            var button_claim = find_component<RectTransform>("Panel/Button/button_claim");
            button_level.SetActive(false);
            button_claim.SetActive(true);

            if (Data.ID == 2 && GameConfigManager.Tile2Storage.BloomBuffTimes > 3)
            {
                GameConfigManager.Tile2Storage.SignLevelCD[2] = GameConfigManager.Tile2Storage.BloomBuffTimes - 3;
                GameConfigManager.Tile2Storage.SignCondition[2] = 2;
                ConditionChange();
            }
        }
        private void on_clicked()
        {
            var panel = find_component<RectTransform>("Panel");
            panel.SetActive(false);
            ClickCalback?.Invoke(this);
        }
        
        public SignIconButton Init(SignConfig data, Action<SignIconButton> click_callback)
        {
            Data = data;
            ClickCalback = click_callback;
            ConditionChange();
            return this;
        }
    }
    
    public HomeUI Home;
    private List<SignConfig> signList;
    private SignConfig currentSign;
    public SignIconButton signIconButton;
    private Dictionary<int, SignIconButton> signIconButtonList;
    //public SignFlyTip signFlyTip;
    public float diff;
    public float hour;
    public float min;
    public float sec;
    public List<float> diffList = new List<float>();
    private int delayTime;
    public int flytip_positionX;

    public SignIcon Init(HomeUI home)
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        signList = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().SignConfigList;
        signIconButtonList = new Dictionary<int, SignIconButton>();
        register_button("Panel/Icon", on_icon_clicked);

        IsSignUnlock();
        RefreshSignIconButton();
        CreatTimeList();
    }
    private void IsSignUnlock()
    {
        if (GameConfigManager.Tile2Storage.IsSignUnlock == false)
        {
            GameConfigManager.Tile2Storage.SignCD[8] = DateTime.Now;//解锁时记录一个时间-与其他cd时间去做对比
            GameConfigManager.Tile2Storage.IsSignUnlock = true;
        }
    }
    private void CreatTimeList()
    {
        for (int i = 0; i < 8; i++)
        {
            var diffcell = new float();
            diffList.Add(diffcell);
        }
    }
    public void RefreshSignIconButton()
    {
        SignIconButtonInit();
        RefreshOtherIcon();
        RefreshTipShow();
        RefreshUIShow();
    }
    private void SignIconButtonInit()
    {
        var signButton_rt = find_component<RectTransform>("Panel/Order");
        foreach (var sign in signList)
        {
            if (signIconButtonList.Count < 3)
            {
                if (GameConfigManager.Tile2Storage.SignCondition[sign.ID] == 2 || GameConfigManager.Tile2Storage.SignCondition[sign.ID] == 3)
                {
                    if (!signIconButtonList.ContainsKey(sign.ID))
                    {
                        signIconButton = create_ui<SignIconButton>($"Template/SignIconTemplate", signButton_rt);
                        signIconButton.Init(sign, p => on_panel_selected(p.Data));
                        signIconButtonList.Add(sign.ID, signIconButton);
                    }
                }
            }
        }
    }
    private void on_panel_selected(SignConfig signConfig)
    {
        currentSign = signConfig;
        SignFlyTipInit();
        GetReward();
        RefreshOtherIcon();
        ClearTheSignIconButton();
    }
    private void RefreshOtherIcon()
    {
        foreach (var item in signIconButtonList)
        {
            item.Value.Init(item.Value.Data, p => on_panel_selected(p.Data));
        }
    }
    private void SignFlyTipInit()
    {
        foreach (var kvp in signIconButtonList.Where(k => k.Key == currentSign.ID))
            flytip_positionX = Convert.ToInt32(kvp.Value.transform.localPosition.x + 90);
        var signFlyTip_rt = find_component<RectTransform>("Panel/Flytips");
        var signFlyTip = create_ui<SignFlyTip>($"Template/SignFlyTip", signFlyTip_rt).Init(currentSign);
    }
    private void GetReward()
    {
        if (currentSign.ID == 1)//bloom倒计时
        {
            GameConfigManager.Tile2Storage.SignCD[currentSign.ID] = DateTime.Now;
            GameConfigManager.Tile2Storage.SignCondition[currentSign.ID] = 1;
            GameConfigManager.Tile2Storage.BloomAllTimes++;
            GameConfigManager.Tile2Storage.SignLevelCD[1] = GameConfigManager.Tile2Storage.BloomAllTimes;
            GameConfigManager.Tile2Storage.SignLevelCD[4] = GameConfigManager.Tile2Storage.BloomAllTimes;
            if (GameConfigManager.Tile2Storage.SignCondition[4] == 3)
                GameConfigManager.Tile2Storage.SignCondition[4] = 2;
        }
        if (currentSign.ID == 2)//bloombuff次
        {
            GameConfigManager.Tile2Storage.SignCD[currentSign.ID] = DateTime.Now;
            GameConfigManager.Tile2Storage.SignCondition[currentSign.ID] = 1;
            GameConfigManager.Tile2Storage.BloomBuffTimes += currentSign.Reward_Num;
            Home.BloomBuffInit();
            Home.levelChest.ButtonInit();
        }
        if (currentSign.ID == 3)//给一套牌
        {
            GameConfigManager.Tile2Storage.SignCondition[currentSign.ID] = 4;
            GameConfigManager.ShareDataGlobalConfig._sign_reward_id = 101;
            GameConfigManager.ShareDataGlobalConfig._notice_id = 5;
            GameConfigManager.Tile2Storage.TileUnlock[GameConfigManager.ShareDataGlobalConfig._sign_reward_id] = true;
            _ui_manager.OpenWindow<NoticeUI>();

        }
        if (currentSign.ID == 4)//bloom倒计时
        {
            GameConfigManager.Tile2Storage.SignCD[currentSign.ID] = DateTime.Now;
            GameConfigManager.Tile2Storage.SignCondition[currentSign.ID] = 1;
            GameConfigManager.Tile2Storage.BloomAllTimes++;
            GameConfigManager.Tile2Storage.SignLevelCD[1] = GameConfigManager.Tile2Storage.BloomAllTimes;
            GameConfigManager.Tile2Storage.SignLevelCD[4] = GameConfigManager.Tile2Storage.BloomAllTimes;
            if (GameConfigManager.Tile2Storage.SignCondition[1] == 3)
                GameConfigManager.Tile2Storage.SignCondition[1] = 2;
        }
        if (currentSign.ID == 5)//签到小花
        {
            GameConfigManager.Tile2Storage.SignCD[currentSign.ID] = DateTime.Now;
            GameConfigManager.Tile2Storage.SignCondition[currentSign.ID] = 1;
            GameConfigManager.CommonStorage.Flower = GameConfigManager.CommonStorage.Flower + GameConfigManager.Tile2Storage.SignCount * 10;
        }
        if (currentSign.ID == 6)//礼包
        {
            //直接给reward
            GameConfigManager.Tile2Storage.SignCD[currentSign.ID] = DateTime.Now;
            GameConfigManager.Tile2Storage.SignCondition[currentSign.ID] = 1;
            GameConfigManager.ShareDataGlobalConfig._sign_reward_id = 101;
            Home.bundleItem.GetCurrentBundle_Sign();
            _ui_manager.OpenWindow<RewardItemUI>();
        }
        if (currentSign.ID == 7)//story
        {
            GameConfigManager.Tile2Storage.SignCondition[currentSign.ID] = 4;
            GameConfigManager.ShareDataGlobalConfig._sign_reward_id = 101;
            GameConfigManager.ShareDataGlobalConfig._notice_id = 6;
            _ui_manager.OpenWindow<NoticeUI>();
            Home.makeOver.GetSignStory();
        }
    }
    async void ClearTheSignIconButton()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.3));
        foreach (var kvp in signIconButtonList.Where(k => k.Key == currentSign.ID))
        {
            destroy_ui(kvp.Value);
            signIconButtonList.Remove(kvp.Key);
            break;
        }
        RefreshSignIconButton();
    }
    public void RefreshTipShow()
    {
        var tip = find_component<RectTransform>("Panel/Tips");
        tip.SetActive(false);
        if (GameConfigManager.Tile2Storage.SignCondition[0] == 1) 
            tip.SetActive(true);
    }
    public void RefreshUIShow()
    {
        var sign_ui = _ui_manager.FindWindow<SignUI>();
        if (sign_ui != null)
            sign_ui.RefreshSignButton();
    }
    private void on_icon_clicked()
    {
        _ui_manager.OpenWindow<SignUI>();
    }

    private void Update()
    {
        SignCD();
        SignButtonUnlockCD();
        SignButtonCD();
    }
    private DateTime GetNextMidnight(DateTime dateTime)
    {
        DateTime midnight = dateTime.Date.AddDays(1); // 下一天的零点
        return midnight;
    }
    private void GetSignCD(DateTime startTime)
    {
        DateTime nextMidnight = GetNextMidnight(startTime);
        var endTime = DateTime.Now;
        TimeSpan difftime = nextMidnight.Subtract(endTime);
        diff = MathF.Floor((float)(difftime.TotalSeconds));
    }
    private void GetCountDown(DateTime startTime, float cdTime)
    {
        var endTime = DateTime.Now;
        TimeSpan difftime = startTime.Subtract(endTime);
        diff = cdTime + MathF.Floor((float)(difftime.TotalSeconds));
    }
    public void GetTimeShow(float value)
    {
        hour = MathF.Floor(value / 3600);
        min = MathF.Floor((value - hour * 3600) / 60);
        sec = MathF.Floor((value - hour * 3600 - min * 60));
    }

    //每日签到按钮的倒计时判断
    private void SignCD()
    {
        if (GameConfigManager.Tile2Storage.SignCondition[0] == 0)
        {
            GetSignCD(GameConfigManager.Tile2Storage.SignCD[0]);
            diffList[0] = diff;
            if (diffList[0] <= 0)
            {
                GameConfigManager.Tile2Storage.SignCondition[0] = 1;
                SignIconButtonInit();
                RefreshUIShow();
            }
        }
    }
    
    //每日签到各个奖励的解锁判断
    private void SignButtonUnlockCD()
    {
        foreach (var sign in signList)
        {
            if (GameConfigManager.Tile2Storage.SignCondition[sign.ID] == 0)
            {
                GetSignCD(GameConfigManager.Tile2Storage.SignCD[8].AddDays(sign.ID - 2));
                diffList[sign.ID] = diff;
                if (diffList[sign.ID] <= 0)
                {
                    GameConfigManager.Tile2Storage.SignCondition[sign.ID] = 2;
                    SetLevelCD(sign.ID);
                    SignIconButtonInit();
                    RefreshUIShow();
                }
            }
        }
    }
    //每日签到各个奖励的CD判断
    private void SignButtonCD()
    {
        foreach (var sign in signList)
        {
            if (GameConfigManager.Tile2Storage.SignCondition[sign.ID] == 1)
            {
                if (sign.ID == 5 || sign.ID == 6)
                    GetSignCD(GameConfigManager.Tile2Storage.SignCD[sign.ID]);
                else
                    GetCountDown(GameConfigManager.Tile2Storage.SignCD[sign.ID], sign.RefreshCD);
                diffList[sign.ID] = diff;
                if (diffList[sign.ID] <= 0)
                {
                    GameConfigManager.Tile2Storage.SignCondition[sign.ID] = 2;
                    SetLevelCD(sign.ID);
                    SignIconButtonInit();
                    RefreshUIShow();
                }
            }
        }
    }
    private void SetLevelCD(int value)
    {
        if (value == 1)
            GameConfigManager.Tile2Storage.SignLevelCD[1] = GameConfigManager.Tile2Storage.BloomAllTimes;
        if (value == 2)
            GameConfigManager.Tile2Storage.SignLevelCD[2] = GameConfigManager.Tile2Storage.BloomBuffTimes - 3;
        if (value == 4)
            GameConfigManager.Tile2Storage.SignLevelCD[4] = GameConfigManager.Tile2Storage.BloomAllTimes;
        if (value == 6)
            GameConfigManager.Tile2Storage.SignLevelCD[6] = 3;
    }
}
