using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameUI : WindowUI
{
    public static new string DefaultPrefabPath = "Game/UI_Game";
    public M3GamePanelUI _panel_ui;
    public GameTileRandom tileRandom;
    public GameBackground gameBG;
    public GameItemGroupUI gameItemGroupUI;//局内道具
    public GameGuideUI gameGuideUI;//新手引导
    public GamePropsFly gamePropsFly;//局内道具飞行
    public GameRewardItem gameRewardItem;//局内奖励
    public GameRewardItemFly gameRewardItemFly;//局内奖励飞行
    public int rewardfly_positionX;//消除特效的坐标 - 修正
    public int rewardfly_positionY;//消除特效的坐标 - 修正

    private LevelStorage levelStorage;
    private ShareDataGlobalConfig shareDataGlobalConfig;
    private CommonStorage commonStorage;
    private Tile2Storage tile2Storage;
    private List<LevelConfig> all_level;
    public LevelConfig level;

    public int leftCell;
    public int totalCell;
    public bool isPause;

    protected override void on_create()
    {
        //var home_ui = _ui_manager.FindWindow<HomeUI>();
        Property.CommonAnimationTransform = transform.Find("Panel");
        commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
        tile2Storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        all_level = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().LevelConfigList;
        level = all_level.Find(a => a.PanelID == levelStorage.CurrentPanel.ID);
        GameActiveDelay();

        //创建游戏布局
        _panel_ui = create_ui<M3GamePanelUI>("Panel");
        tileRandom = create_ui<GameTileRandom>("Panel").Init(this);

        //挂局内奖励
        var _reward_item = find_component<RectTransform>("Panel/Game_Reward");
        gameRewardItem = create_ui<GameRewardItem>("Game/Game_Reward", _reward_item).Init(this);

        //挂道具组/复活
        var _item_group = find_component<RectTransform>("Panel/PropGroup");
        gameItemGroupUI = create_ui<GameItemGroupUI>("Game/Game_ItemGroup", _item_group).Init(this);

        //游戏背景
        gameBG = create_ui<GameBackground>("BG").Init(this);
        gameBG.BackGroundChange();

        //创建按钮
        register_button("Panel/Setting/Button", on_setting_clicked);
        register_button("Panel/LevelWin", on_win_clicked);

        SetGameMusic();
        TileRandomShowInit();
        LoadingInterstitial();

        //显示level等级
        var Game_Level = find_component<Text>("Panel/Title/Text");
        if (shareDataGlobalConfig._level_condition == 1)
            Game_Level.text = "Level " + levelStorage.LevelCount.ToString();
        if (shareDataGlobalConfig._level_condition == 2)
            Game_Level.text = "";

        //新手引导相关
        gameGuideUI = create_ui<GameGuideUI>("Panel/Guide");
    }
    protected override void on_open()
    {
        outItemJump();
        gameGuideUI.Init(this);
        _panel_ui.CollectionUI.CallBack_Match(p => Match());
        _panel_ui.CollectionUI.CallBack_Collect(p => Collect());

        //判断游戏过程中杀进程
        if (shareDataGlobalConfig._is_winstreak)
            tile2Storage.WinStreakOffGame = true;
    }
    private void Update()//游戏更新
    {
        //
    }
    public void GamePause()
    {
        isPause = true;
    }
    public async void GameActiveDelay()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.5));
        isPause = false;
    }
    private void TileRandomShowInit()
    {
        var cat_stay = find_component<RectTransform>("Panel/TileRandom_Cat_Stay");
        var cat_run = find_component<RectTransform>("Panel/TileRandom_Cat_Stay");
        var change = find_component<RectTransform>("Panel/TileRandom_Change");
        cat_stay.SetActive(false);
        cat_run.SetActive(false);
        change.SetActive(false);
        if (level.Type == 1)
            cat_stay.SetActive(true);
    }
    //设置游戏内音乐
    public void SetGameMusic()
    {
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._home_music_id);
        _ui_manager.Framework.AudioManager.StopMusic(shareDataGlobalConfig._game_music_bloom);
        shareDataGlobalConfig._game_music_id = _ui_manager.Framework.AudioManager.PlayMusic("music_game");
    }
    public void SetItemBloomMusic()
    {
        if (gameRewardItem.BloomTimes <= 0)
            _ui_manager.Framework.AudioManager.SetMusicPitch(1.3f);
    }
    public void BloomFinishMusic()
    {
        _ui_manager.Framework.AudioManager.SetMusicPitch(1.0f);
    }
    public void ReviveBloomMusic()
    {
        shareDataGlobalConfig._game_music_id = _ui_manager.Framework.AudioManager.PlayMusic("music_game");
        _ui_manager.Framework.AudioManager.SetMusicPitch(1.3f);
    }

    //初始化游戏布局
    public GameUI Init(M3Panel panel)
    {
        leftCell = 0;
        totalCell = 0;
        _panel_ui.Init(panel);

        //获得leftcell的数量
        for (int i = 0; i < _panel_ui.LayerUIArray.Length; i++)
        {
            leftCell = leftCell + _panel_ui.LayerUIArray[i].Layer.CellList.Count;
        }
        totalCell = leftCell;
        Debug.Log("cell总数：" + totalCell + " / " + "初始lefetcell=" + leftCell);
        return this;
    }
    //点设置按钮
    private void on_setting_clicked()
    {
        shareDataGlobalConfig._winstreak_notice_type = 1;
        _ui_manager.OpenWindow<GameSettingUI>();
    }


    //消除飞行奖励
    public void game_rewarditemfly()
    //public async Task game_rewarditemfly()
    {
        var _fly_rt = find_component<RectTransform>("Panel/Game_RewardFly");
        gameRewardItemFly = create_ui<GameRewardItemFly>("Game/Game_RewardFly", _fly_rt).Init(this);

        _panel_ui.CollectionUI.textcount++;
        if (_panel_ui.CollectionUI.textcount == 1)
        {
            rewardfly_positionX = Convert.ToInt32(_panel_ui.CollectionUI.CollectedCellUIList[_panel_ui.CollectionUI.index].transform.localPosition.x);
            rewardfly_positionY = Convert.ToInt32(_panel_ui.CollectionUI.transform.localPosition.y);
            gameRewardItemFly.StartFly();
        }

        //await Task.Delay(TimeSpan.FromSeconds(2));

        //if (gameRewardItemFly != null)
        //    ClearRewardFly();
    }
    private void ClearRewardFly()
    {
        destroy_ui(gameRewardItemFly);
    }
    //道具飞行奖励
    public void game_propsfly()
    {
        var _prop_rt = find_component<RectTransform>("Panel/Game_PropsFly");
        gamePropsFly = create_ui<GamePropsFly>("Game/Game_PropsFly", _prop_rt).Init(this);
        gamePropsFly.StartFly();
    }

    private void levelwin()
    {
        if (leftCell <= 0)
            LevelWinDelay();
    }
    async void LevelWinDelay()
    {
        _ui_manager.OpenWindow<MaskUI>();
        await Task.Delay(TimeSpan.FromSeconds(1.3));
        _ui_manager.TryCloseWindow<MaskUI>();
        _ui_manager.OpenWindow<LevelwinUI>();
        _ui_manager.TryCloseWindow<GameUI>();
    }
    //道具为0时，进关卡会主动弹ui
    private void outItemJump()
    {
        if (shareDataGlobalConfig._game_outitem_jump <= 0)
        {
            if (levelStorage.LevelCount >= 6)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                    {
                        if (commonStorage.Item_Remove <= 0)
                        {
                            shareDataGlobalConfig._bundle_type_id = 3;
                            _ui_manager.OpenWindow<OutItemRV>();
                            break;
                        }
                    }
                    if (i == 1)
                    {
                        if (commonStorage.Item_Recall <= 0)
                        {
                            shareDataGlobalConfig._bundle_type_id = 4;
                            _ui_manager.OpenWindow<OutItemRV>();
                            break;
                        }
                    }
                    if (i == 2)
                    {
                        if (commonStorage.Item_Bloom <= 0)
                        {
                            shareDataGlobalConfig._bundle_type_id = 5;
                            _ui_manager.OpenWindow<OutItemRV>();
                            break;
                        }
                    }
                }
            }
        }
    }
    //回调match
    private void Match()
    {
        //match成功就去掉一张牌
        play_sound("sound_tile_break");
        //leftCell--;
        leftCell -= 3;
        //LevelType();

        //match的飞花动画
        gameRewardItem.Reward_Item_Show();
        game_rewarditemfly();
        gameGuideUI.Match();
        levelwin();
    }
    //回调collect
    private void Collect()
    {
        gameGuideUI.Guide_FirstStep();
        gameItemGroupUI.ReviveCondition();
        _panel_ui.CollectionUI.isMatchPause = false;
        LevelType_TileChange();
    }
    //win按钮
    private void on_win_clicked()
    {
        _ui_manager.OpenWindow<LevelwinUI>();
        Close();
    }
    //判断关卡的类型
    private void LevelType_TileChange()
    {
        if (level.Type == 1)
            tileRandom.RandomTileChange();
    }

    //加载插屏广告
    private void LoadingInterstitial()
    {
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        if (levelStorage.LevelCount >= globalconfig.Interstitial_UnlockLevel
            && globalconfig.Interstitial_CD_Initial >= globalconfig.Interstitial_CD_Level)
            ADSManager.TriggerADSLoading_Interstitial();
    }
}