using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Newtonsoft.Json;
using UnityEngine.Networking;
using BestHTTP;

namespace CSFramework
{
    public class DownloadContext
    {
        public int MaxDownloadTaskCount = 4;
        public int MaxRetryTimes = 3;
    }

    public class DownloadManager : Module<Framework>
    {
        private DownloadContext _context;
        private VersionManager _version_manager;
        private Dictionary<string, DownloadTask> _task_dict;
        private Queue<DownloadData> _waiting_queue;
        private Queue<DownloadData> _completed_queue;
        
        protected override IEnumerator on_init (params object[] param_list)
        {
            _context = _main_module.Context.DownloadContext;
            _version_manager = _main_module.VersionManager;
            _task_dict = new Dictionary<string, DownloadTask>();
            _waiting_queue = new Queue<DownloadData>();
            _completed_queue = new Queue<DownloadData>();
            return null;
        }

        public DownloadGroupData StartDownloadAssetBundleList (List<AssetBundleInfo> info_list, Action<DownloadGroupData> complete_callback)
        {
            var group_data = new DownloadGroupData(this, complete_callback);
            if (info_list.Count > 0)
            {
                foreach (var info in info_list)
                {
                    var filename = Utils.NormalizePath(info.Name);
                    var url = _main_module.VersionManager.GetRemoteAssetBundleUrl(filename);
                    var data = group_data.AddDownloadData(filename, info.HashValue, url);
                    _waiting_queue.Enqueue(data);
                }
            }
            else
            {
                DelayCall(() => complete_callback?.Invoke(group_data), 0);
            }
            return group_data;
        }

        public DownloadData StartDownloadAssetBundle (AssetBundleInfo info, Action<DownloadData> complete_callback)
        {
            var filename = Utils.NormalizePath(info.Name);
            var url = _main_module.VersionManager.GetRemoteAssetBundleUrl(filename);
            var data = new DownloadData(filename, info.HashValue, url, complete_callback);
            _waiting_queue.Enqueue(data);
            return data;
        }

        public DownloadData StartDownload (string filename, string md5, string url, Action<DownloadData> complete_callback)
        {
            filename = Utils.NormalizePath(filename);
            var data = new DownloadData(filename, md5, url, complete_callback);
            _waiting_queue.Enqueue(data);
            return data;
        }

        public void RetryDownload (DownloadTask task)
        {
            if (task.Data.RetryTimes < _context.MaxRetryTimes)
            {
                task.Data.OnRetry();
                _waiting_queue.Enqueue(task.Data);
                _task_dict.Remove(task.Data.FileName);
            }
            else
            {
                OnDownloadTaskFinished(task);
            }
        }

        public void AbortDownload (DownloadData data)
        {
            if (_task_dict.TryGetValue(data.FileName, out var task))
                AbortDownload(task);
        }

        public void AbortDownload (DownloadTask task)
        {
            task.AbortDownload(false);
        }

        public void AbortAllDownload ()
        {
            var task_list = new List<DownloadTask>(_task_dict.Values);
            foreach (var task in task_list)
                AbortDownload(task);
                
            _task_dict.Clear();
            _waiting_queue.Clear();
        }

        public void OnDownloadTaskFinished (DownloadTask task)
        {
            var data = task.Data;
            _task_dict.Remove(data.FileName);
            _completed_queue.Enqueue(data);
        }

        public bool IsTaskExist (DownloadTask task)
        {
            return _task_dict.ContainsKey(task.Data.FileName);
        }

        protected override void on_tick (float dt)
        {
            if (_waiting_queue.Count > 0)
            {
                var free_task_count = _context.MaxDownloadTaskCount - _task_dict.Count;
                while (free_task_count > 0)
                {
                    if (_waiting_queue.Count > 0)
                    {
                        var data = _waiting_queue.Dequeue();
                        if (data.State == DownloadState.Waiting)
                        {
                            start_task(data);
                            --free_task_count;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            while (_completed_queue.Count > 0)
                _completed_queue.Dequeue().OnCompleted();
        }

        private void start_task (DownloadData data)
        {
            if (_task_dict.TryGetValue(data.FileName, out var task))
            {
                CSFramework.Logger.Warning($"DownloadManager: Task[{data.FileName}] already in progress");
            }
            else
            {
                task = new DownloadTask(data, this);
                _task_dict[task.Data.FileName] = task;

                if (!File.Exists(task.Data.TempPath))
                {
                    Utils.MakeSureDirectoryExist(task.Data.TempPath);
                    File.Create(task.Data.TempPath).Dispose();
                }

                using (var fs = new FileStream(task.Data.TempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    task.Data.DownloadedSize = (int)fs.Length;
                }

                task.StartDownload();
            }
        }

        public IEnumerator CopyFile (string file_path_src, string file_path_dst)
        {
            using (var www = UnityWebRequest.Get(file_path_src))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Logger.Error(string.Format("CopyFile Get error: {0}", www.error));
                    yield break;
                }
                else
                {
                    var data = www.downloadHandler.data;
                    var dir_dst = Path.GetDirectoryName(file_path_dst);
                    if (!Directory.Exists(dir_dst))
                        Directory.CreateDirectory(dir_dst);
                    else if (File.Exists(file_path_dst))
                        File.Delete(file_path_dst);

                    try
                    {
                        using (var fs = new FileStream(file_path_dst, FileMode.Create))
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            fs.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(string.Format("CopyFile copy error: {0}", e));
                        yield break;
                    }
                }
            }
        }

        public void WriteFile (string file_path, string content)
        {
            var dir = Path.GetDirectoryName(file_path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else if (File.Exists(file_path))
                File.Delete(file_path);
            
            using (var file_stream = File.CreateText(file_path))
            {
                file_stream.WriteLine(content);
            }
        }
    }
}