using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ADS_RewardUI : WindowUI
{
    public static new string DefaultPrefabPath = "Reward/UI_Reward_ADS";
    private GameUI game_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        game_ui = _ui_manager.FindWindow<GameUI>();
        RewardInit();
    }
    protected override void on_open()
    {
        StartCoroutine(WaitCheck_close());
    }
    public void RewardInit()
    {
        var commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        if (game_ui != null) 
            game_ui.GameActiveDelay();

        //重置插屏
        globalconfig.Interstitial_CD_Initial = 0;

        ////广告礼包 - 消除
        //if (shareDataGlobalConfig._bundle_type_id == 3)
        //{
        //    commonStorage.Item_Remove = commonStorage.Item_Remove + globalconfig.RV_Reward_Remove;
        //    GameFly();
        //    _ui_manager.TryCloseWindow<OutItemRV>();
        //}
        ////广告礼包 - 回退
        //if (shareDataGlobalConfig._bundle_type_id == 4)
        //{
        //    commonStorage.Item_Recall = commonStorage.Item_Recall + globalconfig.RV_Reward_Recall;
        //    GameFly();
        //    _ui_manager.TryCloseWindow<OutItemRV>();
        //}
        ////广告礼包 - 绽放
        //if (shareDataGlobalConfig._bundle_type_id == 5)
        //{
        //    commonStorage.Item_Bloom = commonStorage.Item_Bloom + globalconfig.RV_Reward_Bloom;
        //    GameFly();
        //    _ui_manager.TryCloseWindow<OutItemRV>();
        //}
        ////复活
        //if (shareDataGlobalConfig._bundle_type_id == 7)
        //{
        //    var revive_ui = _ui_manager.FindWindow<ReviveUI>();
        //    revive_ui.PlayOut();
        //}
        ////道具礼包
        //if (shareDataGlobalConfig._bundle_type_id == 8)
        //{
        //    _ui_manager.OpenWindow<RewardItemUI>();
        //    _ui_manager.TryCloseWindow<BundleItemsUI>();
        //}
        ////绽放礼包
        //if (shareDataGlobalConfig._bundle_type_id == 9)
        //{
        //    var bloom_ui = _ui_manager.FindWindow<BundleBloomUI>();
        //    bloom_ui.GetBloom();
        //    _ui_manager.TryCloseWindow<BundleBloomUI>();
        //}
        //剧情里的猫hint
        if (shareDataGlobalConfig._bundle_type_id == 10)
        {
            var home_ui = _ui_manager.FindWindow<HomeUI>();
            home_ui.makeOver.makeOver_CatImage.Story03Cat();
            home_ui.makeOver.makeOver_Tips.ButtonInit(false);
        }
        //测试
        if (shareDataGlobalConfig._bundle_type_id == 11)
        {
            commonStorage.Flower = commonStorage.Flower + 1000;
        }
        ////winstreak复活
        //if (shareDataGlobalConfig._bundle_type_id == 12)
        //{
        //    var winstreak_ui = _ui_manager.FindWindow<DailyTask_NoticeUI_WinStreak>();
        //    winstreak_ui.PlayOn();
        //}
        //dailytask的找猫
        if (shareDataGlobalConfig._bundle_type_id == 13)
        {
            var findcat_ui = _ui_manager.FindWindow<DailyTask_FindCatUI>();
            findcat_ui.Hint();
        }
    }
    private void GameFly()
    {
        game_ui.game_propsfly();
        game_ui.gameItemGroupUI.ItemRefresh();
        game_ui.gameItemGroupUI.BloomTipsRefresh();
    }
    private IEnumerator WaitCheck_close()
    {
        yield return new WaitForSeconds(0.5f);
        Close();
    }
}
