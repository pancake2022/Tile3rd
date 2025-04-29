using UnityEngine;
using System;
using System.Runtime.InteropServices;
// using Facebook.Unity;
using AOT;

namespace CSFramework.Native
{
    public class NativeBridge
    {
        public static bool IsAppInstalled (string package_name)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS
            return isAppInstalledIOS(package_name);
#elif UNITY_ANDROID
            return aj.CallStatic<bool>("isAppInstalled", package_name);
#else
            return false;      
#endif
        }

//         public static bool IsFacebookInstalled ()
//         {
// #if UNITY_EDITOR
//             return false;
// #elif UNITY_IOS
//             return isFacebookInstalledIOS();
// #elif UNITY_ANDROID
//             return aj.CallStatic<bool>("isFacebookInstalled");
// #else
//             return false;      
// #endif
//         }

        public static bool IsNotificationEnabled ()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS
            return isUserNotificationEnabled();
#elif UNITY_ANDROID
            return aj.CallStatic<bool>("isUserNotificationEnabled");
#else
            return false;      
#endif
        }

        public static bool IsNeedTrackingAuthorization ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return needTrackingAuthorizationIOS();
#else
            return false;
#endif
        }

        // 0: ATTrackingManagerAuthorizationStatusNotDetermined
        // 1: ATTrackingManagerAuthorizationStatusRestricted
        // 2: ATTrackingManagerAuthorizationStatusDenied
        // 3: ATTrackingManagerAuthorizationStatusAuthorized
        public static int GetATTStatus ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return ATTStatusIOS();
#else
            return -1;
#endif
        }

        public static void RequestTrackingAuthorization (String game_object_name)
        {
#if UNITY_IOS && !UNITY_EDITOR
            requestTrackingAuthorizationIOS(game_object_name);
#endif
        }

        public static bool IsIOSSAvailable14 ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return iOSSAvailableFourteen();
#else
            return false;
#endif
        }

        public static bool IsIOSSAvailable14_5 ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return iOSSAvailableFourteenFive();
#else
            return false;
#endif
        }

        public static int GetAndroidSDKVersion ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
#else
            return -1;      
#endif
        }

        public static string GetVersionName ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidVersionName;
#else
            return Application.version;
#endif
        }

        public static int GetVersionCode ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (int.TryParse(getVersionCodeIOS(), out var version_code))
                return version_code;
            return -1;
#elif UNITY_ANDROID && !UNITY_EDITOR
            return AndroidVersionCode;
#else
            return -1;
#endif
        }

        public static String GetDeviceID ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<string>("getDeviceId");
#else
            return "Unknow";
#endif
        }

        public static String GetDeviceType ()
        {
#if UNITY_EDITOR
            return "UnityEditor";
#elif UNITY_ANDROID
            return AndroidDeviceType;
#elif UNITY_IOS
            return SystemInfo.deviceModel.Contains("iPad") ? "tablet" : "phone";
#else
            return "Unknow";
#endif
        }

        public static String GetIMEI ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<string>("getIMEI");
#else
            return "Unknow";
#endif
        }

        public static String GetAndroidID ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<string>("getAndroidID");
#else
            return "Unknow";
#endif
        }

        public static String GetMacAddress ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<string>("getMacAddress");
#else
            return "Unknow";
#endif
        }

        public static ScreenOrientation GetScreenOrientation ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var screenOrientation = getScreenOrientationIOS();
            switch (screenOrientation)
            {
                case 1:
                    return ScreenOrientation.Portrait;
                case 2:
                    return ScreenOrientation.PortraitUpsideDown;
                case 3:
                    return ScreenOrientation.Landscape;
                case 4:
                    return ScreenOrientation.LandscapeRight;
                default:
                    return ScreenOrientation.Unknown;
            }
#else
            return Screen.orientation;
#endif
        }

        public static void Copy (String text)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            var te = new TextEditor();
            te.text = text;
            te.OnFocus();
            te.Copy();
#elif UNITY_IOS
            copy(text);
#elif UNITY_ANDROID
            aj.CallStatic("copy", text);
#endif
        }

        public static String Paste ()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            var te = new TextEditor();
            te.Paste();
            return te.text;
#elif UNITY_IOS
            return paste();
#elif UNITY_ANDROID
            return aj.CallStatic<string>("paste");
#else
            return "";
#endif
        }

        public static void OpenSetting ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            openSetting();
#endif
        }

        public static void OpenAppStoreRate (String rate_url)
        {
#if UNITY_IOS && !UNITY_EDITOR
            openAppStoreRate(rate_url);
#endif
        }

        public static void OpenRateUs (String url, String title, String message, String rate_button_text, String later_button_text, String no_button_text, String game_object_name)
        {
#if UNITY_IOS && !UNITY_EDITOR
            showRateUsViewControllerIOS(url, title, message, rate_button_text, later_button_text, no_button_text, game_object_name);
#endif
        }

        public static void OpenPrivacy (String title, String message, String link_title, String link_url, String agree, String cancel, String game_object_name)
        {
#if UNITY_IOS && !UNITY_EDITOR
            popupPrivacyIOS(title, message, link_title, link_url, agree, cancel, game_object_name);
#elif UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("popupPrivacy", title, message, link_title, link_url, agree, cancel, game_object_name);
#endif
        }

        public static String GetKeyChainData (String key)
        {
#if UNITY_IOS && !UNITY_EDITOR
            return getDataFromKeyChainIOS(key);
#else
            return "";
#endif
        }

        public static void SetKeyChainData (String key, String data)
        {
#if UNITY_IOS && !UNITY_EDITOR
            saveDataToKeyChainIOS(key, data);
#endif
        }

//         public static void OpenFacebook (string url, string page_id)
//         {
// #if UNITY_ANDROID && !UNITY_EDITOR
//             aj.CallStatic("openFacebookPage", url, page_id);
// #endif
//         }

//         public static bool FacebookMessageShare (String link_url, String image_url, String page_id, String title = "title", String sub_title = "subTitle", String button_text = "buttonText")
//         {
// #if UNITY_IOS && !UNITY_EDITOR
//             return FBMessagerShareIOS(link_url, image_url, page_id, title, sub_title, button_text);
// #elif UNITY_ANDROID && !UNITY_EDITOR
//             return aj.CallStatic<bool>("FBMessagerShare", link_url, image_url, page_id, title, sub_title, button_text);
// #else
//             return false;
// #endif
//         }

//         public static bool IsFacebookAccessTokenActive ()
//         {
// #if UNITY_IOS && !UNITY_EDITOR
//             return isFaceBookAccessTokenActiveIOS();
// #elif UNITY_ANDROID && !UNITY_EDITOR
//             return aj.CallStatic<bool>("isFaceBookAccessTokenActive");
// #else
//             return true;
// #endif
//         }

//         public static bool IsFacebookDataAccessTokenExpired ()
//         {
// #if UNITY_IOS && !UNITY_EDITOR
//             return isFacebookDataAccessExpiredIOS();
// #elif UNITY_ANDROID && !UNITY_EDITOR
//             return aj.CallStatic<bool>("isFacebookDataAccessExpired");
// #else
//             return true;
// #endif
//         }

//         public static void AuthorizeFacebookDataAccessToken (Action<int> callback)
//         {
// #if UNITY_IOS && !UNITY_EDITOR
//             _AFDATCallback = callback;
//             reauthorizeFacebookDataAccessIOS(AuthorizeFacebookDataAccessToken_Callback);
// #elif UNITY_ANDROID && !UNITY_EDITOR
//             aj.CallStatic("reauthorizeFacebookDataAccess");
//             // android平台下，重新授权后需要刷新一次accesstoken
//             // 第一次refresh保证eauthorize完成后才执行第二次refresh
//             FB.Mobile.RefreshCurrentAccessToken(result_1 =>
//             {
//                 FB.Mobile.RefreshCurrentAccessToken(result_2 =>
//                 {
//                     if (callback != null)
//                         callback(IsFacebookDataAccessTokenExpired() ? 0 : 1);
//                 });
//             });
// #endif
//         }

        public static void SetAdvertiserTrackingEnabled(bool enabled)
        {
#if UNITY_IOS && !UNITY_EDITOR
        //     FBAdSettingsBridgeSetAdvertiserTrackingEnabled(enabled);
#endif
        }

        public static void OpenTwitter (string url, string page_id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("openTwitterPage", url, page_id);
#endif
        }

        public static void OpenInstagram (string url, string page_id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("openInstagramPage", url, page_id);
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass aj = null;
        private static string AndroidVersionName;
        private static int AndroidVersionCode;
        private static string AndroidDeviceType;
        private static AndroidJavaClass unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        private static AndroidJavaObject activityObj = unityActivityClass.GetStatic<AndroidJavaObject>("currentActivity");

        static NativeBridge ()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null)
                return;

            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            aj = new AndroidJavaClass("com.csframework.NativeBridge");
            aj.CallStatic("initialize", context, activity);

            AndroidVersionName = aj.CallStatic<string>("getVersionName");
            AndroidVersionCode = aj.CallStatic<int>("getVersionCode");
            AndroidDeviceType = aj.CallStatic<string>("getDeviceType");
        }
#elif UNITY_IOS && !UNITY_EDITOR
        private static Action<int> _AFDATCallback = null;
        [MonoPInvokeCallback (typeof(reauthorizeFacebookDataAccessIOSCallBack))]
        private static void AuthorizeFacebookDataAccessToken_Callback()
        {
            if(_AFDATCallback != null)
            {
                _AFDATCallback(isFacebookDataAccessExpiredIOS() ? 0 : 1);
                _AFDATCallback = null;
            }
        }
        [DllImport("__Internal")]
        private static extern bool isFacebookInstalledIOS();
        [DllImport("__Internal")]
        private static extern bool isAppInstalledIOS(string scheme);
        [DllImport("__Internal")]
        private static extern void openSetting();
        [DllImport("__Internal")]
        private static extern void openAppStoreRate(string rateUrl);
        [DllImport("__Internal")]
        private static extern bool isNotificationPermissionOpeniOS();
        [DllImport("__Internal")]
        private static extern void popupPrivacyIOS(string title, string message, string linkTitle, string linkUrl, string agreeTitle, string cancelTitle, string gameObjectName);
        [DllImport("__Internal")]
        private static extern void showRateUsViewControllerIOS(string url, string title, string message, string rateButtonText, string laterButtonText, string noButtonText, string gameObjectName);
        [DllImport("__Internal")]
        private static extern void saveDataToKeyChainIOS(string key, string data);
        [DllImport("__Internal")]
        private static extern string getDataFromKeyChainIOS(string key);
        [DllImport("__Internal")]
        private static extern string getVersionCodeIOS();
        [DllImport("__Internal")]
        private static extern int getScreenOrientationIOS();
        [DllImport("__Internal")]
        private static extern bool FBMessagerShareIOS(string linkUrl, string imageUrl, string pageId, string title, string subTitle, string buttonText);
        [DllImport("__Internal")]
        private static extern bool isFaceBookAccessTokenActiveIOS();
        [DllImport("__Internal")]
        private static extern bool isFacebookDataAccessExpiredIOS();
        [DllImport("__Internal")]
        private static extern bool isUserNotificationEnabled();
        [DllImport("__Internal")]
        private static extern bool needTrackingAuthorizationIOS();
        [DllImport("__Internal")]
        private static extern int ATTStatusIOS();
        [DllImport("__Internal")]
        private static extern void requestTrackingAuthorizationIOS(string game_object_name);
        private delegate void reauthorizeFacebookDataAccessIOSCallBack();
        [DllImport("__Internal")]
        private static extern void reauthorizeFacebookDataAccessIOS(reauthorizeFacebookDataAccessIOSCallBack callback);
        [DllImport("__Internal")]
        private static extern void copy(string text);
        [DllImport("__Internal")]
        private static extern string paste();
        [DllImport("__Internal")]
        private static extern bool iOSSAvailableFourteen();
        [DllImport("__Internal")]
        private static extern bool iOSSAvailableFourteenFive();
        // [DllImport("__Internal")] 
        // private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);
#endif
    }
}