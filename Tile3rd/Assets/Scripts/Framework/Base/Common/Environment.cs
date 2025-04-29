using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSFramework
{
    public static class Environment
    {
#if UNITY_ANDROID
        public readonly static string PlatformName = "Android";
#elif UNITY_IOS
        public readonly static string PlatformName = "iOS";
#else
        public readonly static string PlatformName = "Mac";
#endif
        
        public readonly static string FrameworkPath = "Scripts/Framework";
        public readonly static string GameLogicPath = "Scripts/GameLogic";
        public readonly static string ExtraResourcesPath = "ExtraResources";
        public readonly static string RawResourcesPath = "RawResources";
        public readonly static string LocalVersionFileName = "local_version";
        public readonly static string RemoteVersionFileName = "remote_version";

        public static readonly string AssetBundleExportDirectory = Application.dataPath + "/AssetBundleExport/" + PlatformName;
        public static readonly string StreamingAssetsDirectory = Application.streamingAssetsPath + "/" + PlatformName;
        public static readonly string PersistentDataDirectory = Application.persistentDataPath + "/Download/" + PlatformName;
        
        public static string AssetVersionFilePath = "ProjectConfiguration/AssetVersionFile";
        public static string BuildConfigurationPath = "ProjectConfiguration/BuildConfiguration";
        public static string MaxPluginConfigurationPath = "ProjectConfiguration/MaxPluginConfiguration";
        public static string AdjustPluginConfigurationPath = "ProjectConfiguration/AdjustPluginConfiguration";

#if !UNITY_EDITOR && UNITY_ANDROID
        public static readonly string StreamingAssetsDirectoryWWW = StreamingAssetsDirectory;
#else
        public static readonly string StreamingAssetsDirectoryWWW = "file:///" + StreamingAssetsDirectory;
#endif
    }
}