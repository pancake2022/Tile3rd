using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Newtonsoft.Json;

namespace CSFramework
{
    public class ResourcesManager : Module<Framework>
    {
        public bool LoadFromAssetBundle { get; private set; } = false;

        private VersionManager _version_manager;
        private const int _max_dependency_depth = 6;
        private AssetBundleCache _asset_bundle_cache;
        private Dictionary<string, SpriteAtlas> _sprite_atlas_cache;
        private Dictionary<string, string> _atlas_name_to_directory_path_dict;

        protected override IEnumerator on_init (params object[] param_list) 
        {
            _version_manager = _main_module.VersionManager;
#if UNITY_EDITOR
            LoadFromAssetBundle = _version_manager.LocalVersionFile.LoadFromAssetBundle;
#else
            LoadFromAssetBundle = true;
#endif
            AssetBundle.SetAssetBundleDecryptKey(_version_manager.LocalVersionFile.AssetBundlePW);

            // create cache
            _asset_bundle_cache = new AssetBundleCache();
            _sprite_atlas_cache = new Dictionary<string, SpriteAtlas>();
            _atlas_name_to_directory_path_dict = new Dictionary<string, string>();
            LoadSpriteAtlasConfig();
            SpriteAtlasManager.atlasRequested += on_atlas_requested;
            yield return null;
        }

        protected override IEnumerator on_cleanup ()
        {
            SpriteAtlasManager.atlasRequested -= on_atlas_requested;
            yield return null;
        }

        private void on_atlas_requested (string atlas_name, Action<SpriteAtlas> callback)
        {
            callback?.Invoke(LoadSpriteAtlas(atlas_name));
        }

        public void ClearSpriteAtlasConfig ()
        {
            _atlas_name_to_directory_path_dict.Clear();
        }

        public void LoadSpriteAtlasConfig ()
        {
            var sprite_atlas_config = LoadResource<SpriteAtlasConfig>(_main_module.VersionManager.LocalVersionFile.SpriteAtlasConfigPath);
            foreach (var item in sprite_atlas_config.ItemList)
                _atlas_name_to_directory_path_dict[item.AtlasName] = item.DirectoryPath;
        }

        public T LoadResource<T> (string name, bool use_cache = false) where T : UnityEngine.Object
        {
            try
            {
                if (LoadFromAssetBundle)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        var normalize_path = Utils.NormalizePath(name.ToLower());
                        return load_resource_from_asset_bundle<T>(normalize_path);
                    }
                }
                else
                {
                    var possible_path_list = generate_possible_path_list<T>(name);
                    foreach (var possible_path in possible_path_list)
                    {
                        var obj = load_resource<T>(Utils.NormalizePath(possible_path));
                        if (obj)
                            return obj;
                    }
                    Logger.Warning("LoadResource failed: " + name);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        public SpriteAtlas LoadSpriteAtlas (string name)
        {
            if (!_sprite_atlas_cache.TryGetValue(name, out var atlas))
            {
                if (_atlas_name_to_directory_path_dict.TryGetValue(name, out var path))
                {
                    atlas = LoadResource<SpriteAtlas>(string.Format("{0}/{1}", path, name));
                    if (atlas != null)
                        _sprite_atlas_cache[name] = atlas;
                }
                else
                {
                    CSFramework.Logger.Error(string.Format("LoadSpriteAtlas Error, not found SpriteAtlas path: {0}", name));
                }
            }
            return atlas;
        }

        private T load_resource_from_asset_bundle<T> (string path) where T : UnityEngine.Object
        {
            var asset_bundle_path = string.Format("{0}.ab", Path.GetDirectoryName(path));
            var asset_bundle = load_asset_bundle(asset_bundle_path);
            if (asset_bundle)
            {
                var asset_file_name = Path.GetFileName(path);
                var obj = asset_bundle.LoadAsset<T>(asset_file_name);
                if (obj == null)
                    Logger.Error(string.Format("load_resource not found resource[{0}] from asset bundle: {1}", asset_file_name, path));
                return obj;
            }
            else
            {
                Logger.Error(string.Format("load_resource not found asset bundle: {0}", path));
                return null;
            }
        }

        private T load_resource<T> (string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            path = Utils.GetEditorExtraResourcesPath(path);
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (obj)
                return obj as T;
#endif
            return null;
        }

        private AssetBundle load_asset_bundle (string path, int stack_size = 0)
        {
            if (stack_size > _max_dependency_depth)
            {
                Logger.Error(string.Format("load_asset_bundle [{0}] stack size overflow", path));
                return null;
            }

            var asset_bundle = _asset_bundle_cache.Get(path);

            if (!asset_bundle)
            {
                var asset_bundle_info = _main_module.VersionManager.LocalVersionData.FindAssetBundleInfo(path);
                if (asset_bundle_info == null)
                    warning($"Not Found AssetBundleInfo: {path}");

                var download_path = path.ToDownloadPath();
                if (File.Exists(download_path))
                {
                    asset_bundle = AssetBundle.LoadFromFile(download_path);
                }
                else
                {
                    var package_path = path.ToPackagePath();
                    asset_bundle = AssetBundle.LoadFromFile(package_path);
                }

                if (asset_bundle)
                {
                    _asset_bundle_cache.Add(asset_bundle);

                    // load dependencies
                    var dependency_name_list = asset_bundle_info != null ? asset_bundle_info.DependencyNameList : null;
                    if (dependency_name_list != null && dependency_name_list.Length > 0)
                    {
                        ++stack_size;
                        foreach (var dependency_name in dependency_name_list)
                            load_asset_bundle(dependency_name);
                    }
                }
            }

            return asset_bundle;
        }

        private List<string> generate_possible_path_list<T> (string name) where T : UnityEngine.Object
        {
            var possible_path_list = new List<string>();
            if (typeof(T) == typeof(Sprite))
            {
                possible_path_list.Add(name + ".png");
                possible_path_list.Add(name + ".jpg");
            }
            else if (typeof(T) == typeof(Texture2D))
            {
                possible_path_list.Add(name + ".png");
                possible_path_list.Add(name + ".jpg");
            }
            else if (typeof(T) == typeof(GameObject))
            {
                possible_path_list.Add(name + ".prefab");
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                possible_path_list.Add(name + ".mp3");
                possible_path_list.Add(name + ".wav");
            }
            else if (typeof(T) == typeof(Material))
            {
                possible_path_list.Add(name + ".mat");
            }
            else if (typeof(T) == typeof(TextAsset))
            {
                possible_path_list.Add(name + ".txt");
                possible_path_list.Add(name + ".json");
                possible_path_list.Add(name + ".xml");
                possible_path_list.Add(name + ".bytes");
            }
            else if (typeof(T) == typeof(SpriteAtlas))
            {
                possible_path_list.Add(name + ".spriteatlas");
            }
            else if (typeof(T) == typeof(Material))
            {
                possible_path_list.Add(name + ".mat");
            }
            else if (typeof(T) == typeof(RuntimeAnimatorController))
            {
                possible_path_list.Add(name + ".controller");
            }
            else
            {
                possible_path_list.Add(name + ".asset");
            }
            return possible_path_list;
        }

        public void ReleaseAssetBundleCache (string asset_bundle_name)
        {
            _asset_bundle_cache.Release(asset_bundle_name);
        }

        public void ClearAssetBundleCache ()
        {
            _asset_bundle_cache.Clear();
        }

        #region Editor
#if UNITY_EDITOR
        public void InitInEditor ()
        {
            LoadFromAssetBundle = false;
        }
#endif
        #endregion
    }
}
