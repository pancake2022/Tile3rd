using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework.Editor
{
    public static class AssetBundleEditor
    {
        [MenuItem("CSFramework/AssetBundle/StartAssetBundleFlow")]
        public static void StartAssetBundleFlow ()
        {
            BuildAssetBundle();
            ExecuteAssetBundleFlowShell();
        }

        [MenuItem("CSFramework/AssetBundle/ExecuteAssetBundleFlowShell")]    
        public static void ExecuteAssetBundleFlowShell ()
        {
            var asset_version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
            Utils.ExecuteShell(asset_version_file.CurrentPlatformConfig.AssetBundleFlowShellPath);
        }

        public static void SetPlatformIndex ()
        {
            var args = System.Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                for (var i = 0; i < args.Length; ++i)
                {
                    if (args[i].ToLower() == "-platformindex")
                    {
                        var param_index = i + 1;
                        if (param_index < args.Length && int.TryParse(args[param_index], out var platform_index))
                        {
                            var asset_version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
                            asset_version_file.PlatformIndex = platform_index;
                            Logger.Log($"SetPlatformIndex: {platform_index}, CurrentPlatformName = {asset_version_file.CurrentPlatformConfig.PlatformName}");
                            EditorUtility.SetDirty(asset_version_file);
                            AssetDatabase.SaveAssets();
                            return;
                        }
                    }
                }
            }
            Logger.Error($"SetPlatformIndex Error: Invalid Args: {string.Concat(args, ' ')}");
        }

        [MenuItem("CSFramework/AssetBundle/BuildAssetBundle")]
        public static void BuildAssetBundle ()
        {
            try_build_asset_bundle(BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle);
        }

        [MenuItem("CSFramework/AssetBundle/ClearDownloadDirectory")]
        public static void ClearDownloadDirectory ()
        {
            ClearDownloadDirectory(true);
        }

        public static void ClearDownloadDirectory (bool show_dialog)
        {
            string path = Environment.PersistentDataDirectory;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                AssetDatabase.Refresh();
                if (show_dialog)
                    EditorUtility.DisplayDialog("info", "clear download directory success", "ok");
            }
            else
            {
                if (show_dialog)
                    EditorUtility.DisplayDialog("info", "download directory already empty", "ok");
            }
        }

        [MenuItem("CSFramework/AssetBundle/OpenDownloadDirectory")]
        public static void OpenDownloadDirectory ()
        {
            string path = Environment.PersistentDataDirectory;
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                EditorUtility.DisplayDialog("info", "download directory not exist", "ok");
            }
        }

        private static void try_build_asset_bundle (BuildAssetBundleOptions options)
        {
            try
            {
                build_asset_bundle(options);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                EditorUtility.ClearProgressBar();
            }
        }

        private static void build_asset_bundle (BuildAssetBundleOptions options)
        {
            // clear cache
            Caching.ClearCache();
            AssetDatabase.RemoveUnusedAssetBundleNames();

            EditorUtility.DisplayCancelableProgressBar("prepare", "0%", 0f);

            var version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
            
            var version = version_file.Version;
            var unique_id = Guid.NewGuid().ToString(); // generate new version ID
            var remote_version_data = new AssetVersionData(version, unique_id);
            var local_version_data = new AssetVersionData(version, unique_id);
            var export_root = $"{Environment.AssetBundleExportDirectory}/{version}";
            
            if (Directory.Exists(export_root))
                Directory.Delete(export_root, true);
            Directory.CreateDirectory(export_root);
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayCancelableProgressBar("ready build all asset bundle", "10%", 0.1f);
            BuildVersionFile(export_root, remote_version_data, local_version_data, version_file, options);

            EditorUtility.DisplayCancelableProgressBar("save version file", "60%", 0.6f);
            // save remote version data
            Utils.SaveFile(string.Format("{0}/{1}.txt", Directory.GetParent(export_root), version_file.RemoteVersionFileName), JsonConvert.SerializeObject(remote_version_data));
            // save local version data
            Utils.SaveFile(string.Format("{0}/Resources/{1}.txt", Application.dataPath, version_file.LocalVersionFileName), JsonConvert.SerializeObject(local_version_data));

            EditorUtility.DisplayCancelableProgressBar("copy in package resource", "70%", 0.7f);
            var package_resource_root = $"{Environment.StreamingAssetsDirectory}";
            if (Directory.Exists(Application.streamingAssetsPath)) // clear all streaming assets
            {
                Directory.Delete(Application.streamingAssetsPath, true);
                AssetDatabase.Refresh();
            }

            // copy raw resource into export root
            foreach (var item in version_file.AssetConfig.ItemList)
            {
                if (item.IsUploadRawResource)
                {
                    var full_path = Utils.GetEditorExtraResourcesPath(item.Path);
                    var raw_file_list = Utils.GetExtraResourcesList(item.Path, false);
                    var dst_root = string.Format("{0}/{1}", export_root, item.Path.ToLower());

                    foreach (var raw_file in raw_file_list)
                    {
                        var dst_path = $"{dst_root}/{raw_file.Name}";
                        // CSFramework.Logger.Log($"Copy File {raw_file.FullName} => {dst_path}");
                        Utils.CopyFile(raw_file.FullName, dst_path);
                    }
                }
            }
            
            // copy in package resource into streaming directory
            foreach (var item in version_file.AssetConfig.ItemList)
            {
                if (item.IsPackAssetBundle && item.InPackage)
                {
                    var path_src = string.Format("{0}/{1}.ab", export_root, item.Path.ToLower());
                    var path_dst = string.Format("{0}/{1}.ab", package_resource_root, item.Path.ToLower());
                    Utils.CopyFile(path_src, path_dst);
                }
            };
            
            // refresh
            EditorUtility.DisplayCancelableProgressBar("finished", "100%", 1f);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        private static void BuildVersionFile (string export_root, AssetVersionData remote_version_data, AssetVersionData local_version_data, AssetVersionFile version_file, BuildAssetBundleOptions options)
        {
            BuildPipeline.SetAssetBundleEncryptKey(version_file.AssetBundlePW);

            var build_data_list = new List<AssetBundleBuild>();
            foreach (var item in version_file.AssetConfig.ItemList)
            {
                if (item.IsPackAssetBundle)
                {
                    var asset_name_list = new List<string>();
                    var build_data = new AssetBundleBuild{assetBundleName = item.Path + ".ab"};
                    var pack_file_list = Utils.GetExtraResourcesList(item.Path);

                    foreach (var file in pack_file_list)
                        asset_name_list.Add(Utils.GetRelativePath(file.FullName));

                    build_data.assetNames = asset_name_list.ToArray();
                    build_data_list.Add(build_data);
                    // CSFramework.Logger.Log("Ready Pack AssetBundle: " + item.Path);
                }
            };
            
            var manifest = BuildPipeline.BuildAssetBundles(export_root, build_data_list.ToArray(), options, EditorUserBuildSettings.activeBuildTarget);

            var asset_bundle_name_list = manifest.GetAllAssetBundles();
            foreach (var asset_bundle_name in asset_bundle_name_list)
            {
                var full_path = string.Format("{0}/{1}", export_root, asset_bundle_name);
                var md5 = Utils.GetFileMD5(full_path);
                var dependency_name_list = manifest.GetAllDependencies(asset_bundle_name);

                var remote_asset_bundle_info = new AssetBundleInfo
                {
                    Name = asset_bundle_name,
                    DependencyNameList = dependency_name_list,
                    HashValue = md5,
                };
                remote_version_data.AssetBundleInfoDict[asset_bundle_name] = remote_asset_bundle_info;

                var local_asset_bundle_info = new AssetBundleInfo
                {
                    Name = asset_bundle_name,
                    DependencyNameList = dependency_name_list,
                    // HashValue = group.InPackage ? md5 : "",
                    HashValue = md5, // todo
                };
                local_version_data.AssetBundleInfoDict[asset_bundle_name] = local_asset_bundle_info;
            }
        }
    }
}