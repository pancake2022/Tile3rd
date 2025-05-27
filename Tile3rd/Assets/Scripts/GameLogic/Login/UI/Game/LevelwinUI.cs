using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelwinUI : WindowUI
{
    public static new string DefaultPrefabPath = "Reward/UI_Reward_levelwin";
    private int StartCount;
    private int FinishCount;
    public bool textCountStart;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_level_win");

        register_button("Panel/Cat_Anim/Button/Button_noAD", on_claim_clicked);

        //停止音乐
        _ui_manager.Framework.AudioManager.StopMusic(GameConfigManager.ShareDataGlobalConfig._game_music_id);
        _ui_manager.Framework.AudioManager.StopMusic(GameConfigManager.ShareDataGlobalConfig._game_music_bloom);

        //传输奖励数量
        var game_ui = _ui_manager.FindWindow<GameUI>();
        FinishCount = game_ui.gameRewardItem.game_reward_item_1;

        LevelInit();
        GetReward();
        ShowReward();
    }

    private void Update()//游戏更新
    {
        var textitem_1 = find_component<Text>("Panel/Cat_Anim/cat5/image_flower/item_1/Text");
        if (textCountStart == true)
        {
            if (StartCount < FinishCount)
            {
                StartCount++;
                textitem_1.text = StartCount.ToString();
            }
        }
    }
    private void LevelInit()
    {
        GetNextGameLevel();
        GetNextLevel();
        LevelChestProcess();
        winStreak();
        outItemJump();
        BloomAll();
        ShopCD();
        Interstitial();

        //bloombuff处理
        if (GameConfigManager.Tile2Storage.BloomBuffTimes > 0)
            GameConfigManager.Tile2Storage.BloomBuffTimes--;

        //评分展示倒计时
        GameConfigManager.CommonStorage.Android_Reviewed = GameConfigManager.CommonStorage.Android_Reviewed - 1;

        //签到关卡处理
        if (GameConfigManager.Tile2Storage.SignCondition[1] == 2)
            GameConfigManager.Tile2Storage.SignLevelCD[1]--;
        if (GameConfigManager.Tile2Storage.SignCondition[2] == 2)
            GameConfigManager.Tile2Storage.SignLevelCD[2]--;
        if (GameConfigManager.Tile2Storage.SignCondition[4] == 2)
            GameConfigManager.Tile2Storage.SignLevelCD[4]--;
        if (GameConfigManager.Tile2Storage.SignCondition[6] == 2)
            GameConfigManager.Tile2Storage.SignLevelCD[6]--;
    }
    private void GetReward()
    {
        //获得小花
        var game_ui = _ui_manager.FindWindow<GameUI>();
        if (game_ui.gameRewardItem.game_reward_item_1 > 0)
            GameConfigManager.CommonStorage.Flower = GameConfigManager.CommonStorage.Flower + game_ui.gameRewardItem.game_reward_item_1;

        //保底奖励
        if (GameConfigManager.LevelStorage.LevelCount == 4)
        {
            //如果第3个家具未解锁 - 给60
            //如果第3个家具解锁了 - 给50
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[3] >= 2)
            {
                if (GameConfigManager.CommonStorage.Flower < 50)
                    GameConfigManager.CommonStorage.Flower = 50;
            }
            else
            {
                if (GameConfigManager.CommonStorage.Flower < 60)
                    GameConfigManager.CommonStorage.Flower = 60;
            }
        }
    }
    private void ShowReward()
    {
        //显示小花
        var item_1 = find_component<RectTransform>("Panel/Cat_Anim/cat5/image_flower/item_1");
        item_1.SetActive(false);

        //播放小花动画
        var aniEvent = create_ui<AniGame>("Panel/Cat_Anim").Init(this);
        var anim = find_component<Animator>("Panel/Cat_Anim");
        anim.Play("Cat_give_flower");

        //显示按钮
        var ADbutton = find_component<RectTransform>("Panel/Cat_Anim/Button/Button_AD");
        var noADbutton = find_component<RectTransform>("Panel/Cat_Anim/Button/Button_noAD");
        ADbutton.SetActive(false);
        noADbutton.SetActive(true);
    }
    private void on_claim_clicked()
    {
        if (GameConfigManager.LevelStorage.LevelCount > GameConfigManager.GlobalConfig.Interstitial_UnlockLevel
            && GameConfigManager.GlobalConfig.Interstitial_CD_Initial > GameConfigManager.GlobalConfig.Interstitial_CD_Level)
            ADSManager.TriggerADSShow_Interstitial("Level_Win");
        else
            on_noAD();
    }
    private void on_noAD()
    {
        play_sound("sound_button_click");
        Close();
        _ui_manager.OpenWindow<HomeUI>();
        GameConfigManager.ShareDataGlobalConfig._is_interstitial = false;
    }
    private void GetNextLevel()
    {
        //普通关卡
        if (GameConfigManager.ShareDataGlobalConfig._level_condition == 1) 
        {
            //未来加个判断，曾经达到过满级
            CSFramework.LevelConfig lv = TileUtils.GetNextLevelConfig(GameConfigManager.LevelStorage.CurrentLevel, _ui_manager.Framework.ConfigManager);
            GameConfigManager.LevelStorage.LevelCount++;
            //循环主线关卡
            if (lv.ID > GameConfigManager.GlobalConfig.Level_Loop_Max)
                GameConfigManager.LevelStorage.CurrentLevel = GameConfigManager.GlobalConfig.Level_Loop_Min;
            else
                GameConfigManager.LevelStorage.CurrentLevel = lv.ID;
        }
        //主线猫任务关卡
        if (GameConfigManager.ShareDataGlobalConfig._level_condition == 2)
            GameConfigManager.MakeOverStorage.CatQuestCondition[GameConfigManager.MakeOverStorage.CurrentQuest.ID] = 2;
    }
    //道具为0时，进关卡会主动弹ui
    private void outItemJump()
    {
        GameConfigManager.ShareDataGlobalConfig._game_outitem_jump--;
    }
    //连赢和每日任务的处理
    private void winStreak()
    {
        GameConfigManager.Tile2Storage.WinStreakOffGame = false;

        //如果是连赢状态，连赢才会增加
        if (GameConfigManager.ShareDataGlobalConfig._is_winstreak)
            GameConfigManager.Tile2Storage.WinStreakCount++;
    }
    private void LevelChestProcess()
    {
        //关卡宝箱进度更新
        GameConfigManager.Tile2Storage.LevelChest_Process = 1;
        GameConfigManager.Tile2Storage.LevelChestItemList.Clear();
        //局内使用道具传递给存档
        for (int i = 0; i < 4; i++)
        {
            GameConfigManager.Tile2Storage.LevelChestItemList.Add(GameConfigManager.ShareDataGlobalConfig.itemlist[i]);
            GameConfigManager.ShareDataGlobalConfig.itemlist[i] = 0;
        }
    }
    //bloomAll状态处理
    private void BloomAll()
    {
        if (GameConfigManager.Tile2Storage.BloomAllTimes > 0)
            GameConfigManager.Tile2Storage.BloomAllTimes--;
    }
    //商店礼包现实CD
    private void ShopCD()
    {
        GameConfigManager.ShareDataGlobalConfig._shop_pop_cd++;
    }
    //插屏广告处理
    private void Interstitial()
    {
        //广告
        //插屏CD - 关卡数
        if (GameConfigManager.Tile2Storage.isnoADS == false)
        {
            GameConfigManager.GlobalConfig.Interstitial_CD_Initial++;
        }
    }
    //获得下一个GameLevel
    private void GetNextGameLevel()
    {
        //var all_gamelevel = GameConfigManager.GameConfigGroup.GameLevelConfigList;
        //var nextlevel = all_gamelevel.Find(a => a.Type == 1 && a.ID == GameConfigManager.LevelStorage.Current_GameLevel.ID + 1);
        //GameConfigManager.LevelStorage.Current_GameLevel = nextlevel;

        //需要判断达到最大值的情况
        //当前gamelevel状态设置为true
        GameConfigManager.LevelStorage.GameLevel_Condition[GameConfigManager.LevelStorage.Current_GameLevel.ID] = true;
    }
}
