using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Newtonsoft.Json;
using BestHTTP;

namespace CSFramework
{
    public class VersionManager : Module<Framework>
    {
        public AssetVersionFile LocalVersionFile { get; private set; }
        public AssetVersionData LocalVersionData { get; private set; }
        public AssetVersionData RemoteVersionData { get; private set; }

        protected override IEnumerator on_init (params object[] param_list)
        {
            // load version file
            LocalVersionFile = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
            // load version data
            var text_asset = Resources.Load<TextAsset>(LocalVersionFile.LocalVersionFileName);
            LocalVersionData = JsonConvert.DeserializeObject<AssetVersionData>(text_asset ? text_asset.text : "{}");
            RemoteVersionData = null;
            yield return null;
        }

        public string GetDisplayVersion ()
        {
            return LocalVersionData.Version.Replace('_', '.');
        }

        public string GetRemoteAssetBundleUrl (string asset_bundle_filename)
        {
            if (RemoteVersionData != null)
            {
                var info = RemoteVersionData.FindAssetBundleInfo(asset_bundle_filename);
                if (info != null)
                    return $"{LocalVersionFile.AssetBundleDownloadUrl}/{RemoteVersionData.Version}/{asset_bundle_filename}";
            }
            return null;
        }

        public string GetRemoteFileUrl (string file_path)
        {
            return $"{LocalVersionFile.AssetBundleDownloadUrl}/{LocalVersionData.Version}/{file_path}";
        }

        public void LoadRemoteVersionData (Action<bool> complete_callback)
        {
            StartCoroutine(load_remote_version_data(complete_callback));
        }

        private IEnumerator load_remote_version_data (Action<bool> complete_callback)
        {
            var load_success = false;
            var received_response = false;
            var url = $"{LocalVersionFile.AssetBundleDownloadUrl}/{LocalVersionFile.RemoteVersionFileName}.txt";
            log($"LoadRemoteVersionData Start: url = {url}");
            var request = new HTTPRequest(new Uri(url), (req, resp) => 
            {
                if (resp != null && resp.StatusCode >= 200 && resp.StatusCode < 300)
                {
                    log($"LoadRemoteVersionData Success: url = {url}");
                    RemoteVersionData = JsonConvert.DeserializeObject<AssetVersionData>(resp.DataAsText);
                    load_success = true;
                    received_response = true;
                }
                else
                {
                    log($"LoadRemoteVersionData Failed: url = {url}");
                    received_response = true;
                }
            })
            {
                DisableCache = true,
                IsCookiesEnabled = false,
                ConnectTimeout = TimeSpan.FromSeconds(5),
                Timeout = TimeSpan.FromSeconds(10),
            };
            request.Send();
            while (!received_response)
                yield return new WaitForEndOfFrame();

            complete_callback?.Invoke(load_success);
        }

        public List<AssetBundleInfo> FilterUpdateAssetBundle (IEnumerable<AssetBundleInfo> info_list)
        {
            var update_assetbundle_info_list = new List<AssetBundleInfo>();
            foreach (var info in info_list)
            {
                var path = info.Name.ToLower();
                var download_path = path.ToDownloadPath();
                var md5 = Utils.GetFileMD5(download_path);
                if (string.IsNullOrEmpty(md5) || md5.Trim() != info.HashValue.Trim())
                    update_assetbundle_info_list.Add(info);
            }
            return update_assetbundle_info_list;
        }

        public bool CheckAssetBundleChanged (string asset_bundle_path)
        {
            if (RemoteVersionData != null)
            {
                if (RemoteVersionData.AssetBundleInfoDict.TryGetValue(asset_bundle_path, out var remote_info) && 
                    !string.IsNullOrEmpty(remote_info.HashValue))
                {
                    var md5_remote = remote_info.HashValue.Trim();
                    var md5_local = "";
                    // 先检测本地的版本信息
                    if (LocalVersionData.AssetBundleInfoDict.TryGetValue(asset_bundle_path, out var local_info) && 
                        !string.IsNullOrEmpty(local_info.HashValue))
                    {
                        md5_local = local_info.HashValue.Trim();
                    }

                    if (md5_remote == md5_local)
                        return false;

                    // 再检测本地的实际下载资源
                    md5_local = Utils.GetFileMD5(asset_bundle_path.ToDownloadPath());
                    if (md5_remote == md5_local)
                        return false;

                    return true;
                }
            }
            return false;
        }
    }
}