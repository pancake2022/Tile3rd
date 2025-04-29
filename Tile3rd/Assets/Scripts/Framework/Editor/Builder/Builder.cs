using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework.Editor
{
    public static class Builder
    {
        [MenuItem("CSFramework/Builder/CreateBuildConfiguration")]
        public static void CreateBuildConfiguration ()
        {
            EditorUtils.CreateAsset<BuildConfiguration>(Environment.BuildConfigurationPath);
        }

        [MenuItem("CSFramework/Builder/BuildPretreatment")]
        public static void BuildPretreatment ()
        {
            AssetBundleEditor.BuildAssetBundle();
        }

        [MenuItem("CSFramework/Builder/Build iOS Debug")]
        public static void BuildIOSDebug ()
        {
            Build(BuildTarget.iOS, true);
        }

        [MenuItem("CSFramework/Builder/Build iOS Release")]
        public static void BuildIOSRelease ()
        {
            Build(BuildTarget.iOS, false);
        }

        [MenuItem("CSFramework/Builder/Build Android Debug")]
        public static void BuildAndroidDebug ()
        {
            Build(BuildTarget.Android, true);
        }

        [MenuItem("CSFramework/Builder/Build Android Release APK")]
        public static void BuildAndroidRelease ()
        {
            Build(BuildTarget.Android, false);
        }

        [MenuItem("CSFramework/Builder/Build Android Release AAB")]
        public static void BuildAndroidReleaseAAB ()
        {
            Build(BuildTarget.Android, false, true);
        }

        public static void Build (BuildTarget target, bool is_debug = false, bool build_app_bundle = false)
        {
            var scene_path_list = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
                scene_path_list.Add(scene.path);

            var build_player_options = new BuildPlayerOptions();
            build_player_options.scenes = scene_path_list.ToArray();

            PlayerSettings.SplashScreen.showUnityLogo = false;

            var build_config = Resources.Load<BuildConfiguration>(Environment.BuildConfigurationPath);
            switch (target)
            {
                case BuildTarget.Android:
                {
                    EditorUserBuildSettings.androidCreateSymbolsZip = !is_debug;
                    // if (!is_debug)
                    {
                        PlayerSettings.Android.useCustomKeystore = true;

                        if (build_config.AndroidKeyStoreUseConfiguration)
                        {
                            PlayerSettings.Android.keystoreName = build_config.AndroidKeyStorePath;
                            PlayerSettings.Android.keystorePass = build_config.AndroidKeyStorePass;
                            PlayerSettings.Android.keyaliasName = build_config.AndroidKeyStoreAlias;
                            PlayerSettings.Android.keyaliasPass = build_config.AndroidKeyStoreAliasPass;
                        }
                        else
                        {
                            PlayerSettings.Android.keystoreName = System.Environment.CurrentDirectory + "/BuildTools/tilecountrylife.keystore";
                            PlayerSettings.Android.keystorePass = "321678";
                            PlayerSettings.Android.keyaliasName = "tilecountrylife";
                            PlayerSettings.Android.keyaliasPass = "321678";
                        }
                    }
                    EditorUserBuildSettings.buildAppBundle = build_app_bundle;
                    break;
                }
                case BuildTarget.iOS:
                {
                    PlayerSettings.iOS.appleEnableAutomaticSigning = true;
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }

            // Facebook.Unity.Settings.FacebookSettings.SelectedAppIndex = is_debug ? build_config.FB_SelectedAppIndex_Debug : build_config.FB_SelectedAppIndex_Release;

            var platform = target.ToString();
            var platform_folder = Path.GetFullPath(Application.dataPath + "/../" + platform + "/build/");
            Utils.MakeSureDirectoryExist(platform_folder);

            if (target == BuildTarget.Android)
            {
                var suffix = build_app_bundle ? "aab" : "apk";
                build_player_options.locationPathName = string.Format("{0}{1}.{2}", platform_folder, PlayerSettings.productName, suffix);
            }
            else
            {
                build_player_options.locationPathName = platform_folder;
            }

            build_player_options.target = target;
            if (is_debug)
                build_player_options.options |= BuildOptions.Development;

            var result = BuildPipeline.BuildPlayer(build_player_options);
            CSFramework.Logger.Log(string.Format("Builder Result: ", result.summary.result));

            string path = platform_folder;
            if (Directory.Exists(path))
                System.Diagnostics.Process.Start(path);
        }
    }
}