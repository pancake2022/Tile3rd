using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;

public class HomeUI : WindowUI
{
    public static new string DefaultPrefabPath = "Home/UI_Home";
    public PlayUI playUI;
    public StoryIcon storyIcon;
    public MakeOver makeOver;//装修
    public LevelChest levelChest;//关卡宝箱
    public LoveLevel loveLevel;//好感度
    public CatQuest catQuest;//猫的任务
    public ShopBundleIcon shopBundleIcon;//付费礼包
    public BundleItems bundleItem;//广告礼包
    public BundleBloom bundleBloom;//bloom礼包icon
    public BloomBuff bloomBuff;//bloombuff
    public Collection collection;//收集牌
    public HomeRewardItemFly homeRewardFly;//飞行动画
    public Vector3 startPosition;//飞行动画初始坐标
    public DailyTask_Hint dailyTask_hint;
    public DailyTask_Icon dailyTask_icon;
    public SignIcon signIcon;
    public PopUI popUI;

    private CommonStorage commonStorage;
    private Tile2Storage tile2storage;
    private MakeOverStorage makeoverStorage;
    private LevelStorage levelStorage;
    private ShareDataGlobalConfig shareDataGlobalConfig;
    private GameConfigGroup gameConfigGroup;
    private GlobalConfig globalconfig;

    public M3Panel currentPanel;
    public BundleConfig currentBundle;

    protected override void on_create()
    {
        Property.UseCommonAnimation = false;
        Property.PlayOpenCloseSound = false;

        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
        tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        globalconfig = gameConfigGroup.GlobalConfigList[0];

        //创建按钮
        register_button("Panel/UI_Pop_Icon/UI_Top/Button_setting", on_setting_clicked);

        //测试按钮
        register_button("Panel/UI_Pop_Icon/UI_Right/item_test", on_icon_test_clicked);
    }

    protected override void on_open()
    {
        HomePanelShow(true);
        PlayUIInit();
        MakeOverInit();
        DefaultAnimSet();
        StoryIconInit();
        LoveLevelInit();
        BloomBuffInit();
        AudioInit();
        GoogleRevivew();
        SystemUnlock();
        PopUIInit();
        CreatePanel();
        SignInit();
    }
    private void Update()
    {
        //货币 - 实时数量
        var Flower_Num = find_component<Text>("Panel/UI_Pop_Icon/UI_Top/coin_bar/cointext");
        Flower_Num.text = commonStorage.Flower.ToString();
    }
    //系统解锁
    public void SystemUnlock()
    {
        if (levelStorage.LevelCount >= globalconfig.Unlock_LevelChest) 
            LevelChestInit();
        if (levelStorage.LevelCount >= globalconfig.Unlock_CatQuest)
            CatQuestInit();
        if (levelStorage.LevelCount >= globalconfig.Unlock_NewBundle)
            BundleItemInit();
        if (levelStorage.LevelCount >= globalconfig.Unlock_BloomBundle)
            BundleBloomInit();
        //if (levelStorage.LevelCount >= globalconfig.Unlock_Sign)
        //    SignInit();
        if (levelStorage.LevelCount >= globalconfig.Unlock_Collection)
            CollectionInit();
        if (makeoverStorage.CatQuestCondition[globalconfig.Unlock_DailyTask] == 3) 
        {
            
            DailyChainInit();
            DailyTaskInit();
        }
        if (levelStorage.LevelCount >= globalconfig.Unlock_Shop)
            ShopBundleIconInit();
    }
    private void PopUIInit()
    {
        popUI = create_ui<PopUI>("Panel").Init(this);
    }
    //创建关卡
    private void CreatePanel()
    {
        CSFramework.LevelConfig current_levelconfig = TileUtils.GetCurrentLevelConfig(levelStorage.CurrentLevel, _ui_manager.Framework.ConfigManager);
        var panel_config_ta = _ui_manager.Framework.ResourcesManager.LoadResource<TextAsset>($"{M3Const.M3PanelConfigPath}/{current_levelconfig.PanelID}");
        if (panel_config_ta != null)
        {
            try
            {
                currentPanel = JsonUtility.FromJson<M3Panel>(panel_config_ta.text);
                levelStorage.CurrentPanel = currentPanel;
            }
            catch (Exception e)
            {
                CSFramework.Logger.Error(e);
            }
        }
    }
    //挂play按钮
    private void PlayUIInit()
    {
        playUI = create_ui<PlayUI>("Panel/UI_Pop_Icon/UI_Bottom/Play").Init(this);
    }
    //挂story
    private void StoryIconInit()
    {
        storyIcon = create_ui<StoryIcon>("Panel/UI_Pop_Icon/UI_Bottom/Story").Init(this);
    }
    //挂makeover
    public void MakeOverInit()
    {
        MakeOverClear();
        var all_story = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().StoryConfigList;
        var _mo_rt = find_component<RectTransform>("MakeOver/MakeOver");
        var story = all_story.Find(a => a.ID == makeoverStorage.CurrentStoryID);
        makeOver = create_ui<MakeOver>($"MakeOver/UI_MakeOver_{story.ID.ToString("D2")}", _mo_rt).Init(this);
        makeOver.CurrentStoryCondition();
    }
    private void MakeOverClear()
    {
        if (makeOver != null)
            destroy_ui(makeOver);
    }
    //挂collection
    private void CollectionInit()
    {
        if (collection != null)
            destroy_ui(collection);
        var _col_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Left");
        collection = create_ui<Collection>("Home/UI_Icon_collection", _col_rt).Init(this);
    }
    //挂levelchest
    public void LevelChestInit()
    {
        if (levelChest != null)
            destroy_ui(levelChest);
        var _lc_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Bottom/LevelChest");
        levelChest = create_ui<LevelChest>("Home/LevelChest", _lc_rt).Init(this);
    }
    //挂bloomBuff
    public void BloomBuffInit()
    {
        var _bb_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Bottom/BloomBuff");
        if (tile2storage.BloomBuffTimes > 0)
            bloomBuff = create_ui<BloomBuff>("Home/BloomBuff", _bb_rt).Init(this);
    }
    //挂lovelevel
    private void LoveLevelInit()
    {
        loveLevel = create_ui<LoveLevel>("Panel/UI_Pop_Icon/UI_Top/level_bar").Init(this);
    }
    //猫任务
    public void CatQuestInit()
    {
        if (catQuest != null)
            destroy_ui(catQuest);
        var _quest_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Bottom/Quest");
        catQuest = create_ui<CatQuest>("MakeOver/UI_CatQuest", _quest_rt).Init(this);
        catQuest.InitCatQuest();
    }
    public void ShopBundleIconInit()
    {
        shopBundleIcon = create_ui<ShopBundleIcon>("Panel/UI_Pop_Icon/UI_Right/shop_bundle").Init(this);
    }
    public void BundleItemInit()
    {
        if (bundleItem != null)
            destroy_ui(bundleItem);
        var _bundle_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Right");
        bundleItem = create_ui<BundleItems>("Home/UI_Icon_bundle", _bundle_rt).Init(this);
    }
    //绽放礼包icon
    public void BundleBloomInit()
    {
        var _bloom_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Right");
        if (bundleBloom != null)
            destroy_ui(bundleBloom);
        if (tile2storage.BloomBuffTimes < 1)
        {
            if (levelChest._bloombuff_check)
                bundleBloom = create_ui<BundleBloom>("Home/UI_Icon_bloom", _bloom_rt).Init(this);
        }
    }
    //DailyTaskChain
    public void DailyChainInit()
    {
        //Icon
        DailyChainClear();
        var _dailytaskicon_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Left");
        dailyTask_icon = create_ui<DailyTask_Icon>("DailyTask/UI_DailyTask_Icon", _dailytaskicon_rt).Init(this);
        dailyTask_icon.InitDailyTask_Icon();
    }
    public void DailyChainClear()
    {
        if (dailyTask_icon != null)
            destroy_ui(dailyTask_icon);
    }
    public void DailyTaskInit()
    {
        //Hint
        DailyTaskClear();
        var _dailytaskhint_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Bottom/DailyTask");
        dailyTask_hint = create_ui<DailyTask_Hint>("DailyTask/UI_DailyTask_Hint", _dailytaskhint_rt).Init(this);
        dailyTask_hint.InitDailyTask_Hint();
    }
    public void DailyTaskClear()
    {
        if (dailyTask_hint != null)
            destroy_ui(dailyTask_hint);
    }
    public void SignInit()
    {
        if (signIcon != null) 
            destroy_ui(signIcon);
        var _sign_rt = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Left");
        signIcon = create_ui<SignIcon>("Home/UI_Icon_sign", _sign_rt).Init(this);
        SetSort();
    }
    public void SetSort()
    {
        GameObject father = GameObject.Find("Panel/UI_Pop_Icon/UI_Left");
        if (father != null)
        {
            foreach (Transform item in father.transform)
            {
                if (item.gameObject.name == "UI_Icon_sign")
                    item.SetAsFirstSibling();
            }
        }
        //Transform child = father.transform.GetChild(1);
    }
    //飞行奖励
    public void home_rewarditemfly()
    {
        ClearRewardFly();
        var _fly_rt = find_component<RectTransform>("Fly/levelchest");
        homeRewardFly = create_ui<HomeRewardItemFly>("Home/UI_Home_RewardFly", _fly_rt).Init(this);
        homeRewardFly.StartFly();
    }
    public void ClearRewardFly()
    {
        if (homeRewardFly != null)
            destroy_ui(homeRewardFly);
    }
    public void GetLevelChestPosition()
    {
        var levelchest_p = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Bottom/LevelChest");
        startPosition = levelchest_p.localPosition;
    }
    public void GetLoveLevelPosition()
    {
        var lovelevel_p = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Top/level_bar");
        startPosition = lovelevel_p.localPosition;
    }
    public void GetFlowerPosition()
    {
        var flower_p = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Top/coin_bar");
        startPosition = flower_p.localPosition;
    }
    public void GetDailyHintPosition()
    {
        var dailyhint_p = find_component<RectTransform>("Panel/UI_Pop_Icon/UI_Bottom/DailyTask");
        startPosition = dailyhint_p.localPosition;
    }

    //各种按钮功能
    //设置按钮
    private void on_setting_clicked()
    {
        shareDataGlobalConfig._winstreak_notice_type = 0;
        _ui_manager.OpenWindow<SettingUI>();
    }

    //显示/隐藏homeUI
    public void HomePanelShow(bool isShow)
    {
        var home_panel = find_component<RectTransform>("Panel");
        home_panel.SetActive(isShow);
    }

    //音乐音效
    private void AudioInit()
    {
        //播放home音乐
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._game_music_id);
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._game_music_bloom);
        shareDataGlobalConfig._home_music_id = _ui_manager.Framework.AudioManager.PlayMusic("music_home");

        //初始化音乐&音效
        if (commonStorage.MusicOpen)
            _ui_manager.Framework.AudioManager.SetMusicOpen(true);
        else
            _ui_manager.Framework.AudioManager.SetMusicOpen(false);

        if (commonStorage.SoundOpen)
            _ui_manager.Framework.AudioManager.SetSoundOpen(true);
        else
            _ui_manager.Framework.AudioManager.SetSoundOpen(false);
    }

    //google评分
    private void GoogleRevivew()
    {
        if (levelStorage.LevelCount >= globalconfig.Unlock_GoogleReview
            && commonStorage.Android_Reviewed <= 0
            && shareDataGlobalConfig._is_interstitial == false)
            GoogleReviewManager.Instance?.TryPromptReview();
    }
    public void DefaultAnimSet()
    {
        //默认家具动画
        foreach (var item in makeOver.makeOver_Image.imageButtonList)
            item.DefaultAnim();
        //默认猫动画
        if (makeOver.makeOver_CatImage.catButton != null)
        {
            makeOver.makeOver_CatImage.catButton.CatShow(true);
            makeOver.makeOver_CatImage.catButton.CatDefaultAnim();
        }
    }
    
    //点test按钮
    private void on_icon_test_clicked()
    {
        commonStorage.Flower = commonStorage.Flower + 1000;
        SetSort();
    }
}

