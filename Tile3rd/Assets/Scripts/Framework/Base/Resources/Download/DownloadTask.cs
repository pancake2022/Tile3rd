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
    public class DownloadTask
    {
        public DownloadData Data { get; private set; }
        private DownloadManager _download_manager;
        private HTTPRequest _head_request;
        private HTTPRequest _content_request;

        public DownloadTask (DownloadData data, DownloadManager download_manager)
        {
            Data = data;
            _download_manager = download_manager;
        }

        public void StartDownload ()
        {
            CSFramework.Logger.Log($"DownloadTask-Head: start download {Data.Url}");
            Data.SetDownloadResult(DownloadResult.None);
            Data.SetDownloadState(DownloadState.Downloading);

            _head_request = new HTTPRequest(new Uri(Data.Url), HTTPMethods.Head, (req, resp) => 
            {
                if (Data.Result == DownloadResult.Abort)
                    return;

                if (_download_manager.IsTaskExist(this))
                {
                    if (resp == null)
                    {
                        CSFramework.Logger.Log($"DownloadTask-Head: Response Was Null");
                        Data.SetDownloadResult(DownloadResult.ServerUnreachable);
                        _download_manager.RetryDownload(this);
                    }
                    else if (resp.StatusCode == 200 || resp.StatusCode == 206)
                    {
                        try
                        {
                            Data.FileSize = int.Parse(resp.GetFirstHeaderValue("Content-Length"));
                            start_break_point_download();
                        }
                        catch (Exception e)
                        {
                            CSFramework.Logger.Error($"DownloadTask-Head: exception in download[{Data.FileName}]: {e}");
                            Data.SetDownloadResult(DownloadResult.Failed);
                            _download_manager.RetryDownload(this);
                        }
                    }
                    else
                    {
                        CSFramework.Logger.Log($"DownloadTask-Head: {Data.Url} Failed, StatusCode: {resp.StatusCode}");
                        Data.SetDownloadResult(DownloadResult.Failed);
                        _download_manager.RetryDownload(this);
                    }
                }
                else
                {
                    CSFramework.Logger.Log($"DownloadTask-Head: {Data.Url} Canceled");
                }
            })
            {
                DisableCache = true
            };

            _head_request.Send();
        }

        public void AbortDownload (bool retry)
        {
            Data.SetDownloadResult(DownloadResult.Abort);
            _content_request.Abort();

            if (retry)
                _download_manager.RetryDownload(this);
            else
                _download_manager.OnDownloadTaskFinished(this);
        }

        private void start_break_point_download ()
        {
            _content_request = new HTTPRequest(new Uri(Data.Url), HTTPMethods.Get, (req, resp) => 
            {
                if (Data.Result == DownloadResult.Abort)
                    return;

                if (resp == null)
                {
                    Data.SetDownloadResult(DownloadResult.ServerUnreachable);
                    CSFramework.Logger.Log($"DownloadTask: {Data.Url} Failed, Response Was Null");
                    AbortDownload(true);
                }
                else if (resp.StatusCode >= 200 && resp.StatusCode < 300)
                {
                    read_stream(req, resp);
                }
                else
                {
                    if (resp.StatusCode == 416) // 416表示本地续传文件字节数大于服务器上对应文件的字节数，所以需要删除本地的续传文件（大概率本地续传文件已出错）
                    {
                        if (File.Exists(Data.TempPath))
                            File.Delete(Data.TempPath);
                    }
                    Data.SetDownloadResult(DownloadResult.Failed);
                    CSFramework.Logger.Log($"DownloadTask: {Data.Url} Failed, StatusCode: {resp.StatusCode}");
                    AbortDownload(true);
                }
            });

            _content_request.SetRangeHeader(Data.DownloadedSize, Data.FileSize);
            _content_request.IsKeepAlive = true;
            _content_request.UseStreaming = true;
            _content_request.StreamFragmentSize = 1 * 1024 * 10; // 10K  1M
            _content_request.DisableCache = true;
            _content_request.IsCookiesEnabled = false;
            _content_request.ConnectTimeout = TimeSpan.FromSeconds(5);
            _content_request.Timeout = TimeSpan.FromSeconds(20);
            _content_request.EnableTimoutForStreaming = true;

            _content_request.Send();
        }

        private void read_stream (HTTPRequest request, HTTPResponse response)
        {
            try
            {
                var streamed_fragment_list = response.GetStreamedFragments();
                if (streamed_fragment_list != null)
                {
                    var size = 0;
                    using (var fs = new FileStream(Data.TempPath, FileMode.Append))
                    {
                        foreach (var streamed_fragment in streamed_fragment_list)
                        {
                            size += streamed_fragment.Length;
                            fs.Write(streamed_fragment, 0, streamed_fragment.Length);
                        }
                    }
                    Data.DownloadedSize += size;
                    Data.OnProgressUpdate();
                }
            }
            catch (Exception e)
            {
                CSFramework.Logger.Error($"DownloadTask: exception in read stream[{Data.FileName}]: {e}");
                // todo check disk full
                Data.SetDownloadResult(DownloadResult.Failed);
                AbortDownload(true);
                return;
            }

            if (!response.IsStreamingFinished || request.State != HTTPRequestStates.Finished)
                return;

            // 下载完成之后进行MD5校验
            var md5 = Utils.GetFileMD5(Data.TempPath).Trim();
            var dst_md5 = Data.MD5.Trim();
            if (md5 != dst_md5)
            {
                if (File.Exists(Data.TempPath))
                    File.Delete(Data.TempPath);

                Data.SetDownloadResult(DownloadResult.MD5VerifyFailed);
                CSFramework.Logger.Error($"DownloadTask: [{Data.FileName}] MD5 VerifyFailed, DownloadMD5: [{md5}], TargetMD5: [{dst_md5}]");
                AbortDownload(true);
            }
            else
            {
                CSFramework.Logger.Log($"DownloadTask: {Data.Url} Success");
                _download_manager.OnDownloadTaskFinished(this);
            }
        }
    }
}