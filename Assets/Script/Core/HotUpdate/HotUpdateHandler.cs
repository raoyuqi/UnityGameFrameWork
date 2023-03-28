using FrameWork.Core.Mixin;
using FrameWork.Core.SingletonManager;
using FrameWork.Core.Utils;
using Game.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FrameWork.Core.HotUpdate
{
    public enum HotUpdateStatus
    {
        NotUpdate = 0,      // 无需更新
        ForceUpdate = 1,    // 需要重新下载APP
        UpdateResources = 2,// 需要更新资源

        Downloading = 3,    // 资源下载中

        UpdateSuccess = 4,  // 更新完成
        UpdateFailed = 5,    // 更新失败

        Decompressing = 6,  // 资源解压中
    }

    public struct HotUpdateCallbackInfo
    {
        // 当前进度
        public float CurProgress;
        // 总进度
        public float TotalProgress;
    }

    /// <summary>
    /// 1. 对比本地和远端的版本文件
    /// 2. 若版本不一致，则跳转第三步，否则进入游戏
    /// 3. 大版本不一致，走强更逻辑-->跳转到应用商店
    /// 3. 下载远端的热更新json文件
    /// 4. 对比远端热更json文件和本地的资源清单json文件
    /// 5. 将md5不同的文件添加到下载列表
    /// 6. 开始下载列表中的文件，下载失败，弹窗提示
    /// 7. 下载过程中对比本地文件，若存在且md5一致，则跳过(因为已经下载完成了)
    /// 8. 所有文件下载完成后，使用远端的版本文件覆盖本地
    /// 9. 使用远端的资源清单覆盖本地
    /// </summary>
    public sealed class HotUpdateHandler : SingletonBase<HotUpdateHandler>
    {
        public event Action NotUpdateCallback;
        public event Action<HotUpdateCallbackInfo> UpdateProgressCallback;
        public event Action ForceUpdateCallback;
        public event Action<string> UpdateFailedCallback;
        public event Action UpdateSuccessCallback;

        public HotUpdateStatus UpdateStatus { get; private set; }

        public bool IsFailed
        {
            get
            {
                return this.UpdateStatus == HotUpdateStatus.UpdateFailed;
            }
        }

        // 热更资源Url
        private string m_HotUpdateUrl;
        private string m_AppVersionFileName;
        private string m_ResListFileName;

        // 远端版本文件和资源清单数据
        private byte[] m_RemoteAppVersionFileData;
        private byte[] m_RemoteResListFileData;

        // 需要热更的文件大小 b
        private long m_HotUpdateFileLength;
        // 已下载大小 b
        private long m_DownloadedFileLength;

        // app版本号
        private string m_RemoteAppVersion;
        private string m_LocalAppVersion;

        // 需要更新的资源清单
        private Dictionary<string, Dictionary<string, string>> m_DownloadResourceListDic;

        public void Initialize()
        {
            this.m_HotUpdateFileLength = 0;
            this.m_HotUpdateUrl = AppConst.HotUpdateUrl;
            this.m_AppVersionFileName = AppConst.AppVersionFileName;
            this.m_ResListFileName = AppConst.AppResourceListFileName;
            this.m_DownloadResourceListDic = new Dictionary<string, Dictionary<string, string>>();
        }

        public IEnumerator StartHotUpdateProcessAsync()
        {
            yield return this.CheckAppVersion();
        }

        public IEnumerator CheckAppVersion()
        {
            yield return this.GetRemoteAppVersion();

            if (this.IsFailed)
                yield break;

            if (string.IsNullOrEmpty(this.m_RemoteAppVersion))
            {
                // 远端没有版本文件，无需更行
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate);
                yield break;
            }

            this.m_LocalAppVersion = this.GetLocalAppVersion();
            if (string.IsNullOrEmpty(this.m_LocalAppVersion))
            {
                this.HotUpdateCallback(HotUpdateStatus.UpdateFailed, 0, "本地版本文件丢失");
                yield break;
            }

            if (this.m_RemoteAppVersion == this.m_LocalAppVersion)
            {
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate);
                yield break;
            }

            var remoteVersionArr = this.m_RemoteAppVersion.Split('.');
            var localVersionArr = this.m_LocalAppVersion.Split('.');
            if (remoteVersionArr.Length == 0 || localVersionArr.Length == 0)
            {
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate);
                yield break;
            }

            var remoteLargeVersion = int.Parse(remoteVersionArr[0]);
            var localLargeVersion = int.Parse(localVersionArr[0]);
            if (remoteLargeVersion > localLargeVersion)
            {
                this.HotUpdateCallback(HotUpdateStatus.ForceUpdate);
                yield break;
            }

            var remoteSmallVersion = float.Parse($"{ remoteVersionArr[1] }.{ remoteVersionArr[2] }");
            var localSmallVersion = float.Parse($"{ localVersionArr[1] }.{ localVersionArr[2] }");
            if (remoteLargeVersion == localLargeVersion && remoteSmallVersion > localSmallVersion)
                yield return this.DownloadFile();

            yield return null;
            this.FinalHandle();

            this.HotUpdateCallback(HotUpdateStatus.UpdateSuccess);
        }

        private IEnumerator DownloadFile()
        {
            // 获取需要下载的文件列表
            yield return this.GetDownloadFileList();

            if (this.IsFailed)
                yield break;

            if (this.m_DownloadResourceListDic.Count == 0)
                yield break;

            var hotFileDownloader = new HotFileDownloader();
            hotFileDownloader.OnError += (msg) => this.HotUpdateCallback(HotUpdateStatus.UpdateFailed, 0, msg);
            hotFileDownloader.OnProgress += (curLen, totalLen) => this.HotUpdateCallback(HotUpdateStatus.Downloading, curLen);

            // 开始下载
            foreach (var item in this.m_DownloadResourceListDic)
            {
                if (this.IsFailed)
                    yield break;

                var filePath = item.Value["file"];
                if (this.IsDownloaded(filePath))
                {
                    this.HotUpdateCallback(HotUpdateStatus.Downloading, float.Parse(item.Value["size"]));
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    // var totalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));
                    // 文件夹不存在，需要创建
                    var persistentPath = Application.persistentDataPath;
                    var dirPath = $"{ persistentPath }/{ item.Value["dir"] }";
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    // 断点续传
                    var uri = $"{ this.m_HotUpdateUrl }/{ filePath }";
                    var persistentFilePath = $"{ persistentPath }/{ filePath }";
                    yield return hotFileDownloader.GetRange(uri, persistentFilePath);
                }
            }

            hotFileDownloader.Dispose();
        }

        private IEnumerator GetDownloadFileList()
        {
            using(var www = UnityWebRequest.Get($"{ this.m_HotUpdateUrl }/AssetBundle/{ this.m_ResListFileName }"))
            {
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error))
                {
                    this.HotUpdateCallback(HotUpdateStatus.UpdateFailed, 0, www.error);
                    yield break;
                }

                var jsonStr = www.downloadHandler.text;
                this.m_RemoteResListFileData = www.downloadHandler.data;
                this.m_HotUpdateFileLength += this.m_RemoteResListFileData.Length;

                if (!JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, Dictionary<string, string>> remoteListDic))
                    yield break;

                var localListDic = this.GetLocalResourceList();
                foreach (var item in remoteListDic)
                {
                    if (!localListDic.ContainsKey(item.Key))
                    {
                        // 新增的资源
                        this.m_HotUpdateFileLength += long.Parse(item.Value["size"]);
                        this.m_DownloadResourceListDic.Add(item.Key, item.Value);
                        continue;
                    }

                    var remoteFileInfo = item.Value;
                    var localFileInfo = localListDic[item.Key];
                    if (localFileInfo["md5"] != remoteFileInfo["md5"])
                    {
                        this.m_HotUpdateFileLength += long.Parse(item.Value["size"]);
                        this.m_DownloadResourceListDic.Add(item.Key, item.Value);
                    }
                }
            }
        }

        public IEnumerator GetRemoteAppVersion()
        {
            using (var www = UnityWebRequest.Get($"{ this.m_HotUpdateUrl }/AssetBundle/{ this.m_AppVersionFileName }"))
            {
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error))
                {
                    this.HotUpdateCallback(HotUpdateStatus.UpdateFailed, 0, www.error);
                    yield break;
                }
                else
                {
                    this.m_RemoteAppVersionFileData = www.downloadHandler.data;
                    this.m_HotUpdateFileLength += this.m_RemoteAppVersionFileData.Length;
                    var jsonStr = www.downloadHandler.text;
                    if (JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, string> dic))
                        this.m_RemoteAppVersion = dic[AppConst.AppVersionKey];
                    else
                        yield break;
                }
            }
        }

        public void Dispose()
        {
            this.m_RemoteAppVersionFileData = null;
            this.m_RemoteResListFileData = null;
            this.NotUpdateCallback = null;
            this.UpdateProgressCallback = null;
            this.ForceUpdateCallback = null;
            this.UpdateFailedCallback = null;
            this.UpdateSuccessCallback = null;
        }

        private string GetLocalAppVersion()
        {
            var filePath = $"{ PathTool.GetAssetsBundlePersistentPath() }{ AppConst.AppVersionFileName }";
            if (!File.Exists(filePath))
                filePath = $"{ PathTool.GetAssetsBundleStreamingPath() }{ AppConst.AppVersionFileName }";

            if (!File.Exists(filePath))
                return string.Empty;

            var jsonStr = File.ReadAllText(filePath);
            if (JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, string> dic))
                return dic[AppConst.AppVersionKey];

            return string.Empty;
        }

        private Dictionary<string, Dictionary<string, string>> GetLocalResourceList()
        {
            var filePath = $"{ PathTool.GetAssetsBundlePersistentPath() }{ AppConst.AppResourceListFileName }";
            if (!File.Exists(filePath))
                filePath = $"{ PathTool.GetAssetsBundleStreamingPath() }{ AppConst.AppResourceListFileName }";

            if (!File.Exists(filePath))
                return default;

            var jsonStr = File.ReadAllText(filePath);
            if (JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, Dictionary<string, string>> dic))
                return dic;

            return default;
        }

        private bool IsDownloaded(string filePath)
        {
            var downloadInfo = this.m_DownloadResourceListDic[filePath];
            var absolutePath = $"{ Application.persistentDataPath }/{ filePath }";
            if (!File.Exists(absolutePath))
                return false;

            var fileInfo = new FileInfo(absolutePath);
            var md5 = MD5Util.GetFileInfoMD5(fileInfo);

            return md5 == downloadInfo["md5"];
        }

        private void FinalHandle()
        {
            // 最终处理，更新远端的资源清单和版本文件到本地
            if (this.m_RemoteResListFileData != null)
            {
                var fileNmae = $"{ Application.persistentDataPath }/AssetBundle/{ this.m_ResListFileName }";
                using (var fs = new FileStream(fileNmae, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(this.m_RemoteResListFileData, 0, this.m_RemoteResListFileData.Length);
                    this.HotUpdateCallback(HotUpdateStatus.Downloading, fs.Length);
                }
            }

            if (this.m_RemoteAppVersionFileData != null)
            {
                var fileNmae = $"{ Application.persistentDataPath }/AssetBundle/{ this.m_AppVersionFileName }";
                using (var fs = new FileStream(fileNmae, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(this.m_RemoteAppVersionFileData, 0, this.m_RemoteAppVersionFileData.Length);
                    this.HotUpdateCallback(HotUpdateStatus.Downloading, fs.Length);
                }   
            }
        }

        private void HotUpdateCallback(HotUpdateStatus status, float curLen = 0, string errorMsg = "")
        {
            this.m_DownloadedFileLength += (long)curLen;
            this.UpdateStatus = status;

            switch (status)
            {
                case HotUpdateStatus.NotUpdate:
                    this.NotUpdateCallback?.Invoke();
                    break;
                case HotUpdateStatus.UpdateSuccess:
                    this.UpdateSuccessCallback?.Invoke();
                    break;
                case HotUpdateStatus.ForceUpdate:
                    this.ForceUpdateCallback?.Invoke();
                    break;
                case HotUpdateStatus.UpdateFailed:
                    this.UpdateFailedCallback?.Invoke(errorMsg);
                    break;
                case HotUpdateStatus.Downloading:
                    this.UpdateProgressCallback?.Invoke(new HotUpdateCallbackInfo()
                    {
                        CurProgress = this.m_DownloadedFileLength / 1024,
                        TotalProgress = this.m_HotUpdateFileLength / 1024
                    });
                    break;
                //case HotUpdateStatus.Decompressing:
                //    break;
            }
        }
    }
}