// using UnityEngine;
// using System;
// using System.IO;
// using System.Collections;
// using System.Collections.Generic;
// using Newtonsoft.Json;
// using Firebase.Extensions;

// namespace CSFramework
// {
//     public class FirebaseManager : Module<BIManager>
//     {
//         public bool IsAvailable { get; private set; }
//         public Action OnAvailable;
        
//         protected override IEnumerator on_init (params object[] param_list)
//         {
//             CSFramework.Logger.Log("FirebaseManager Start Init");
//             IsAvailable = false;
//             Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => 
//             {
//                 CSFramework.Logger.Log("FirebaseManager ContinueWithOnMainThread: " + task.Result);
//                 if (task.Result == Firebase.DependencyStatus.Available)
//                 {
//                     IsAvailable = true;
//                     var app = Firebase.FirebaseApp.DefaultInstance;
//                     Firebase.Messaging.FirebaseMessaging.TokenReceived += on_token_received;
//                     Firebase.Messaging.FirebaseMessaging.MessageReceived += on_message_received;

//                     Firebase.DynamicLinks.DynamicLinks.DynamicLinkReceived += on_dynamic_link;
                    
//                     if (!string.IsNullOrEmpty(_main_module.Context.FirebaseUserID))
//                     {
//                         Firebase.Analytics.FirebaseAnalytics.SetUserId(_main_module.Context.FirebaseUserID);
//                         Firebase.Crashlytics.Crashlytics.SetUserId(_main_module.Context.FirebaseUserID);
//                     }
//                     CSFramework.Logger.Log("FirebaseManager IsCrashlyticsCollectionEnabled: " + Firebase.Crashlytics.Crashlytics.IsCrashlyticsCollectionEnabled);
//                     OnAvailable?.Invoke();
//                 }
//                 else
//                 {
//                     CSFramework.Logger.Warning(string.Format("Firebase DependencyStatus Not Available: {0}", task.Result));
//                 }
//             });

//             yield return null;
//         }

//         public void SetUserID (string user_id)
//         {
//             if (IsAvailable)
//             {
//                 Firebase.Analytics.FirebaseAnalytics.SetUserId(user_id);
//                 Firebase.Crashlytics.Crashlytics.SetUserId(user_id);
//             }
//         }

//         public void AddTrackEvent (string event_name, Dictionary<string, string> info_dict = null)
//         {
//             if (IsAvailable)
//             {
//                 var obj = new Newtonsoft.Json.Linq.JObject();
//                 if (info_dict != null)
//                 {
//                     foreach (var info in info_dict)
//                         obj[info.Key] = info.Value;
//                 }
//                 Firebase.Analytics.FirebaseAnalytics.LogEvent(event_name, "param", obj.ToString());
//             }
//         }

//         protected void on_token_received (object sender, Firebase.Messaging.TokenReceivedEventArgs token)
//         {

//         }

//         protected void on_message_received (object sender, Firebase.Messaging.MessageReceivedEventArgs e)
//         {

//         }

//         protected void on_dynamic_link (object sender, EventArgs args)
//         {
//             var event_args = args as Firebase.DynamicLinks.ReceivedDynamicLinkEventArgs;
//             var url = event_args.ReceivedDynamicLink.Url;

//             log($"Received dynamic link: {url.OriginalString}");
//             log($"Received dynamic link Host: {url.Host}");
//             log($"Received dynamic link HostType: {url.HostNameType}");
//             log($"Received dynamic link Query: {url.Query}");
//         }
//     }
// }