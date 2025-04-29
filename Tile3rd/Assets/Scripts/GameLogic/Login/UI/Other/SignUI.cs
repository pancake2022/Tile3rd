using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SignUI : WindowUI
{
    public class SignButton : BaseUI
    {
        public SignConfig Data;
        public Action<SignButton> ClickCalback;
        private Tile2Storage tile2storage;
        private ShareDataGlobalConfig shareDataGlobalConfig;
        private HomeUI home_ui;
        private RectTransform Desc;
        private RectTransform Banner;
        private RectTransform Button;
        private RectTransform RewardNum;
        private RectTransform BloomTime;
        private RectTransform level;
        private RectTransform claim;
        private RectTransform finish;
        private Text desc;
        private Text descLevel;
        private Text rewardNum;
        private Text bloomTime;
        private Coffee.UIExtensions.UIEffect grayBG;
        private Coffee.UIExtensions.UIEffect grayIcon;
        private Image imageBG;
        private Image imageIcon;
        private float Unlock_diff = new float();

        protected override void on_create()
        {
            home_ui = _ui_manager.FindWindow<HomeUI>();
            tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
            shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
            register_button("Panel/Condition/button/claim", on_claim_clicked);
            register_button("Panel/Condition/button/level", on_level_clicked);
            Banner = find_component<RectTransform>("Panel/Banner");
            Desc = find_component<RectTransform>("Panel/Condition/desc");
            Button = find_component<RectTransform>("Panel/Condition/button");
            RewardNum = find_component<RectTransform>("Panel/Icon/Num");
            BloomTime = find_component<RectTransform>("Panel/Icon/Image/Desc");
            desc = find_component<Text>("Panel/Condition/desc");
            descLevel = find_component<Text>("Panel/Condition/button/level/Text");
            rewardNum = find_component<Text>("Panel/Icon/Num");
            bloomTime = find_component<Text>("Panel/Icon/Image/Desc");
            grayBG = find_component<Coffee.UIExtensions.UIEffect>("Panel/BG/back");
            grayIcon = find_component<Coffee.UIExtensions.UIEffect>("Panel/Icon/Image");
            imageBG = find_component<Image>("Panel/BG/back");
            imageIcon = find_component<Image>("Panel/Icon/Image");
            level = find_component<RectTransform>("Panel/Condition/button/level");
            claim = find_component<RectTransform>("Panel/Condition/button/claim");
            finish = find_component<RectTransform>("Panel/Condition/button/finish");
        }
        private void Update()
        {
            //ShowUnlockCD();
            ShowCD();
        }
        private void ShowInit()
        {
            Banner.SetActive(false);
            Desc.SetActive(false);
            Button.SetActive(false);
            RewardNum.SetActive(false);
            BloomTime.SetActive(false);
            grayBG.enabled = false;
            grayIcon.enabled = false;
            level.SetActive(false);
            claim.SetActive(false);
            finish.SetActive(false);
            imageIcon.sprite = _ui_manager.FindSprite($"{Data.IconPack}", $"{Data.UIIcon}", true);

            if (Data.ID == 1)//bloom倒计时奖励
            {
                BloomTime.SetActive(true);
                imageBG.sprite = _ui_manager.FindSprite($"M3BackStory", $"shop_panel_5", true);
                bloomTime.text = $"Bloom\nAll";
            }
            if (Data.ID == 2)//bloombuff的次数
            {
                RewardNum.SetActive(true);
                imageBG.sprite = _ui_manager.FindSprite($"M3BackStory", $"shop_panel_5", true);
                rewardNum.text = $"x{Data.Reward_Num}";
            }
            if (Data.ID == 3)//奖励一套牌
            {
                Banner.SetActive(true);
                imageBG.sprite = _ui_manager.FindSprite($"M3BackStory", $"shop_panel_7", true);
            }
            if (Data.ID == 4)//bloom倒计时奖励
            {
                BloomTime.SetActive(true);
                imageBG.sprite = _ui_manager.FindSprite($"M3BackStory", $"shop_panel_5", true);
                bloomTime.text = $"Bloom\nAll";
            }
            if (Data.ID == 5)//奖励花-签到日期x10
            {
                RewardNum.SetActive(true);
                imageBG.sprite = _ui_manager.FindSprite($"M3BackStory", $"shop_panel_7", true);
                rewardNum.text = $"x{tile2storage.SignCount * 10}";
            }
            if (Data.ID == 6)//奖励礼包
            {
                imageBG.sprite = _ui_manager.FindSprite($"M3BackStory", $"shop_panel_5", true);
            }
            if (Data.ID == 7)//奖励story
            {
                Banner.SetActive(true);
                imageBG.sprite = _ui_manager.FindSprite($"M3BackStory", $"shop_panel_7", true);
            }
        }
        private void ShowUnlockCD()//测试显示用
        {
            if (tile2storage.SignCondition[Data.ID] == 0)
            {
                home_ui.signIcon.GetTimeShow(home_ui.signIcon.diffList[Data.ID]);
                desc.text = $"{home_ui.signIcon.hour}:{home_ui.signIcon.min}:{home_ui.signIcon.sec}";
            }
        }
        private void ShowCD()
        {
            if (tile2storage.SignCondition[Data.ID] == 1)
            {
                home_ui.signIcon.GetTimeShow(home_ui.signIcon.diffList[Data.ID]);
                if (home_ui.signIcon.hour >= 1)
                    desc.text = $"{home_ui.signIcon.hour}h{home_ui.signIcon.min}m";
                if (home_ui.signIcon.hour < 1 && home_ui.signIcon.min >= 1)
                    desc.text = $"{home_ui.signIcon.min}min";
                if (home_ui.signIcon.min < 1)
                    desc.text = $"{home_ui.signIcon.sec}s";
            }
        }
        private void Condition_Lock()
        {
            Desc.SetActive(true);
            grayBG.enabled = true;
            grayIcon.enabled = true;
            desc.text = $"Day {Data.ID}";
        }
        private void Condition_cd()
        {
            Desc.SetActive(true);
        }
        private void Condition_level()
        {
            Button.SetActive(true);
            level.SetActive(true);
            descLevel.text = $"Win {tile2storage.SignLevelCD[Data.ID]} level";

            if (tile2storage.SignLevelCD[Data.ID] <= 0)
            {
                tile2storage.SignCondition[Data.ID] = 3;
                ConditionChange();
            }
        }
        private void Condition_claim()
        {
            Button.SetActive(true);
            claim.SetActive(true);

            if (Data.ID == 2 && tile2storage.BloomBuffTimes > 3) 
            {
                tile2storage.SignLevelCD[2] = tile2storage.BloomBuffTimes - 3;
                tile2storage.SignCondition[2] = 2;
                ConditionChange();
            }
        }
        private void Condition_finish()
        {
            Button.SetActive(true);
            finish.SetActive(true);
        }
        private void ConditionChange()
        {
            ShowInit();
            if (tile2storage.SignCondition[Data.ID] == 0)
                Condition_Lock();
            if (tile2storage.SignCondition[Data.ID] == 1)
                Condition_cd();
            if (tile2storage.SignCondition[Data.ID] == 2)
                Condition_level();
            if (tile2storage.SignCondition[Data.ID] == 3)
                Condition_claim();
            if (tile2storage.SignCondition[Data.ID] == 4)
                Condition_finish();
        }
        private void on_claim_clicked()
        {
            GetReward();
            ConditionChange();
            ClickCalback?.Invoke(this);
        }
        private void on_level_clicked()
        {
            if (Data.ID == 6)//礼包
            {
                shareDataGlobalConfig._sign_reward_id = 101;
                home_ui.bundleItem.GetCurrentBundle_Sign();
                _ui_manager.OpenWindow<BundleItemsUI>();
            }
        }
        private void GetReward()
        {
            if (Data.ID == 1)//bloom倒计时
            {
                tile2storage.SignCD[Data.ID] = DateTime.Now;
                tile2storage.SignCondition[Data.ID] = 1;
                tile2storage.BloomAllTimes++;
                tile2storage.SignLevelCD[1] = tile2storage.BloomAllTimes;
                tile2storage.SignLevelCD[4] = tile2storage.BloomAllTimes;
                if (tile2storage.SignCondition[4] == 3) 
                    tile2storage.SignCondition[4] = 2;
                ConditionChange();
            }
            if (Data.ID == 2)//bloombuff次
            {
                tile2storage.SignCD[Data.ID] = DateTime.Now;
                tile2storage.SignCondition[Data.ID] = 1;
                tile2storage.BloomBuffTimes += Data.Reward_Num;
                home_ui.BloomBuffInit();
                home_ui.levelChest.ButtonInit();
            }
            if (Data.ID == 3)//给一套牌
            {
                tile2storage.SignCondition[Data.ID] = 4;
                shareDataGlobalConfig._sign_reward_id = 101;
                shareDataGlobalConfig._notice_id = 5;
                tile2storage.TileUnlock[shareDataGlobalConfig._sign_reward_id] = true;
                _ui_manager.OpenWindow<NoticeUI>();
                _ui_manager.TryCloseWindow<SignUI>();
            }
            if (Data.ID == 4)//bloom倒计时
            {
                tile2storage.SignCD[Data.ID] = DateTime.Now;
                tile2storage.SignCondition[Data.ID] = 1;
                tile2storage.BloomAllTimes++;
                tile2storage.SignLevelCD[1] = tile2storage.BloomAllTimes;
                tile2storage.SignLevelCD[4] = tile2storage.BloomAllTimes;
                if (tile2storage.SignCondition[1] == 3)
                    tile2storage.SignCondition[1] = 2;
                ConditionChange();
            }
            if (Data.ID == 5)//签到小花
            {
                tile2storage.SignCD[Data.ID] = DateTime.Now;
                tile2storage.SignCondition[Data.ID] = 1;
                var commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
                commonStorage.Flower = commonStorage.Flower + tile2storage.SignCount * 10;
            }
            if (Data.ID == 6)//礼包
            {
                //直接给reward
                tile2storage.SignCD[Data.ID] = DateTime.Now;
                tile2storage.SignCondition[Data.ID] = 1;
                shareDataGlobalConfig._sign_reward_id = 101;
                home_ui.bundleItem.GetCurrentBundle_Sign();
                _ui_manager.OpenWindow<RewardItemUI>();
                _ui_manager.TryCloseWindow<SignUI>();
            }
            if (Data.ID == 7)//story
            {
                tile2storage.SignCondition[Data.ID] = 4;
                shareDataGlobalConfig._sign_reward_id = 101;
                shareDataGlobalConfig._notice_id = 6;
                _ui_manager.OpenWindow<NoticeUI>();
                _ui_manager.TryCloseWindow<SignUI>();
                home_ui.makeOver.GetSignStory();
            }
            home_ui.SignInit();
        }
        public SignButton Init(SignConfig data, Action<SignButton> click_callback)
        {
            Data = data;
            ClickCalback = click_callback;
            ConditionChange();
            return this;
        }
    }
    
    public static new string DefaultPrefabPath = "Panel/UI_Panel_sign";
    public SignButton signButton;
    public List<SignButton> signButtonList;
    private Tile2Storage tile2storage;
    private SignConfig currentSign;
    private HomeUI home_ui;
    private float UIdiff;
    public SignFlyTip signFlyTip;
    public int flytip_positionX;
    public int flytip_positionY;
    private Tween flowerTween;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        home_ui = _ui_manager.FindWindow<HomeUI>();
        tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        signButtonList = new List<SignButton>();

        register_button("Panel/Button/Button_close", on_close_clicked);
        register_button("Panel/Button/Button_sign/Button_sign", on_sign_clicked);

        RefreshSignButton();
    }
    private void Update()
    {
        SignButtonCD();
    }
    public void RefreshSignButton()
    {
        SignButtonInit();
        TitleInit();
        ButtonInit();
    }
    private void ClearSignButtonList()
    {
        foreach (var signButton in signButtonList)
            destroy_ui(signButton);
        signButtonList.Clear();
    }
    private void SignButtonInit()
    {
        ClearSignButtonList();
        var signList = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().SignConfigList;
        var signButton_rt = find_component<RectTransform>("Panel/Reward");
        foreach (var sign in signList)
        {
            signButton = create_ui<SignButton>($"Template/{sign.Style}", signButton_rt);
            signButton.Init(sign, p => on_panel_selected(p.Data));
            signButtonList.Add(signButton);
        }
    }
    private void TitleInit()
    {
        var title = find_component<Text>("Panel/Title/Text");
        title.text = $"Signed {tile2storage.SignCount} days";
    }
    private void ButtonInit()
    {
        //2个状态，倒计时和claim
        var button_cd = find_component<RectTransform>("Panel/Button/Button_cd");
        var button_sign = find_component<RectTransform>("Panel/Button/Button_sign");
        
        button_cd.SetActive(false);
        button_sign.SetActive(false);

        if (tile2storage.SignCondition[0] == 0)
            button_cd.SetActive(true);
        if (tile2storage.SignCondition[0] == 1)
            button_sign.SetActive(true);
    }
    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        home_ui.signIcon.RefreshSignIconButton();
        Close();
    }
    private void SignButtonCD()
    {
        if (tile2storage.SignCondition[0] == 0)
        {
            home_ui.signIcon.GetTimeShow(home_ui.signIcon.diffList[0]);
            var timeText = find_component<Text>("Panel/Button/Button_cd/Button_cd/Text");
            timeText.text = $"{home_ui.signIcon.hour}h{home_ui.signIcon.min}m{home_ui.signIcon.sec}";
        }
    }
    private void on_sign_clicked()
    {
        //获得猫的好感
        tile2storage.SignCondition[0] = 0;
        tile2storage.SignCount++;
        tile2storage.SignCD[0] = DateTime.Now;
        StartCoroutine(FlyFlower());
        RefreshSignButton();
    }
    private void on_panel_selected(SignConfig signConfig)
    {
        currentSign = signConfig;
        StartCoroutine(DelayGetPosition());
        RefreshSignButton();
    }
    private void SignFlyTipInit()
    {
        var signFlyTip_rt = find_component<RectTransform>("Panel/Flytips");
        signFlyTip = create_ui<SignFlyTip>($"Template/SignFlyTip", signFlyTip_rt).Init(currentSign);
    }

    private IEnumerator DelayGetPosition()
    {
        // 等待一帧，让LayoutGroup更新
        yield return null;


        foreach (var signButton in signButtonList)
        {
            if (currentSign.ID == signButton.Data.ID)
            {
                flytip_positionX = Convert.ToInt32(signButton.transform.localPosition.x);
                flytip_positionY = Convert.ToInt32(signButton.transform.localPosition.y + 30);
                SignFlyTipInit();
                //var signFlyTip_rt = find_component<RectTransform>("Panel/Flytips");
                //signFlyTip = create_ui<SignFlyTip>($"Template/SignFlyTip", signFlyTip_rt).Init(currentSign);
            }
        }
        yield return new WaitForSeconds(1f);
        if (signFlyTip != null)
        {
            destroy_ui(signFlyTip);
        }
    }

    private IEnumerator FlyFlower()
    {
        yield return null; // 确保 Layout 更新完

        var startImageRT = find_component<RectTransform>("Panel/Button/Button_sign/Cat2/Panel/image/image (1)");
        if (startImageRT == null) yield break;

        Vector3 startPos = startImageRT.position;

        var endButton = signButtonList.Find(a => a.Data.ID == 5);
        if (endButton == null) yield break;

        Vector3 endPos = endButton.transform.position;

        Transform gameManagerRoot = _ui_manager._context.WindowUILayerDict[UILayer.Message];
        var flowerPrefab = create_ui<SignFlower>("Template/SignFlower", gameManagerRoot);
        if (flowerPrefab == null) yield break;

        RectTransform flowerRT = flowerPrefab.GetComponent<RectTransform>();
        if (flowerRT == null) yield break;

        flowerRT.position = startPos;

        float duration = 0.6f;
        float timer = 0f;

        while (timer < duration)
        {
            if (flowerRT == null || flowerRT.gameObject == null) yield break;

            timer += Time.deltaTime;
            float t = timer / duration;

            flowerRT.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (flowerRT != null)
            flowerRT.position = endPos;

        if (flowerPrefab != null)
        {
            destroy_ui(flowerPrefab);
        }
            
    }
}