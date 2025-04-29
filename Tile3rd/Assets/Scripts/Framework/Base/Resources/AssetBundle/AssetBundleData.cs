using UnityEngine;

namespace CSFramework
{
    public class AssetBundleData
    {
        public AssetBundle AssetBundle { get; private set; }
        public int ReferenceCount { get; private set; }

        public AssetBundleData (AssetBundle asset_bundle)
        {
            AssetBundle = asset_bundle;
            ReferenceCount = 0;
        }

        public void Retain ()
        {
            ++ReferenceCount;
        }

        public void Release ()
        {
            --ReferenceCount;
        }
    }
}