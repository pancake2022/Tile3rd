using System.Collections.Generic;
using UnityEngine;

namespace CSFramework
{
    public class AssetBundleCache
    {
        private Dictionary<string, AssetBundleData> _dict = new Dictionary<string, AssetBundleData>();

        public void Add (AssetBundle asset_bundle)
        {
            if (asset_bundle)
            {
                var data = new AssetBundleData(asset_bundle);
                _dict.Add(asset_bundle.name, data);
            }
        }

        public AssetBundle Get (string name)
        {
            if (_dict.TryGetValue(name, out var data))
            {
                data.Retain();
                return data.AssetBundle;
            }
            else
            {
                return null;
            }
        }

        public bool Exist (string name)
        {
            return _dict.ContainsKey(name);
        }

        public void Release (string name)
        {
            if (_dict.TryGetValue(name, out var data))
            {
                data.Release();
                if (data.ReferenceCount <= 0)
                {
                    data.AssetBundle.Unload(false);
                    _dict.Remove(name);
                }
            }
        }

        public void Clear ()
        {
            foreach (var data in _dict.Values)
            {
                data.AssetBundle.Unload(false);
            }
            _dict.Clear();
        }
    }
}