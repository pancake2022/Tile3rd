using Unity.Services.LevelPlay;
using UnityEngine;
using System;
using System.Collections.Generic;
using CSFramework;

// Example for IronSource Unity.
public class ADSManager : WindowUI
{
    private LevelPlayBannerAd bannerAd;
    private LevelPlayInterstitialAd interstitialAd;

    public static event Action OnLoading_Interstitial;
    public static event Action<string> OnShow_Interstitial;
    public static event Action<string> OnShow_Reward;

#if UNITY_ANDROID
    string appKey = "1dba470f5";
    string bannerAdUnitId = "thnfvcsog13bhn08";
    string interstitialAdUnitId = "6mgm5otugj9uc0pv";
#elif UNITY_IPHONE
    string appKey = "8545d445";
    string bannerAdUnitId = "iep3rxsyp9na3rw8";
    string interstitialAdUnitId = "wmgt0712uuux8ju4";
#else
    string appKey = "unexpected_platform";
    string bannerAdUnitId = "unexpected_platform";
    string interstitialAdUnitId = "unexpected_platform";
#endif

    public void Start()
    {
        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();

        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // SDK init
        Debug.Log("unity-script: LevelPlay SDK initialization");
        LevelPlay.Init(appKey,adFormats:new []{com.unity3d.mediation.LevelPlayAdFormat.REWARDED});

        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
    }

    void EnableAds()
    {
        //Add ImpressionSuccess Event
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

        bannerAd = new LevelPlayBannerAd(bannerAdUnitId);

        // Register to Banner events
        bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;

        // Create Interstitial object
        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        // Register to Interstitial events
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }

    void OnApplicationPause(bool isPaused)
    {
        Debug.Log("unity-script: OnApplicationPause = " + isPaused);
        IronSource.Agent.onApplicationPause(isPaused);
    }

    #region Init callback handlers

    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: "+ config);
        EnableAds();
    }

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: "+ error);
    }

    #endregion

    #region AdInfo Rewarded Video
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo);
    }

    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);
    }

    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdAvailable With AdInfo " + adInfo);
    }

    void RewardedVideoOnAdUnavailable()
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdUnavailable");
        //广告加载不成功//或未联网
        //禁用观看广告按钮或提示“检查网络”
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdShowFailedEvent With Error" + ironSourceError + "And AdInfo " + adInfo);
        //奖励广告播放失败//中途退出//切后台等
        //提示“需要看完广告才能领取奖励”
    }

    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
        string placement = ironSourcePlacement.getPlacementName();
        HandleADSRewarded(placement);
        InterstitialInit();
    }

    void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
    }

    #endregion
    #region AdInfo Interstitial

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadFailedEvent With Error " + error);
    }

    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdDisplayedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError)
    {
        Debug.Log("unity-script: I got InterstitialOnAdDisplayFailedEvent With InfoError " + infoError);
    }

    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);
        //插屏播放完成 - 关闭关卡胜利界面/打开home界面
        InterstitialInit();
        CloseLevelWin();
    }

    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdInfoChangedEvent With AdInfo " + adInfo);
    }

    #endregion

    #region Banner AdInfo

    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError);
    }

    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdDisplayedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
    {
        Debug.Log("unity-script: I got BannerOnAdDisplayFailedEvent With AdInfoError " + adInfoError);
    }

    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdCollapsedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo);
    }

    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdExpandedEvent With AdInfo " + adInfo);
    }

    #endregion

    #region ImpressionSuccess callback handler

    void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        Debug.Log("unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
        Debug.Log("unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
    }

    #endregion
    private void OnEnable()
    {
        ADSManager.OnLoading_Interstitial += HandleADSLoading_Interstitial;
        ADSManager.OnShow_Interstitial += HandleADSShow_Interstitial;
        ADSManager.OnShow_Reward += HandleADSShow_Reward;
    }
    private void OnDisable()
    {
        bannerAd?.DestroyAd();
        interstitialAd?.DestroyAd();

        ADSManager.OnLoading_Interstitial -= HandleADSLoading_Interstitial;
        ADSManager.OnShow_Interstitial -= HandleADSShow_Interstitial;
        ADSManager.OnShow_Reward -= HandleADSShow_Reward;
    }

    //trigger - 外部调用并接收placementName
    public static void TriggerADSLoading_Interstitial()
    {
        OnLoading_Interstitial?.Invoke();
    }
    public static void TriggerADSShow_Interstitial(string placementName)
    {
        OnShow_Interstitial?.Invoke(placementName);
    }
    public static void TriggerADSShow_Reward(string placementName)
    {
        OnShow_Reward?.Invoke(placementName);
    }

    //回调 - 插屏广告的读取
    private void HandleADSLoading_Interstitial()
    {
        if (interstitialAd == null)// 广告管理器还没初始化完或广告模块失效
        {
            Debug.Log("unity-script: InterstitialAd is NULL! 请检查广告是否初始化成功");
            return;
        }
        if (!interstitialAd.IsAdReady())
        {
            Debug.Log($"unity-script: 开始加载广告");
            interstitialAd.LoadAd();
        }
        else
        {
            Debug.Log($"unity-script: 广告已准备好，无需重复加载");
        }
    }
    //回调 - 插屏广告的触发
    private void HandleADSShow_Interstitial(string placementName)
    {
        if (interstitialAd == null)// 广告管理器还没初始化完或广告模块失效
        {
            Debug.LogWarning("unity-script: 插屏广告模块未初始化");
            CloseLevelWin();
            return;
        }

        if (interstitialAd.IsAdReady())
        {
            interstitialAd.ShowAd(placementName);
        }
        else
        {
            //关闭关卡结算界面
            Debug.Log($"unity-script: 插屏广告 {placementName} 不可用");
            CloseLevelWin();
        }
    }
    //回调 - 奖励广告的触发
    private void HandleADSShow_Reward(string placementName)
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo(placementName);
        }
        else
        {
            Debug.LogWarning($"unity-script: 奖励广告 {placementName} 不可用");
            _ui_manager.OpenWindow<NoticeUI_Internet>();
        }
    }

    //奖励广告发放奖励
    void HandleADSRewarded(String placement)
    {
        Dictionary<string, Action> rewardActions = new Dictionary<string, Action>
        {
            { "Item_Remove", () => GiveItem("Remove") },
            { "Item_Recall", () => GiveItem("Recall") },
            { "Item_Bloom", () => GiveItem("Bloom") },
            { "Revive_Life", () => HandleRevive("Life") },
            { "Revive_WinStreak", () => HandleRevive("WinStreak") },
            { "Bundle_Bloom", () => HandleBundle("BundleBloom") },
            { "Bundle_Item", () => HandleBundle("BundleItem") },
            { "FindCat_Story", () => HandleFindCat("FindCatStory") },
            { "FindCat_DailyTask", () => HandleFindCat("FindCatDailyTask") },
        };

        // 检查是否有对应的广告奖励逻辑，并执行
        if (rewardActions.TryGetValue(placement, out Action rewardAction))
        {
            rewardAction.Invoke();
        }
        else
        {
            Debug.LogWarning($"unity-script: 未知的广告奖励: {placement}");
        }
    }

    //奖励逻辑 - 复活
    void HandleRevive(string lifeType)
    {
        if (lifeType == "Life")
        {
            var revive_ui = _ui_manager.FindWindow<ReviveUI>();
            revive_ui.PlayOut();
        }
        else if (lifeType == "WinStreak")
        {
            var winstreak_ui = _ui_manager.FindWindow<DailyTask_NoticeUI_WinStreak>();
            winstreak_ui.PlayOn();
        }
        else
        {
            Debug.Log($"unity-script: 非有效复活");
        }
    }
    //奖励逻辑 - 道具
    void GiveItem(string itemType)
    {
        var commonStorage = _ui_manager.Framework.StorageManager.Storage<CommonStorage>();//获取通用存档
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        var game_ui = _ui_manager.FindWindow<GameUI>();
        if (game_ui != null)
            game_ui.GameActiveDelay();
        
        if (itemType == "Remove")
        {
            commonStorage.Item_Remove += globalconfig.RV_Reward_Remove;
            _ui_manager.TryCloseWindow<OutItemRV>();
            GameFly();
        }
        else if (itemType == "Recall")
        {
            commonStorage.Item_Recall += globalconfig.RV_Reward_Recall;
            _ui_manager.TryCloseWindow<OutItemRV>();
            GameFly();
        }
        else if (itemType == "Bloom")
        {
            commonStorage.Item_Bloom += globalconfig.RV_Reward_Bloom;
            _ui_manager.TryCloseWindow<OutItemRV>();
            GameFly();
        }
        else
        {
            Debug.Log($"unity-script: 非有效道具");
        }
    }
    private void GameFly()
    {
        var game_ui = _ui_manager.FindWindow<GameUI>();
        game_ui.game_propsfly();
        game_ui.gameItemGroupUI.ItemRefresh();
        game_ui.gameItemGroupUI.BloomTipsRefresh();
    }
    //奖励逻辑 - 礼包
    void HandleBundle(string bundleType)
    {
        if (bundleType == "BundleBloom")
        {
            var bloom_ui = _ui_manager.FindWindow<BundleBloomUI>();
            bloom_ui.GetBloom();
            _ui_manager.TryCloseWindow<BundleBloomUI>();
        }
        else if (bundleType == "BundleItem")
        {
            _ui_manager.OpenWindow<RewardItemUI>();
            _ui_manager.TryCloseWindow<BundleItemsUI>();
        }
        else
        {
            Debug.Log($"unity-script: 非有效礼包");
        }
    }
    //奖励逻辑 - 找猫
    void HandleFindCat(string findcatType)
    {
        if (findcatType == "FindCatStory")
        {
            var home_ui = _ui_manager.FindWindow<HomeUI>();
            home_ui.makeOver.makeOver_CatImage.Story03Cat();
            home_ui.makeOver.makeOver_Tips.ButtonInit(false);
        }
        else if (findcatType == "FindCatDailyTask")
        {
            var findcat_ui = _ui_manager.FindWindow<DailyTask_FindCatUI>();
            findcat_ui.Hint();
        }
        else
        {
            Debug.Log($"unity-script: 非有效找猫");
        }
    }

    //插屏逻辑
    void InterstitialInit()
    {
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        globalconfig.Interstitial_CD_Initial = 0;
    }
    void CloseLevelWin()
    {
        _ui_manager.OpenWindow<HomeUI>();
        _ui_manager.TryCloseWindow<LevelwinUI>();
    }
}
