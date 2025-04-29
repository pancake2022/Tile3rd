using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSFramework
{
    public class AdsManager : CSFramework.Subsystem
    {
        public enum AdsTaskStatus
        {
            None,
            Loading,
            Loaded,
            Showing,
            Closed,
        }

        public class AdsTask
        {
            public AdsConfigItem ConfigItem;
            public AdsTaskStatus Status;
            public int LoadAttemptTimes;
            public float EscapeTime;
            public Action AdsClosedCallback;
            public Action AdsClickCallback;
            public Action TriggerRewardCallback;

            public void StartLoad ()
            {
                // CSFramework.Logger.Log(string.Format("MaxSDK StartLoad: {0}, {1}", ConfigItem.AdsType, ConfigItem.Key));
                Status = AdsTaskStatus.Loading;
                ++LoadAttemptTimes;
                EscapeTime = 0;

                // if (ConfigItem.AdsType == AdsType.Interstitial)
                //     MaxSdk.LoadInterstitial(ConfigItem.Key);
                // else if (ConfigItem.AdsType == AdsType.RewardedVideo)
                //     MaxSdk.LoadRewardedAd(ConfigItem.Key);
            }

            public void Reset ()
            {
                // CSFramework.Logger.Log($"AdsTask.Reset: {ConfigItem.Key}");
                Status = AdsTaskStatus.None;
                AdsClickCallback = null;
            }

            public void OnClose ()
            {
                // CSFramework.Logger.Log($"AdsTask.OnClose: {ConfigItem.Key}");
                Status = AdsTaskStatus.Closed;
            }

            public void OnClick ()
            {
                // CSFramework.Logger.Log($"AdsTask.OnClick: {ConfigItem.Key}");
                AdsClickCallback?.Invoke();
                AdsClickCallback = null;
            }

            public void TriggerClose ()
            {
                // CSFramework.Logger.Log($"AdsTask.TriggerClose: {ConfigItem.Key}, TriggerReward: {TriggerReward}, AdsClosedCallback: {AdsClosedCallback}");
                Status = AdsTaskStatus.None;
                AdsClosedCallback?.Invoke();
                AdsClosedCallback = null;
            }

            public void OnLoaded ()
            {
                // CSFramework.Logger.Log($"AdsTask.OnLoaded: {ConfigItem.Key}");
                Status = AdsTaskStatus.Loaded;
                LoadAttemptTimes = 0;

                AdsClosedCallback = null;
                AdsClickCallback = null;
                TriggerRewardCallback = null;
            }

            public void OnTriggerReward ()
            {
                // CSFramework.Logger.Log($"AdsTask.OnTriggerReward: {ConfigItem.Key}");
                TriggerRewardCallback?.Invoke();
                TriggerRewardCallback = null;
            }
        }

        protected Dictionary<string, AdsTask> _ads_task_dict;
        protected MaxPluginConfiguration _max_plugin_configuration;

        protected override IEnumerator on_init (params object[] param_list)
        {
            _ads_task_dict = new Dictionary<string, AdsTask>();
            _max_plugin_configuration = Resources.Load<MaxPluginConfiguration>(Environment.MaxPluginConfigurationPath);

            register_callback();

            var deviceId = _main_module.Framework.NativeManager.GetDeviceID();

            // init max sdk
            // MaxSdk.SetSdkKey(_max_plugin_configuration.SDKKey);
            // MaxSdk.SetUserId(deviceId);
            // MaxSdk.InitializeSdk();

            yield return null;
        }

        protected override IEnumerator on_cleanup()
        {
            deregister_callback();
            yield return null;
        }

        protected void register_callback ()
        {
            // MaxSdkCallbacks.OnSdkInitializedEvent += on_max_sdk_initialized;

            // MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += on_interstitial_loaded_event;
            // MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += on_interstitial_failed_event;
            // MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += on_interstitial_displayed_event;
            // MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += on_interstitial_failed_to_display_event;
            // MaxSdkCallbacks.Interstitial.OnAdClickedEvent += on_interstitial_clicked_event;
            // MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += on_interstitial_revenue_paid_event;
            // MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += on_interstitial_hidden_event;

            // MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += on_rewarded_ad_loaded_event;
            // MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += on_rewarded_ad_load_failed_event;
            // MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += on_rewarded_ad_displayed_event;
            // MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += on_rewarded_ad_failed_to_display_event;
            // MaxSdkCallbacks.Rewarded.OnAdClickedEvent += on_rewarded_ad_clicked_event;
            // MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += on_rewarded_ad_revenue_paid_event;
            // MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += on_rewarded_ad_hidden_event;
            // MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += on_rewarded_ad_received_reward_event;
        }

        protected void deregister_callback ()
        {
            // MaxSdkCallbacks.OnSdkInitializedEvent -= on_max_sdk_initialized;

            // MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= on_interstitial_loaded_event;
            // MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= on_interstitial_failed_event;
            // MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= on_interstitial_displayed_event;
            // MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= on_interstitial_failed_to_display_event;
            // MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= on_interstitial_clicked_event;
            // MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= on_interstitial_revenue_paid_event;
            // MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= on_interstitial_hidden_event;

            // MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= on_rewarded_ad_loaded_event;
            // MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= on_rewarded_ad_load_failed_event;
            // MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= on_rewarded_ad_displayed_event;
            // MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= on_rewarded_ad_failed_to_display_event;
            // MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= on_rewarded_ad_clicked_event;
            // MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= on_rewarded_ad_revenue_paid_event;
            // MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= on_rewarded_ad_hidden_event;
            // MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= on_rewarded_ad_received_reward_event;
        }

//         protected void on_max_sdk_initialized (MaxSdkBase.SdkConfiguration configuration)
//         {
//             CSFramework.Logger.Log("MaxSDK Initialized");
//             var ads_platform = AdsPlatform.None;
// #if UNITY_IOS
//             ads_platform = AdsPlatform.iOS;
// #else
//             ads_platform = AdsPlatform.Android;
// #endif

//             foreach (var config_item in _max_plugin_configuration.ConfigItemList)
//             {
//                 if (config_item.AdsPlatform == ads_platform) // only init target platform ads
//                 {
//                     if (config_item.AdsType == AdsType.Interstitial || config_item.AdsType == AdsType.RewardedVideo)
//                     {
//                         _ads_task_dict[config_item.Key] = new AdsTask
//                         {
//                             ConfigItem = config_item,
//                             Status = AdsTaskStatus.None,
//                             LoadAttemptTimes = 0,
//                             EscapeTime = 0.0f,
//                         };
//                     }
//                     else
//                     {
//                         CSFramework.Logger.Warning(string.Format("Not Support AdsType: {0}, Key={1}", config_item.AdsType, config_item.Key));
//                     }
//                 }
//             }
        // }

        protected override void on_tick (float dt)
        {
            foreach (var ads_task in _ads_task_dict.Values)
            {
                if (ads_task.Status == AdsTaskStatus.None)
                {
                    ads_task.EscapeTime += dt;
                    if (ads_task.EscapeTime >= ads_task.LoadAttemptTimes)
                        ads_task.StartLoad();
                }
                else if (ads_task.Status == AdsTaskStatus.Closed)
                {
                    ads_task.TriggerClose();
                }
            }
        }

        protected bool try_find_ads_task (string ad_unit_id, out AdsTask ads_task)
        {
            var result = _ads_task_dict.TryGetValue(ad_unit_id, out ads_task);
            if (!result)
                CSFramework.Logger.Error(string.Format("AdsManager Not Found Ads Task: {0}", ad_unit_id));
            return result;
        }

        #region Max插屏广告
        // protected void on_interstitial_loaded_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Interstitial Loaded: " + ad_unit_id);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnLoaded();
        // }

        // protected void on_interstitial_failed_event(string ad_unit_id, MaxSdk.ErrorInfo error_info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Interstitial Load Failed: " + ad_unit_id + ", Info: " + error_info.Code + ", " + error_info.Message + ", " + error_info.AdLoadFailureInfo);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.Reset();
        // }

        // protected void on_interstitial_displayed_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Interstitial Displayed: " + ad_unit_id);
        //     // if (try_find_ads_task(ad_unit_id, out var ads_task))
        //     //     ads_task.OnClose();
        // }

        // protected void on_interstitial_failed_to_display_event(string ad_unit_id, MaxSdk.ErrorInfo error_info, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Interstitial Failed To Display: " + ad_unit_id + ", Info: " + error_info.Code + ", " + error_info.Message + ", " + error_info.AdLoadFailureInfo);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnClose();
        // }

        // protected void on_interstitial_clicked_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Interstitial Clicked: " + ad_unit_id);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnClick();
        // }

        // protected void on_interstitial_revenue_paid_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Interstitial Revenue Paid: " + ad_unit_id);
        //     // if (try_find_ads_task(ad_unit_id, out var ads_task))
        //     //     ads_task.OnClose();
        // }

        // protected void on_interstitial_hidden_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Interstitial Hidden: " + ad_unit_id);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnClose();
        // }
        // #endregion

        // #region Max 激励视频
        // protected void on_rewarded_ad_loaded_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Loaded: " + ad_unit_id);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnLoaded();
        // }

        // protected void on_rewarded_ad_load_failed_event(string ad_unit_id, MaxSdk.ErrorInfo error_info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Load Failed: " + ad_unit_id + ", Info: " + error_info.Code + ", " + error_info.Message + ", " + error_info.AdLoadFailureInfo);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.Reset();
        // }

        // protected void on_rewarded_ad_displayed_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Displayed: " + ad_unit_id);
        //     // if (try_find_ads_task(ad_unit_id, out var ads_task))
        //     //     ads_task.OnClose();
        // }

        // protected void on_rewarded_ad_failed_to_display_event(string ad_unit_id, MaxSdk.ErrorInfo error_info, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Failed To Display: " + ad_unit_id + ", Info: " + error_info.Code + ", " + error_info.Message + ", " + error_info.AdLoadFailureInfo);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnClose();
        // }

        // protected void on_rewarded_ad_clicked_event(string ad_unit_id, MaxSdk.AdInfo info) 
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Clicked: " + ad_unit_id);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnClick();
        // }

        // protected void on_rewarded_ad_revenue_paid_event(string ad_unit_id, MaxSdk.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Revenue Paid: " + ad_unit_id);
        //     // if (try_find_ads_task(ad_unit_id, out var ads_task))
        //     //     ads_task.OnClose();
        // }

        // protected void on_rewarded_ad_hidden_event(string ad_unit_id, MaxSdkBase.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Hidden: " + ad_unit_id);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnClose();
        // }

        // protected void on_rewarded_ad_received_reward_event(string ad_unit_id, MaxSdk.Reward reward, MaxSdkBase.AdInfo info)
        // {
        //     CSFramework.Logger.Log("MaxSDK Rewarded AD Received Reward: " + ad_unit_id);
        //     if (try_find_ads_task(ad_unit_id, out var ads_task))
        //         ads_task.OnTriggerReward();
        // }
        #endregion

        #region Show Ads
        public void ShowDebugger ()
        {
// #if (DEBUG || DEVELOPMENT_BUILD) && !(UNITY_EDITOR)
//             MaxSdk.ShowMediationDebugger();
// #endif
        }

        public bool IsInterstitialReady ()
        {
            foreach (var ads_task in _ads_task_dict.Values)
            {
                if (ads_task.ConfigItem.AdsType == AdsType.Interstitial && ads_task.Status == AdsTaskStatus.Loaded)
                    return true;
            }
            return false;
        }

        public bool TryShowInterstitial (Action ads_closed_callback, out AdsTask ads_task)
        {
            if (try_show(AdsType.Interstitial, out ads_task))
            {
                ads_task.AdsClosedCallback += ads_closed_callback;
                return true;
            }
            return false;
        }

        public bool IsRewardedVideoReady ()
        {
            foreach (var ads_task in _ads_task_dict.Values)
            {
                if (ads_task.ConfigItem.AdsType == AdsType.RewardedVideo && ads_task.Status == AdsTaskStatus.Loaded)
                    return true;
            }
            return false;
        }

        public bool TryShowRewardedVideo (Action ads_closed_callback, Action trigger_reward_callback, out AdsTask ads_task)
        {
            if (try_show(AdsType.RewardedVideo, out ads_task))
            {
                ads_task.AdsClosedCallback += ads_closed_callback;
                ads_task.TriggerRewardCallback += trigger_reward_callback;
                return true;
            }
            return false;
        }

        protected bool try_show (AdsType ads_type, out AdsTask show_ads_task)
        {
            var loaded_task_list = new List<AdsTask>();
            // foreach (var ads_task in _ads_task_dict.Values)
            // {
            //     if (ads_task.ConfigItem.AdsType == ads_type && 
            //         ads_task.Status == AdsTaskStatus.Loaded)
            //     {
            //         loaded_task_list.Add(ads_task);
            //     }
            // }

            // if (loaded_task_list.Count > 0)
            // {
            //     var ads_task = loaded_task_list[UnityEngine.Random.Range(0, loaded_task_list.Count)];
            //     if (ads_type == AdsType.Interstitial)
            //     {
            //         if (MaxSdk.IsInterstitialReady(ads_task.ConfigItem.Key))
            //         {
            //             ads_task.Status = AdsTaskStatus.Showing;
            //             MaxSdk.ShowInterstitial(ads_task.ConfigItem.Key);
            //             show_ads_task = ads_task;
            //             return true;
            //         }
            //     }
            //     else if (ads_type == AdsType.RewardedVideo)
            //     {
            //         if (MaxSdk.IsRewardedAdReady(ads_task.ConfigItem.Key))
            //         {
            //             ads_task.Status = AdsTaskStatus.Showing;
            //             MaxSdk.ShowRewardedAd(ads_task.ConfigItem.Key);
            //             show_ads_task = ads_task;
            //             return true;
            //         }
            //     }
            // }
            show_ads_task = null;
            return false;
        }
        #endregion
    }
}