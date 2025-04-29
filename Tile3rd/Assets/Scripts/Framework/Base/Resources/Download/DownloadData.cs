using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace CSFramework
{
    public enum DownloadState
    {
        Waiting,
        Downloading,
        Completed,
    }

    public enum DownloadResult
    {
        None = -1,
        Success = 0,
        Failed,
        Timeout,
        ServerUnreachable,
        MD5VerifyFailed,
        Abort,
    }

    public class DownloadData
    {
        public string FileName = ""; // 文件名
        public string MD5 = ""; // 文件MD5
        public int FileSize; // 文件大小（字节数）
        public int DownloadedSize; // 已下载大小
        public string Url = ""; // 下载地址
        public string SavePath = ""; // 下载成功后的保存路径
        public string TempPath = ""; // 下载过程中的缓存路径
        public float Timeout = 20.0f; // 超时时间
        public Action<DownloadData> CompleteCallback; // 下载结束的回调
        public Action<DownloadData> ProgressCallback; // 下载进度的回调
        public bool AutoSaveTempFile = true;
        public int RetryTimes = 0; // 重试次数
        public DownloadState State { get; private set; } = DownloadState.Waiting; // 下载状态
        public DownloadResult Result { get; private set; } = DownloadResult.None; // 下载结果
        public float DownloadProgress = 0.0f;

        public DownloadData (string file_name, string md5, string url, Action<DownloadData> complete_callback)
        {
            FileName = file_name;
            MD5 = md5;
            Url = url;
            CompleteCallback = complete_callback;

            SavePath = FileName.ToDownloadPath();
            TempPath = $"{FileName}.temp".ToDownloadPath();
        }

        public bool SaveTempFile ()
        {
            if (Result == DownloadResult.Success)
            {
                if (File.Exists(SavePath))
                    File.Delete(SavePath);

                File.Move(TempPath, SavePath);
                return true;
            }
            return false;
        }

        public void OnProgressUpdate ()
        {
            DownloadProgress = (float)DownloadedSize / (float)FileSize;
            ProgressCallback?.Invoke(this);
        }

        public void OnRetry ()
        {
            ++RetryTimes;
            Result = DownloadResult.None;
            State = DownloadState.Waiting;
            DownloadProgress = 0.0f;

            CSFramework.Logger.Log($"RetryDownload[{FileName}]: RetryTimes = {RetryTimes}");
        }

        public void OnCompleted ()
        {
            SetDownloadResult(DownloadResult.Success);

            if (AutoSaveTempFile)
                SaveTempFile();

            CompleteCallback?.Invoke(this);
        }

        public void SetDownloadResult (DownloadResult result)
        {
            Result = result;
            if (result != DownloadResult.None)
                State = DownloadState.Completed;
        }

        public void SetDownloadState (DownloadState state)
        {
            State = state;
        }
    }

    /// <summary>
    /// 组任务，只有组里面的下载任务全部成功之后，下载文件才会被替换
    /// </summary>
    public class DownloadGroupData
    {
        public List<DownloadData> DataList = new List<DownloadData>();
        public Action<DownloadGroupData> CompleteCallback; // 下载结束的回调
        public Action<DownloadGroupData> ProgressCallback; // 下载进度的回调
        public DownloadResult Result = DownloadResult.None; // 下载结果
        public float DownloadProgress = 0.0f;

        private DownloadManager _download_manager;

        public DownloadGroupData (DownloadManager download_manager, Action<DownloadGroupData> complete_callback)
        {
            _download_manager = download_manager;
            CompleteCallback = complete_callback;
        }

        public DownloadData AddDownloadData (string file_name, string md5, string url)
        {
            var data = new DownloadData(file_name, md5, url, on_download_complete);
            data.ProgressCallback += on_download_progress;
            data.AutoSaveTempFile = false;
            DataList.Add(data);
            return data;
        }

        private void on_download_complete (DownloadData data)
        {
            if (data.Result == DownloadResult.Success)
            {
                var all_completed = true;
                foreach (var di in DataList)
                {
                    if (di.State != DownloadState.Completed)
                    {
                        all_completed = false;
                        break;
                    }
                }

                if (all_completed)
                {
                    foreach (var di in DataList)
                        di.SaveTempFile();

                    Result = DownloadResult.Success;
                    CompleteCallback?.Invoke(this);
                }
            }
            else
            {
                // abort all not completed task
                foreach (var di in DataList)
                {
                    if (di.State != DownloadState.Completed)
                        _download_manager.AbortDownload(di);
                }

                Result = data.Result;
                CompleteCallback?.Invoke(this);
            }
        }

        private void on_download_progress (DownloadData data)
        {
            var total_progress = 0.0f;
            foreach (var di in DataList)
                total_progress += di.DownloadProgress;

            DownloadProgress = total_progress / DataList.Count;
            ProgressCallback?.Invoke(this);
        }
    }
}