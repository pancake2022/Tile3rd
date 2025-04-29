using System.Collections.Generic;

namespace CSFramework
{
    [System.Serializable]
    public class AssetVersionData
    {
        public string Version;
        public string UniqueID;
        public Dictionary<string, AssetBundleInfo> AssetBundleInfoDict;

        public AssetVersionData (string version, string unique_id)
        {
            Version = version;
            UniqueID = unique_id;
            AssetBundleInfoDict = new Dictionary<string, AssetBundleInfo>();
        }

        public AssetBundleInfo FindAssetBundleInfo (string asset_bundle_name)
        {
            if (AssetBundleInfoDict.TryGetValue(asset_bundle_name, out var info))
                return info;
            return null;
        }
    }
}