using UnityEngine;
using System;
using System.Collections.Generic;

namespace CSFramework
{
    [Serializable]
    public class AssetVersionFile_AssetConfigItem
    {
        public string Path; // 路径
        public bool InPackage = true; // 是否在初始包中

        public bool IsPackAssetBundle = false; // 是否打AssetBundle
        public bool IsPackAllChild = false; // 是否将所有的子目录打AssetBundle

        public bool IsUploadRawResource = false; // 是否上传原始资源
    }

    [Serializable]
    public class AssetVersionFile_AssetConfig
    {
        public List<AssetVersionFile_AssetConfigItem> ItemList = new List<AssetVersionFile_AssetConfigItem>();
    }

    [Serializable]
    public class AssetVersionFile_PlatformConfig
    {
        public string PlatformName = "";
        public string ResourceUrl = "";
        public string Version = "1.0.0";
        public string AssetBundleFlowShellPath = "";
    }

    public class AssetVersionFile : ScriptableObject
    {
        public bool LoadFromAssetBundle = false;
        public List<AssetVersionFile_PlatformConfig> PlatformConfigList;
        public int PlatformIndex = 0;
        public AssetVersionFile_AssetConfig AssetConfig;
        public string AssetBundlePW = "";
        public string SpriteAtlasConfigPath = "";

        public AssetVersionFile_PlatformConfig CurrentPlatformConfig
        {
            get
            {
                if (PlatformConfigList == null)
                    PlatformConfigList = new List<AssetVersionFile_PlatformConfig> { new AssetVersionFile_PlatformConfig() };
                if (PlatformIndex < 0)
                    return PlatformConfigList.FirstElement();
                else if (PlatformIndex >= PlatformConfigList.Count)
                    return PlatformConfigList.LastElement();
                else
                    return PlatformConfigList[PlatformIndex];
            }
        }

        public string ResourceUrl
        {
            get
            {
                return CurrentPlatformConfig.ResourceUrl;
            }
        }

        public string AssetBundleDownloadUrl
        {
            get
            {
                return $"{ResourceUrl}/{Environment.PlatformName}";
            }
        }

        public string Version
        {
            get
            {
                return CurrentPlatformConfig.Version;
            }
        }

        public string LocalVersionFileName
        {
            get
            {
                return $"{Environment.LocalVersionFileName}.{Version}";
            }
        }

        public string RemoteVersionFileName
        {
            get
            {
                return $"{Environment.RemoteVersionFileName}.{Version}";
            }
        }
    }

    public class AssetVersionFileTemporary
    {
        public class ItemTemporary
        {
            public AssetVersionFile_AssetConfigItem Item = null;
            public Dictionary<string, ItemTemporary> ChildDict = new Dictionary<string, ItemTemporary>();

            public ItemTemporary (string path, Dictionary<string, AssetVersionFile_AssetConfigItem> item_dict)
            {
                if (!item_dict.TryGetValue(path, out Item))
                    Item = new AssetVersionFile_AssetConfigItem{Path = path};
            }

            public void EachItem (Action<ItemTemporary> callback)
            {
                callback?.Invoke(this);
                foreach (var child in ChildDict.Values)
                    child.EachItem(callback);
            }
        }

        public AssetVersionFile File;
        public ItemTemporary Root;

        public AssetVersionFileTemporary (AssetVersionFile file)
        {
            File = file;

            var item_dict = new Dictionary<string, AssetVersionFile_AssetConfigItem>();
            foreach (var item in file.AssetConfig.ItemList)
                item_dict[item.Path] = item;

            Root = new ItemTemporary("", item_dict);
            fill(Root, item_dict);
        }

        protected void fill (ItemTemporary temp_item, Dictionary<string, AssetVersionFile_AssetConfigItem> item_dict)
        {
            var extra_resource_path = Utils.GetEditorExtraResourcesPath(temp_item.Item.Path);
            var dir_list = Utils.GetDirectoryList(extra_resource_path, false);
            temp_item.ChildDict.Clear();
            foreach (var dir in dir_list)
            {
                var item_path = Utils.GetRelativeExtraResourcesPath(dir.FullName);
                if (!item_dict.TryGetValue(item_path, out var item))
                    item = new AssetVersionFile_AssetConfigItem{Path = item_path};

                var temp_child_item = new ItemTemporary(item_path, item_dict);
                temp_item.ChildDict[item_path] = temp_child_item;
                fill(temp_child_item, item_dict);
            }
        }

        public void OverwriteFile ()
        {
            File.AssetConfig.ItemList.Clear();
            Root.EachItem(item => 
            {
                File.AssetConfig.ItemList.Add(item.Item);
            });
        }
    }
}