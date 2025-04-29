// using UnityEngine;
// using System;
// using System.IO;
// using System.Collections;
// using System.Collections.Generic;
// using Newtonsoft.Json;
// using com.adjust.sdk;

// namespace CSFramework
// {
//     public class AdjustManager : Module<BIManager>
//     {
// #if UNITY_EDITOR
//         public string AdjustID => "";
// #else
//         public string AdjustID => Adjust.getAdid();
// #endif

// #if UNITY_IOS
//         public string Idfa => Adjust.getIdfa();
// #else
//         public string Idfa => "";
// #endif

//         public string InviteCode { get; private set; }
//         public AdjustAttribution Data { get; private set; }

//         private AdjustPluginConfiguration _adjust_configuration;
//         private Dictionary<string, string> _event_token_dict;
//         protected override IEnumerator on_init (params object[] param_list)
//         {
//             _adjust_configuration = Resources.Load<AdjustPluginConfiguration>(Environment.AdjustPluginConfigurationPath);
//             _event_token_dict = new Dictionary<string, string>();
//             if (_adjust_configuration)
//             {
//                 foreach (var event_config in _adjust_configuration.EventConfigList)
//                     _event_token_dict[event_config.Key] = event_config.Token;
//             }

// #if DEBUG
//             var adjustConfig = new AdjustConfig(_adjust_configuration.AppToken, AdjustEnvironment.Sandbox);
// #else
//             var adjustConfig = new AdjustConfig(_adjust_configuration.AppToken, AdjustEnvironment.Production);
// #endif

//             adjustConfig.setLogLevel(AdjustLogLevel.Info);
//             adjustConfig.setSendInBackground(false);
//             adjustConfig.setEventBufferingEnabled(true);
//             adjustConfig.setLaunchDeferredDeeplink(true);
//             adjustConfig.setLogDelegate(msg => CSFramework.Logger.Log(msg));
//             adjustConfig.setEventSuccessDelegate(on_event_success);
//             adjustConfig.setEventFailureDelegate(on_event_failure);
//             adjustConfig.setSessionSuccessDelegate(on_session_success);
//             adjustConfig.setSessionFailureDelegate(on_session_failure);
//             adjustConfig.setDeferredDeeplinkDelegate(on_deferred_deeplink);
//             adjustConfig.setAttributionChangedDelegate(on_attribution_changed);
//             adjustConfig.setDelayStart(10);
//             Adjust.start(adjustConfig);

//             yield return null;
//         }

//         public void SetPartnerParameter(string key, string value)
//         {
// #if !UNITY_EDITOR
//             Adjust.addSessionPartnerParameter(key, value);
// #endif
//         }

//         public void SetCallbackParameter(string key, string value)
//         {
// #if !UNITY_EDITOR
//             Adjust.addSessionCallbackParameter(key, value);
// #endif
//         }

//         public void SetDeviceToken (string token)
//         {
// #if !UNITY_EDITOR
//             Adjust.setDeviceToken(token);
// #endif
//         }

//         public void AddTrackEvent (string event_name, Dictionary<string, string> info_dict = null)
//         {
//             if (!string.IsNullOrEmpty(event_name) && _event_token_dict.TryGetValue(event_name, out var event_token))
//             {
//                 var adjust_event = new AdjustEvent(event_token);
//                 event_add_info(adjust_event, info_dict);
//                 Adjust.trackEvent(adjust_event);
//             }
//             else
//             {
//                 CSFramework.Logger.Warning("AddTrackEvent Warning: Not Contains Event: " + event_name);
//             }
//         }

//         public void AddPurchaseTrackEvent (string event_name, double price, string currency, string transaction_id, bool is_test, Dictionary<string, string> info_dict)
//         {
//             if (!string.IsNullOrEmpty(event_name) && _event_token_dict.TryGetValue(event_name, out var event_token))
//             {
//                 var adjust_event = new AdjustEvent(event_token);
//                 event_add_info(adjust_event, info_dict);

// #if DEBUG
//                 adjust_event.setRevenue(price, currency);
//                 adjust_event.setTransactionId(transaction_id);
// #else
//                 if (!is_test)
//                 {
//                     adjust_event.setRevenue(price, currency);
//                     adjust_event.setTransactionId(transaction_id);
//                 }
// #endif
//                 Adjust.trackEvent(adjust_event);
//             }
//             else
//             {
//                 CSFramework.Logger.Warning("AddPurchaseTrackEvent Warning: Not Contains Event: " + event_name);
//             }
//         }

//         public void AddLoginTrackEvent (string event_name)
//         {
//             if (_event_token_dict.TryGetValue(event_name, out var event_token))
//             {
//                 var adjust_event = new AdjustEvent(event_token);
//                 Adjust.trackEvent(adjust_event);
//             }
//         }

//         protected void event_add_info (AdjustEvent adjust_event, Dictionary<string, string> info_dict)
//         {
//             if (info_dict != null)
//             {
//                 foreach (var info in info_dict)
//                 {
//                     if (!string.IsNullOrEmpty(info.Key) && !string.IsNullOrEmpty(info.Value))
//                         adjust_event.addPartnerParameter(info.Key, info.Value);
//                 }
//             }
//         }

//         protected void on_event_success(AdjustEventSuccess data)
//         {
//             CSFramework.Logger.Log(string.Format("Event tracked successfully!"));

//             if (data.Message != null)
//                 CSFramework.Logger.Log(string.Format("Message: {0}", data.Message));
//             if (data.Timestamp != null)
//                 CSFramework.Logger.Log(string.Format("Timestamp: {0}", data.Timestamp));
//             if (data.Adid != null)
//                 CSFramework.Logger.Log(string.Format("Adid: {0}", data.Adid));
//             if (data.EventToken != null)
//                 CSFramework.Logger.Log(string.Format("EventToken: {0}", data.EventToken));
//             if (data.CallbackId != null)
//                 CSFramework.Logger.Log(string.Format("CallbackId: {0}", data.CallbackId));
//             if (data.JsonResponse != null)
//                 CSFramework.Logger.Log(string.Format("JsonResponse: {0}", data.GetJsonResponse()));
//         }

//         protected void on_event_failure(AdjustEventFailure data)
//         {
//             CSFramework.Logger.Log(string.Format("Event tracking failed!"));

//             if (data.Message != null)
//                 CSFramework.Logger.Log(string.Format("Message: {0}", data.Message));
//             if (data.Timestamp != null)
//                 CSFramework.Logger.Log(string.Format("Timestamp: {0}", data.Timestamp));
//             if (data.Adid != null)
//                 CSFramework.Logger.Log(string.Format("Adid: {0}", data.Adid));
//             if (data.EventToken != null)
//                 CSFramework.Logger.Log(string.Format("EventToken: {0}", data.EventToken));
//             if (data.CallbackId != null)
//                 CSFramework.Logger.Log(string.Format("CallbackId: {0}", data.CallbackId));
//             if (data.JsonResponse != null)
//                 CSFramework.Logger.Log(string.Format("JsonResponse: {0}", data.GetJsonResponse()));

//             CSFramework.Logger.Log(string.Format("WillRetry: {0}", data.WillRetry.ToString()));
//         }

//         protected void on_session_success(AdjustSessionSuccess data)
//         {
//             CSFramework.Logger.Log(string.Format("Session tracked successfully!"));

//             if (data.Message != null)
//                 CSFramework.Logger.Log(string.Format("Message: {0}", data.Message));
//             if (data.Timestamp != null)
//                 CSFramework.Logger.Log(string.Format("Timestamp: {0}", data.Timestamp));
//             if (data.Adid != null)
//                 CSFramework.Logger.Log(string.Format("Adid: {0}", data.Adid));
//             if (data.JsonResponse != null)
//                 CSFramework.Logger.Log(string.Format("JsonResponse: {0}", data.GetJsonResponse()));
//         }

//         protected void on_session_failure(AdjustSessionFailure data)
//         {
//             CSFramework.Logger.Log(string.Format("Session tracking failed!"));

//             if (data.Message != null)
//                 CSFramework.Logger.Log(string.Format("Message: {0}", data.Message));
//             if (data.Timestamp != null)
//                 CSFramework.Logger.Log(string.Format("Timestamp: {0}", data.Timestamp));
//             if (data.Adid != null)
//                 CSFramework.Logger.Log(string.Format("Adid: {0}", data.Adid));
//             if (data.JsonResponse != null)
//                 CSFramework.Logger.Log(string.Format("JsonResponse: {0}", data.GetJsonResponse()));

//             CSFramework.Logger.Log(string.Format("WillRetry: {0}", data.WillRetry.ToString()));
//         }

//         protected void on_deferred_deeplink(string url)
//         {
//             CSFramework.Logger.Log(string.Format("Deeplink URL: {0}", url));
//         }

//         protected void on_attribution_changed(AdjustAttribution data)
//         {
//             CSFramework.Logger.Log(string.Format("Attribution changed!"));
//             Data = data;

//             if (data.trackerName != null)
//                 CSFramework.Logger.Log(string.Format("Tracker name: {0}", data.trackerName));
//             if (data.trackerToken != null)
//                 CSFramework.Logger.Log(string.Format("Tracker token: {0}", data.trackerToken));
//             if (data.network != null)
//                 CSFramework.Logger.Log(string.Format("Network: {0}", data.network));
//             if (data.campaign != null)
//                 CSFramework.Logger.Log(string.Format("Campaign: {0}", data.campaign));
//             if (data.adgroup != null)
//                 CSFramework.Logger.Log(string.Format("Adgroup: {0}", data.adgroup));
//             if (data.creative != null)
//                 CSFramework.Logger.Log(string.Format("Creative: {0}", data.creative));
//             if (data.clickLabel != null)
//                 CSFramework.Logger.Log(string.Format("Click label: {0}", data.clickLabel));
//             if (data.adid != null)
//                 CSFramework.Logger.Log(string.Format("ADID: {0}", data.adid));

//             if (data.trackerName == "Invite" && !string.IsNullOrEmpty(data.clickLabel))
//                 InviteCode = data.clickLabel;
//         }
//     }
// }