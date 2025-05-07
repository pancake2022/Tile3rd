using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameRewardItemFly : WindowUI
{
    public class FlyItem : BaseUI
    {
        private Image iconImage;
        private Text rewardText;
        private Text bloomText;

        protected override void on_create()
        {
            iconImage = find_component<Image>("Image");
            rewardText = find_component<Text>("Image/Text_1");
            bloomText = find_component<Text>("Image/Text_2");
        }
        public void SetType(int value)
        {
            if (value == 1)//normal
            {
                play_sound("story_flower");
                iconImage.sprite = _ui_manager.FindSprite("M3Reward", "icon_flower", true);
                SetReward();
            }
            if (value == 2)//startbloom
            {
                play_sound("story_bike");
                iconImage.sprite = _ui_manager.FindSprite("M3Reward", "icon_tag", true);
                SetBloomText();
            }
        }
        public void SetReward()
        {
            var game_ui = _ui_manager.FindWindow<GameUI>();

            int randomRewardnormal = UnityEngine.Random.Range(GameConfigManager.GlobalConfig.Flower_Bloom_Normal_Min, GameConfigManager.GlobalConfig.Flower_Bloom_Normal_Max + 1);//普通的鲜花数量
            game_ui.gameRewardItem.game_reward_item_1 = game_ui.gameRewardItem.game_reward_item_1 + randomRewardnormal;
            rewardText.text = randomRewardnormal.ToString();
        }
        public void SetBloomText()
        {
            bloomText.text = "Bloom";
        }
        public void SetPosition()
        {
            var game_ui = _ui_manager.FindWindow<GameUI>();
            var v = transform.localPosition;
            int random = UnityEngine.Random.Range(-100, 100);//默认
            v.x = game_ui.rewardfly_positionX - 300 + random;
            v.y = game_ui.rewardfly_positionY + 280;
            transform.localPosition = v;
        }
        public FlyItem Init()
        {
            SetPosition();
            Show();
            return this;
        }
    }

    public static new string DefaultPrefabPath = "Game/Game_RewardFly";
    public GameUI Game;
    private RectTransform rewardfly_rt;
    private GameObject rewarditem_temp;

    public GameRewardItemFly Init(GameUI game)//PanelUI的初始化
    {
        Game = game;
        return this;
    }
    protected override void on_create()
    {
        rewardfly_rt = find_component<RectTransform>("Panel");
        rewarditem_temp = find_component<RectTransform>("itemTemplate", rewardfly_rt).gameObject;
        rewarditem_temp.SetActive(false);
    }
    public GameRewardItemFly StartFly()
    {
        CreateFlyItem();
        return this;
    }
    public void CreateFlyItem()
    {
        //FlyItemShow();
        FlyITemUnlock();
    }
    async void Bloom()
    {
        Game.gameRewardItem.BloomTimes--;
        rewarditem_temp.SetActive(false);
        int bloomrandom = UnityEngine.Random.Range(GameConfigManager.GlobalConfig.Flower_Bloom_Min, GameConfigManager.GlobalConfig.Flower_Bloom_Max + 1);
        int buffrandom = UnityEngine.Random.Range(GameConfigManager.GlobalConfig.Flower_Bloom_Buff_Min, GameConfigManager.GlobalConfig.Flower_Bloom_Buff_Max + 1);

        int flyCount = GameConfigManager.Tile2Storage.BloomBuffTimes > 0 ? buffrandom : bloomrandom;

        for (int i = 0; i < flyCount; i++)
        {
            var select_button = create_ui<FlyItem>(rewarditem_temp, rewardfly_rt);
            select_button.SetType(1);
            select_button.Init();

            _ = AutoDestroyAfterDelay(select_button, 2f); // 1 秒后销毁，不需要等待

            await Task.Delay(TimeSpan.FromSeconds(0.15));
        }
    }
    // 异步销毁函数（不阻塞主逻辑）
    private async Task AutoDestroyAfterDelay(FlyItem item, float delaySeconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

        if (item != null && item.gameObject != null) // 避免 null 错误
        {
            destroy_ui(item);
        }
    }
    async void normal()
    {
        rewarditem_temp.SetActive(false);
        var select_button = create_ui<FlyItem>(rewarditem_temp, rewardfly_rt);
        select_button.SetType(1);
        select_button.Init();
    }
    async void BloomStart()
    {
        Game.SetItemBloomMusic();
        rewarditem_temp.SetActive(false);
        var select_button = create_ui<FlyItem>(rewarditem_temp, rewardfly_rt);
        select_button.SetType(2);
        select_button.Init();
    }
    private void FlyItemShow()
    {
        int BloomRate = 100 - GameConfigManager.GlobalConfig.Flower_Bloom_Rate;
        int BloomBuffRate = 100 - GameConfigManager.GlobalConfig.Flower_Bloom_Buff_Rate;
        int randomNum = UnityEngine.Random.Range(1, 101);//整体的几率
        //bloomAll状态
        if (GameConfigManager.Tile2Storage.BloomAllTimes > 0)
        {
            Bloom();
        }
        //bloom状态
        else if (Game.gameRewardItem.BloomBuff == true)
        {
            if (Game.gameRewardItem.BloomTimes <= 1)
            {
                Bloom();
                Game.gameRewardItem.BloomBuff = false;
                Game.BloomFinishMusic();
            }
            else
                Bloom();
        }
        //普通状态
        else
        {
            if (GameConfigManager.Tile2Storage.BloomBuffTimes > 0) 
            {
                if (randomNum <= BloomBuffRate)
                    normal();
                else
                {
                    if (Game.leftCell >= 15)
                        BloomStartCount(GameConfigManager.GlobalConfig.Bloom_Times_Match);
                    else
                        normal();
                }
            }
            else
            {
                if (randomNum <= BloomRate)
                    normal();
                else
                {
                    if (Game.leftCell >= 15)
                        BloomStartCount(GameConfigManager.GlobalConfig.Bloom_Times_Match);
                    else
                        normal();
                }
            }
        }
    }
    private void FlyITemUnlock()
    {
        //前3关没有bloom
        //任务关第一次消除就送一个bloom
        //不完成任务关不让打第4关
        Debug.Log("flyleftcell"+Game.leftCell);
        if (GameConfigManager.LevelStorage.CurrentLevel <= 3)
            normal();
        else if (Game._panel_ui.Panel.ID == 2024901) 
        {
            if (Game.leftCell >= 42)
                normal();
            else if(Game.leftCell >= 39 && Game.leftCell < 42)
                BloomStartCount(5);
            else if(Game.gameRewardItem.BloomTimes < 1)
            {
                if (Game.leftCell >= 15 && Game.leftCell < 18)
                    BloomStartCount(5);
                else
                    normal();
            }
            else
                FlyItemShow();
        }
        else if (Game._panel_ui.Panel.ID == 2024005)
        {
            if (Game.leftCell < 45 && Game.leftCell > 42 && Game.gameRewardItem.BloomTimes <= 0)
                BloomStartCount(GameConfigManager.GlobalConfig.Bloom_Times_Match);
            else
                FlyItemShow();
        }
        else
            FlyItemShow();
    }
    private void BloomStartCount(int value)
    {
        BloomStart();
        Game.gameRewardItem.BloomBuff = true;
        Game.gameRewardItem.BloomTimes = Game.gameRewardItem.BloomTimes + value;
    }
}