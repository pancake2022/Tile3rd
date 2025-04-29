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

    private CommonStorage commonStorage;
    private Tile2Storage tile2storage;
    private LevelStorage levelStorage;
    private MakeOverStorage makeoverStorage;
    private ShareDataGlobalConfig shareDataGlobalConfig;
    private GameConfigGroup gameConfigGroup;
    private GlobalConfig globalconfig;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_level_win");

        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
        tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        globalconfig = gameConfigGroup.GlobalConfigList[0];

        register_button("Panel/Cat_Anim/Button/Button_noAD", on_claim_clicked);

        //停止音乐
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._game_music_id);
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._game_music_bloom);

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
        GetNextLevel();
        LevelChestProcess();
        winStreak();
        outItemJump();
        BloomAll();
        ShopCD();
        Interstitial();

        //bloombuff处理
        if (tile2storage.BloomBuffTimes > 0)
            tile2storage.BloomBuffTimes--;

        //评分展示倒计时
        commonStorage.Android_Reviewed = commonStorage.Android_Reviewed - 1;

        //签到关卡处理
        if (tile2storage.SignCondition[1] == 2)
            tile2storage.SignLevelCD[1]--;
        if (tile2storage.SignCondition[2] == 2)
            tile2storage.SignLevelCD[2]--;
        if (tile2storage.SignCondition[4] == 2)
            tile2storage.SignLevelCD[4]--;
        if (tile2storage.SignCondition[6] == 2)
            tile2storage.SignLevelCD[6]--;
    }
    private void GetReward()
    {
        //获得小花
        var game_ui = _ui_manager.FindWindow<GameUI>();
        if (game_ui.gameRewardItem.game_reward_item_1 > 0)
            commonStorage.Flower = commonStorage.Flower + game_ui.gameRewardItem.game_reward_item_1;

        //保底奖励
        if (levelStorage.LevelCount == 4)
        {
            //如果第3个家具未解锁 - 给60
            //如果第3个家具解锁了 - 给50
            if (makeoverStorage.TouchPointCondition[3] >= 2)
            {
                if (commonStorage.Flower < 50)
                    commonStorage.Flower = 50;
            }
            else
            {
                if (commonStorage.Flower < 60)
                    commonStorage.Flower = 60;
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
        if (levelStorage.LevelCount > globalconfig.Interstitial_UnlockLevel
            && globalconfig.Interstitial_CD_Initial > globalconfig.Interstitial_CD_Level)
            ADSManager.TriggerADSShow_Interstitial("Level_Win");
        else
            on_noAD();
    }
    private void on_noAD()
    {
        play_sound("sound_button_click");
        Close();
        _ui_manager.OpenWindow<HomeUI>();
        shareDataGlobalConfig._is_interstitial = false;
    }
    private void GetNextLevel()
    {
        //普通关卡
        if (shareDataGlobalConfig._level_condition == 1) 
        {
            //未来加个判断，曾经达到过满级
            CSFramework.LevelConfig lv = TileUtils.GetNextLevelConfig(levelStorage.CurrentLevel, _ui_manager.Framework.ConfigManager);
            levelStorage.LevelCount++;
            //循环主线关卡
            if (lv.ID > globalconfig.Level_Loop_Max)
                levelStorage.CurrentLevel = globalconfig.Level_Loop_Min;
            else
                levelStorage.CurrentLevel = lv.ID;
        }
        //主线猫任务关卡
        if (shareDataGlobalConfig._level_condition == 2)
            makeoverStorage.CatQuestCondition[makeoverStorage.CurrentQuest.ID] = 2;
    }
    //道具为0时，进关卡会主动弹ui
    private void outItemJump()
    {
        shareDataGlobalConfig._game_outitem_jump--;
    }
    //连赢和每日任务的处理
    private void winStreak()
    {
        tile2storage.WinStreakOffGame = false;

        //如果是连赢状态，连赢才会增加
        if (shareDataGlobalConfig._is_winstreak)
            tile2storage.WinStreakCount++;
    }
    private void LevelChestProcess()
    {
        //关卡宝箱进度更新
        tile2storage.LevelChest_Process = 1;
        tile2storage.LevelChestItemList.Clear();
        //局内使用道具传递给存档
        for (int i = 0; i < 4; i++)
        {
            tile2storage.LevelChestItemList.Add(shareDataGlobalConfig.itemlist[i]);
            shareDataGlobalConfig.itemlist[i] = 0;
        }
    }
    //bloomAll状态处理
    private void BloomAll()
    {
        if (tile2storage.BloomAllTimes > 0)
            tile2storage.BloomAllTimes--;
    }
    //商店礼包现实CD
    private void ShopCD()
    {
        shareDataGlobalConfig._shop_pop_cd++;
    }
    //插屏广告处理
    private void Interstitial()
    {
        //广告
        //插屏CD - 关卡数
        if (tile2storage.isnoADS == false)
        {
            globalconfig.Interstitial_CD_Initial++;
        }
    }
}
