using FrameWork.Core.Mixin;
using FrameWork.Core.SingletonManager;
using FrameWork.Core.Utils;
using Game.Config;
using MiniJSON;
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
        // 更新提示
        public string UpdateTip;
        // 进度
        //public float Progress;
        // 当前进度
        public float CurProgress;
        // 总进度
        public float TotalProgress;

        public bool IsDone;
        public bool IsFailed;
    }

    /// TODO: 分离出http下载文件类
    /// <summary>
    /// 1. 对比本地和远端的版本文件
    /// 2. 若版本不一致，则跳转第三步，否则进入游戏
    /// 3. 大版本不一致，走强更逻辑-->跳转到应用商店
    /// 3. 下载远端的热更新json文件
    /// 4. 对比远端热更json文件和本地的资源清单json文件
    /// 5. 将md5不同的文件添加到下载列表
    /// 6. 开始下载列表中的文件，下载失败，弹窗提示
    /// 7. 将下载完成的文件写入到下载完成的json，用于断点续传，即过滤已下载的文件
    /// 8. 所有文件下载完成后，使用远端的版本文件覆盖本地
    /// 9. 使用远端的资源清单覆盖本地
    /// 10. 删除本地临时生成的记录下载完成的json文件
    /// </summary>
    public sealed class HotUpdateHandler : SingletonBase<HotUpdateHandler>
    {
        public event Action<HotUpdateCallbackInfo> UpdateProgressCallback;
        public event Action ForceUpdateCallback;
        public event Action UpdateFailedCallback;
        public event Action UpdateSuccessCallback;

        //public delegate void HotUpdateCallback(HotUpdateCallbackInfo status);

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

        // 需要热更的文件大小 b
        private long m_HotUpdateFileLength;
        // 已下载大小 b
        private long m_DownloadedFileLength;

        // app版本号
        private string m_RemoteAppVersion;
        private string m_LocalAppVersion;

        // 资源清单
        //private List<Dictionary<string, string>> m_RemoteResourceList;
        //private List<Dictionary<string, string>> m_LocalResourceList;
        private Dictionary<string, Dictionary<string, string>> m_DownloadResourceListDic;
        private Dictionary<string, Dictionary<string, string>> m_DownloadedResourceListDic;

        //private string m_DownloadedResFielPath;

        public void Initialize()
        {
            this.m_HotUpdateUrl = AppConst.HotUpdateUrl;
            this.m_AppVersionFileName = AppConst.AppVersionFileName;
            this.m_ResListFileName = AppConst.AppResourceListFileName;
            //this.m_DownloadedResFielPath = $"{ PathTool.GetAssetsBundlePersistentPath() }downloaded_res_list.json";
            this.m_DownloadResourceListDic = new Dictionary<string, Dictionary<string, string>>();
            this.m_DownloadedResourceListDic = new Dictionary<string, Dictionary<string, string>>();
        }

        public void StartHotUpdateProcess()
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(this.CheckAppVersion());
        }

        public IEnumerator CheckAppVersion()
        {
            yield return this.GetRemoteAppVersion();

            if (string.IsNullOrEmpty(this.m_RemoteAppVersion))
            {
                // 远端没有版本文件，无需更行
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate);
                yield break;
            }

            this.m_LocalAppVersion = this.GetLocalAppVersion();

            Debug.Log($"m_RemoteAppVersion = {this.m_RemoteAppVersion}, m_LocalAppVersion = {this.m_LocalAppVersion}");
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
                Debug.Log($"remoteLargeVersion = {remoteLargeVersion}, localLargeVersion = {localLargeVersion}");
                this.HotUpdateCallback(HotUpdateStatus.ForceUpdate);
                yield break;
            }

            var remoteSmallVersion = float.Parse($"{remoteVersionArr[1]}.{remoteVersionArr[2]}");
            var localSmallVersion = float.Parse($"{localVersionArr[1]}.{localVersionArr[2]}");
            if (remoteLargeVersion == localLargeVersion && remoteSmallVersion > localSmallVersion)
            {
                // TODO: 开始下载文件
                Debug.Log($"remoteSmallVersion = {remoteSmallVersion}, localSmallVersion = {localSmallVersion}");
                yield return this.DownloadFile();
            }

            Debug.Log("热更结束>>>>>>>>>>>>>>");
        }

        private IEnumerator DownloadFile()
        {
            // 获取需要下载的文件列表
            yield return this.GetDownloadFileList();

            if (this.m_DownloadResourceListDic.Count == 0)
            {
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate);
                yield break;
            }

            // 获取已经下载完成的文件列表
            //this.m_DownloadedResourceListDic = this.GetDownloadedFileList();

            // 开始下载
            var hotFileDownloader = new HotFileDownloader();
            hotFileDownloader.OnError += (msg) =>
            {
                // TODO: 弹出错误窗口
                Debug.LogError($"热更新失败 msg = { msg }");
                this.HotUpdateCallback(HotUpdateStatus.UpdateFailed);
            };

            hotFileDownloader.OnProgress += (curLen, totalLen) =>
            {
                Debug.Log($"更新中... curLen = { curLen }, totalLen = { totalLen }");
                this.HotUpdateCallback(HotUpdateStatus.Downloading, curLen);
            };

            foreach (var item in this.m_DownloadResourceListDic)
            {
                if (this.IsFailed)
                    yield break;

                var filePath = item.Value["file"];
                if (this.IsDownloaded(filePath))
                {
                    Debug.Log($"该文件下载完成：{filePath}");
                    this.HotUpdateCallback(HotUpdateStatus.Downloading, float.Parse(item.Value["size"]));
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    // 断点续传
                    var uri = $"{ this.m_HotUpdateUrl }/{ filePath }";
                    //var headRequest = UnityWebRequest.Head(uri);
                    //yield return headRequest.SendWebRequest();

                    //if (!string.IsNullOrEmpty(headRequest.error))
                    //{
                    //    Debug.LogError(headRequest.error);
                    //    this.HotUpdateCallback(HotUpdateStatus.UpdateField, 1);
                    //    yield break;
                    //}

                    //// 获取响应体的长度
                    //var totalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));

                    //// TODO: 文件不存在需要创建
                    var persistentPath = Application.persistentDataPath;
                    var dirPath = $"{ persistentPath }/{ item.Value["dir"] }";
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    var persistentFilePath = $"{ persistentPath }/{ filePath }";
                    yield return hotFileDownloader.GetRange(uri, persistentFilePath);

                    Debug.Log($">>>>>>>>>>>>下载完成 filePath = {filePath}");

                    //using (var fs = new FileStream(persistentFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                    //{
                    //    // 当前文件大小
                    //    var fileLength = fs.Length;
                    //    // 没下载完
                    //    //Debug.Log($"文件大小：{ fileLength } + { totalLength } + { filePath }");
                    //    Debug.Log($"persistentFilePath = {persistentFilePath}");
                    //    if (fileLength < totalLength)
                    //    {
                    //        // 设置本地文件流起始位置
                    //        fs.Seek(fileLength, SeekOrigin.Begin);
                    //        // 请求文件
                    //        var www = UnityWebRequest.Get(uri);
                    //        // 从本地已下载文件大小，请求到文件末尾的所有bytes
                    //        www.SetRequestHeader("Range", $"bytes={ fileLength }-");
                    //        www.SendWebRequest();

                    //        if (!string.IsNullOrEmpty(headRequest.error))
                    //        {
                    //            Debug.LogError(headRequest.error);
                    //            this.HotUpdateCallback(HotUpdateStatus.UpdateField, 1);
                    //            yield break;
                    //        }

                    //        var index = 0;
                    //        while (!www.isDone)
                    //        {
                    //            yield return null;

                    //            // TODO: 优化，使用固定大小的buffer接收数据 DownloadHandlerScript
                    //            // DownloadHandlerScript
                    //            var buffer = www.downloadHandler.data;
                    //            if (buffer != null)
                    //            {
                    //                var length = buffer.Length - index;
                    //                fs.Write(buffer, index, length);
                    //                index += length;
                    //                fileLength += length;
                    //                Debug.Log($"本次下载大小：{ filePath } + { length } + { buffer.Length } + { totalLength }");
                    //                this.HotUpdateCallback(HotUpdateStatus.Downloading, 1);
                    //            }
                    //        }

                    //        Debug.Log($"本次下载大小：www.downloadHandler.data  + { www.downloadHandler.data.Length }");
                    //    }
                    //}

                    // 更新下载完成文件
                    //this.UpdateDownloadedFileListDic(filePath);


                    //AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

                    // TODO: 下载百分比进度
                    //this.HotUpdateCallback(HotUpdateStatus.Downloading, 1);
                }
            }

            hotFileDownloader.Dispose();
        }

        private IEnumerator GetDownloadFileList()
        {
            var www = UnityWebRequest.Get($"{this.m_HotUpdateUrl}/AssetBundle/{this.m_ResListFileName}");
            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                yield break;
            }

            this.m_HotUpdateFileLength = 0;
            Dictionary<string, Dictionary<string, string>> remoteListDic = null;
            Debug.Log(www.downloadHandler.text);
            //byte[] results = www.downloadHandler.data;
            var jsonStr = www.downloadHandler.text;
            if (!JsonUtil.TryDeserializeToDictionary(jsonStr, out remoteListDic))
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
                    Debug.Log($"需要下载：{ item.Key }");
                }
            }

            //this.m_HotUpdateFileLength /= 1024;
            Debug.Log("总共要下载 this.m_HotUpdateFileLength ====== " + this.m_HotUpdateFileLength);
        }

        public IEnumerator GetRemoteAppVersion()
        {
            var www = UnityWebRequest.Get($"{this.m_HotUpdateUrl}/AssetBundle/{this.m_AppVersionFileName}");
            //UnityWebRequestAssetBundle.GetAssetBundle
            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                yield break;
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                //byte[] results = www.downloadHandler.data;
                var jsonStr = www.downloadHandler.text;
                if(JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, string> dic))
                    this.m_RemoteAppVersion = dic[AppConst.AppVersionKey];
                else
                    yield break;
            }
        }

        private string GetLocalAppVersion()
        {
            var filePath = $"{ PathTool.GetAssetsBundlePersistentPath()}{AppConst.AppVersionFileName}";
            if (!File.Exists(filePath))
                filePath = $"{ PathTool.GetAssetsBundleStreamingPath()}{AppConst.AppVersionFileName}";

            if (!File.Exists(filePath))
                return string.Empty;

            var jsonStr = File.ReadAllText(filePath);
            if (JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, string> dic))
                return dic[AppConst.AppVersionKey];

            return string.Empty;
        }

        private Dictionary<string, Dictionary<string, string>> GetLocalResourceList()
        {
            var filePath = $"{ PathTool.GetAssetsBundlePersistentPath()}{AppConst.AppResourceListFileName}";
            if (!File.Exists(filePath))
                filePath = $"{ PathTool.GetAssetsBundleStreamingPath()}{AppConst.AppResourceListFileName}";

            if (!File.Exists(filePath))
                return default;

            var jsonStr = File.ReadAllText(filePath);
            if (JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, Dictionary<string, string>> dic))
                return dic;

            return default;
        }

        private Dictionary<string, Dictionary<string, string>> GetDownloadedFileList()
        {
            var filePath = $"{ PathTool.GetAssetsBundlePersistentPath() }downloaded_res_list.json";
            if (File.Exists(filePath))
            {
                var jsonStr = File.ReadAllText(filePath);
                if (JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, Dictionary<string, string>> dic))
                    return dic;
            }

            return null;
        }

        private bool IsDownloaded(string filePath)
        {
            //if (this.m_DownloadedResourceListDic == null)
            //    return false;

            //var downloadedInfo = this.m_DownloadedResourceListDic[filePath];
            var downloadInfo = this.m_DownloadResourceListDic[filePath];
            var absolutePath = $"{ Application.persistentDataPath }/{ filePath }";
            if (!File.Exists(absolutePath))
                return false;

            var fileInfo = new FileInfo(absolutePath);
            var md5 = MD5Util.GetFileInfoMD5(fileInfo);

            return md5 == downloadInfo["md5"];
        }

        private void UpdateDownloadedFileListDic(string filePath)
        {
            if (this.m_DownloadedResourceListDic == null)
                this.m_DownloadedResourceListDic = new Dictionary<string, Dictionary<string, string>>();

            var absolutePath = $"{ Application.persistentDataPath }/{ filePath }";
            if (!File.Exists(absolutePath))
            {
                Debug.LogError($"本地文件为空: {absolutePath}");
                File.Create(absolutePath);
            }

            var fileInfo = new FileInfo(absolutePath);
            var dirPath = PathTool.GetDirectoryRelativelyPath($"{Application.persistentDataPath}/", fileInfo.DirectoryName);
            var md5 = MD5Util.GetFileInfoMD5(fileInfo);

            if (this.m_DownloadedResourceListDic.ContainsKey(filePath))
            {
                var info = this.m_DownloadedResourceListDic[filePath];
                info["dir"] = dirPath;
                info["file"] = filePath;
                info["md5"] = md5;
                return;
            }

            this.m_DownloadedResourceListDic[filePath] = new Dictionary<string, string>();
            this.m_DownloadedResourceListDic[filePath].Add("dir", dirPath);
            this.m_DownloadedResourceListDic[filePath].Add("file", filePath);
            this.m_DownloadedResourceListDic[filePath].Add("md5", md5);

            using(FileStream fs = File.OpenWrite(absolutePath))
            {
                var json = Json.Serialize(this.m_DownloadedResourceListDic);
            }
        }

        private void HotUpdateCallback(HotUpdateStatus status, float curLen = 0)
        {
            this.m_DownloadedFileLength += (long)curLen;
            Debug.Log("this.m_DownloadedFileLength ==== " + this.m_DownloadedFileLength);
            this.UpdateStatus = status;

            switch (status)
            {
                case HotUpdateStatus.NotUpdate:
                case HotUpdateStatus.UpdateSuccess:
                    this.UpdateSuccessCallback?.Invoke();
                    break;
                case HotUpdateStatus.ForceUpdate:
                    this.ForceUpdateCallback?.Invoke();
                    break;
                //case HotUpdateStatus.UpdateResources:
                //    break;
                case HotUpdateStatus.Downloading:
                    this.UpdateProgressCallback?.Invoke(new HotUpdateCallbackInfo()
                    {
                        CurProgress = this.m_DownloadedFileLength / 1024,
                        TotalProgress = this.m_HotUpdateFileLength / 1024,
                        IsDone = false,
                        IsFailed = false,
                        UpdateTip = "更新中..."
                    });
                    break;
                case HotUpdateStatus.UpdateFailed:
                    this.UpdateFailedCallback?.Invoke();
                    break;
                //case HotUpdateStatus.Decompressing:
                //    break;
            }
        }
    }
}