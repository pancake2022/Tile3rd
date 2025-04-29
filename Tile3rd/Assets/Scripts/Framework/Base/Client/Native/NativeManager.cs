using UnityEngine;
using CSFramework.Native;
using System;
using System.Collections;

namespace CSFramework
{
    public class NativeManager : Module<Framework>
    {
        private readonly string advertiser_tracking_enabled_key = "AdvertiserTrackingEnabled";
        private static string _adid;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void on_runtime_loaded()
        {
            CSFramework.Logger.Log("NativeManager.on_runtime_loaded");
            Application.RequestAdvertisingIdentifierAsync(request_adid_callback);
        }

        private static void request_adid_callback (string adid, bool success, string error)
        {
            if (success)
            {
                _adid = adid;
                CSFramework.Logger.Log($"NativeManager.request_adid_callback success: {_adid}");
            }
            else
            {
                CSFramework.Logger.Warning($"NativeManager.request_adid_callback failed: {error}");
            }
        }

        public Action<DeviceOrientation, Rect> OnScreenChanged; // <Orientation, SafeArea>
        private DeviceOrientation _device_orientation;

        protected override IEnumerator on_init(params object[] param_list)
        {
            _device_orientation = Input.deviceOrientation;
            // check ads tracking
            CheckAdsTracking();
            // check advertiser tracking enabled
            try
            {
                if (NativeBridge.IsIOSSAvailable14() && !PlayerPrefs.HasKey(advertiser_tracking_enabled_key))
                {
                    NativeBridge.SetAdvertiserTrackingEnabled(true);
                    PlayerPrefs.SetInt(advertiser_tracking_enabled_key, 1);
                }
            }
            catch (Exception e)
            {
                error(e);
            }
            // register DeepLink
            ImaginationOverflow.UniversalDeepLinking.DeepLinkManager.Instance.LinkActivated += on_deep_link_active;
            return null;
        }

        protected override void on_tick(float dt)
        {
            if (_device_orientation != Input.deviceOrientation)
            {
                _device_orientation = Input.deviceOrientation;
                OnScreenChanged?.Invoke(Input.deviceOrientation, Screen.safeArea);
            }
        }

        private void on_deep_link_active (ImaginationOverflow.UniversalDeepLinking.LinkActivation data)
        {
            log($"DeepLink: Uri = {data.Uri}");
            log($"DeepLink: QueryString = {data.QueryString}");
            log($"DeepLink: RawQueryString = {data.RawQueryString}");
            
            if (!string.IsNullOrEmpty(data.RawQueryString))
            {
                // todo notify
            }
        }

        public string GetADID ()
        {
            return _adid;
        }

        public static string GetDeviceID (string project_name)
        {
            var device_id_key = string.Format("{0}_DeviceID", project_name);
            var device_id = PlayerPrefs.GetString(device_id_key, "");
            if (string.IsNullOrEmpty(device_id))
            {
#if UNITY_IOS && !UNITY_EDITOR
                var key = $"{Application.identifier}_DeviceID";
                device_id = NativeBridge.GetKeyChainData(key);
                if (string.IsNullOrEmpty(device_id))
                {
                    device_id = SystemInfo.deviceUniqueIdentifier;
                    NativeBridge.SetKeyChainData(key, device_id);
                }
#else
                device_id = SystemInfo.deviceUniqueIdentifier;
                PlayerPrefs.SetString(device_id_key, device_id);
#endif
            }
            return device_id;
        }

        public string GetDeviceID ()
        {
            return GetDeviceID(_main_module.Context.ProjectName);
        }


        public bool IsFullScreenIOS()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var generation = UnityEngine.iOS.Device.generation;
            if (generation == UnityEngine.iOS.DeviceGeneration.iPhoneX ||
                generation == UnityEngine.iOS.DeviceGeneration.iPhoneXR ||
                generation == UnityEngine.iOS.DeviceGeneration.iPhoneXS ||
                generation == UnityEngine.iOS.DeviceGeneration.iPhoneXSMax ||
                generation == UnityEngine.iOS.DeviceGeneration.iPhone11 ||
                generation == UnityEngine.iOS.DeviceGeneration.iPhone11Pro ||
                generation == UnityEngine.iOS.DeviceGeneration.iPhone11ProMax ||
                generation.ToString().Contains("iPhone12") ||
                (generation == UnityEngine.iOS.DeviceGeneration.iPhoneUnknown && GetScreenWHRate() >= 2.0f))
            {
                return true;
            }
#endif
            return false;
        }

        public static float GetScreenWHRate()
        {
            return (float)Screen.width / Screen.height;
        }

        public bool IsAppInstalled (string package_name)
        {
            return NativeBridge.IsAppInstalled(package_name);
        }

        // public bool IsFacebookInstalled ()
        // {
        //     return NativeBridge.IsFacebookInstalled();
        // }

        public bool IsNotificationEnabled ()
        {
            return NativeBridge.IsNotificationEnabled();
        }

        public bool IsNeedTrackingAuthorization ()
        {
            return NativeBridge.IsNeedTrackingAuthorization();
        }

        public int GetATTStatus ()
        {
            return NativeBridge.GetATTStatus();
        }

        public void RequestTrackingAuthorization (String game_object_name)
        {
            NativeBridge.RequestTrackingAuthorization(game_object_name);
        }

        public int GetAndroidSDKVersion ()
        {
            return NativeBridge.GetAndroidSDKVersion();
        }

        public string GetVersionName ()
        {
            return NativeBridge.GetVersionName();
        }

        public int GetVersionCode ()
        {
            return NativeBridge.GetVersionCode();
        }

        public String GetDeviceType ()
        {
            return NativeBridge.GetDeviceType();
        }

        public String GetIMEI ()
        {
            return NativeBridge.GetIMEI();
        }

        public String GetAndroidID ()
        {
            return NativeBridge.GetAndroidID();
        }

        public String GetMacAddress ()
        {
            return NativeBridge.GetMacAddress();
        }

        public ScreenOrientation GetScreenOrientation ()
        {
            return NativeBridge.GetScreenOrientation();
        }

        public void Copy (String text)
        {
            NativeBridge.Copy(text);
        }

        public String Paste ()
        {
            return NativeBridge.Paste();
        }

        public void OpenSetting ()
        {
            NativeBridge.OpenSetting();
        }

        public void OpenAppStoreRate (String rate_url)
        {
            NativeBridge.OpenAppStoreRate(rate_url);
        }

        public void OpenRateUs (String url, String title, String message, String rate_button_text, String later_button_text, String no_button_text, String game_object_name)
        {
            NativeBridge.OpenRateUs(url, title, message, rate_button_text, later_button_text, no_button_text, game_object_name);
        }

        public void OpenPrivacy (String title, String message, String link_title, String link_url, String agree, String cancel, String game_object_name)
        {
            NativeBridge.OpenPrivacy(title, message, link_title, link_url, agree, cancel, game_object_name);
        }

        public String GetKeyChainData (String key)
        {
            return NativeBridge.GetKeyChainData(key);
        }

        public void SetKeyChainData (String key, String data)
        {
            NativeBridge.SetKeyChainData(key, data);
        }

        // public void OpenFacebook (string url, string page_id)
        // {
        //     NativeBridge.OpenFacebook(url, page_id);
        // }

        // public bool FacebookMessageShare (String link_url, String image_url, String page_id, String title = "title", String sub_title = "subTitle", String button_text = "buttonText")
        // {
        //     return NativeBridge.FacebookMessageShare(link_url, image_url, page_id, title, sub_title, button_text);
        // }

        // public bool IsFacebookAccessTokenActive ()
        // {
        //     return NativeBridge.IsFacebookAccessTokenActive();
        // }

        // public bool IsFacebookDataAccessTokenExpired ()
        // {
        //     return NativeBridge.IsFacebookDataAccessTokenExpired();
        // }

        // public void AuthorizeFacebookDataAccessToken (Action<int> callback)
        // {
        //     NativeBridge.AuthorizeFacebookDataAccessToken(callback);
        // }

        public void OpenTwitter (string url, string page_id)
        {
            NativeBridge.OpenTwitter(url, page_id);
        }

        public void OpenInstagram (string url, string page_id)
        {
            NativeBridge.OpenInstagram(url, page_id);
        }

        public void CheckAdsTracking ()
        {
            if (!IsAdsTrackingOpened())
                NativeBridge.RequestTrackingAuthorization("");
        }

        public bool IsAdsTrackingOpened ()
        {
            if (NativeBridge.IsIOSSAvailable14_5())
            {
                var code = NativeBridge.GetATTStatus();
                return code == 3; // todo
            }
            else
            {
                return true;
            }
        }

        public float GetScreenRatio ()
        {
            return Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        }

#if UNITY_EDITOR
        private float _screen_width;
        private float _screen_height;
        private void OnGUI() 
        {
            if (_screen_width != Screen.width || _screen_height != Screen.height)
            {
                _screen_width = Screen.width;
                _screen_height = Screen.height;
                OnScreenChanged?.Invoke(Input.deviceOrientation, Screen.safeArea);
            }
        }
#endif
    }
}