using FrameWork.Core.Mixin;
using FrameWork.Core.Utils;
using Game.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FrameWork.Core.SingletonManager
{
    public enum HotUpdateStatus
    {
        NotUpdate = 0,      // 无需更新
        ForceUpdate = 1,    // 需要重新下载APP
        UpdateResources = 2,// 需要更新资源

        Downloading = 3,    // 资源下载中

        UpdateSuccess = 4,  // 更新完成
        UpdateField = 5,    // 更新失败

        Decompressing = 6,  // 资源解压中
    }

    public struct HotUpdateCallbackInfo
    {
        // 更新提示
        public string UpdateTip;
        // 进度
        public float Progress;

        public bool IsDone;
        public bool IsFailed;
    }

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
        public event Action<HotUpdateCallbackInfo> UpdateCallback;

        //public delegate void HotUpdateCallback(HotUpdateCallbackInfo status);

        public HotUpdateStatus HotUpdateStatus { get; private set; }

        // 热更资源Url
        private string m_HotUpdateUrl = AppConst.HotUpdateUrl;
        private string m_AppVersionFileName = AppConst.AppVersionFileName;
        private string m_ResListFileName = AppConst.AppResourceListFileName;

        // app版本号
        private string m_RemoteAppVersion;
        private string m_LocalAppVersion;

        // 资源清单
        //private List<Dictionary<string, string>> m_RemoteResourceList;
        //private List<Dictionary<string, string>> m_LocalResourceList;
        private Dictionary<string, Dictionary<string, string>> m_DownloadResourceListDic;
        private Dictionary<string, Dictionary<string, string>> m_DownloadedResourceListDic;

        public HotUpdateHandler()
        {
            this.m_DownloadResourceListDic = new Dictionary<string, Dictionary<string, string>>();
            this.m_DownloadedResourceListDic = new Dictionary<string, Dictionary<string, string>>();
        }

        public IEnumerator CheckAppVersion()
        {
            yield return this.GetRemoteAppVersion();

            if (string.IsNullOrEmpty(this.m_RemoteAppVersion))
            {
                // 远端没有版本文件，无需更行
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate, 1);
                yield break;
            }

            this.m_LocalAppVersion = this.GetLocalAppVersion();
            var remoteVersionArr = this.m_RemoteAppVersion.Split('.');
            var localVersionArr = this.m_LocalAppVersion.Split('.');
            if (remoteVersionArr.Length == 0 || localVersionArr.Length == 0)
            {
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate, 1);
                yield break;
            }

            var remoteLargeVersion = int.Parse(remoteVersionArr[0]);
            var localLargeVersion = int.Parse(localVersionArr[0]);
            if (remoteLargeVersion > localLargeVersion)
            {
                this.HotUpdateCallback(HotUpdateStatus.ForceUpdate, 1);
                yield break;
            }

            var remoteSmallVersion = int.Parse($"{remoteVersionArr[1]}.{remoteVersionArr[2]}");
            var localSmallVersion = int.Parse($"{localVersionArr[1]}.{localVersionArr[2]}");
            if (remoteLargeVersion == localLargeVersion && remoteSmallVersion > localSmallVersion)
            {
                // TODO: 开始下载文件
            }
        }

        private IEnumerator DownloadFile()
        {
            // 获取需要下载的文件列表
            yield return this.GetDownloadFileList();

            if (this.m_DownloadResourceListDic.Count == 0)
            {
                this.HotUpdateCallback(HotUpdateStatus.NotUpdate, 1);
                yield break;
            }

            // 开始下载
            foreach (var item in this.m_DownloadResourceListDic)
            {
                // TODO:下载完成校验

                var filePath = item.Value["file"];
                var www = UnityWebRequest.Get($"{ this.m_HotUpdateUrl }/{ filePath }");
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError(www.error);
                    this.HotUpdateCallback(HotUpdateStatus.UpdateField, 1);
                    yield break;
                }

                Debug.Log($"下载成功: {filePath}");
                // TODO: 文件不存在需要创建
                var persistentPath = PathTool.GetAssetsBundlePersistentPath();
                var dirPath = $"{ persistentPath }{ item.Value["dir"] }";
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                var persistentFilePath = $"{ persistentPath }{ filePath }";
                using (var fs = File.OpenWrite(persistentFilePath))
                {
                    var data = www.downloadHandler.data;
                    fs.Write(data, 0, data.Length);
                }

                var size = new FileInfo(persistentFilePath).Length;
                Debug.Log("文件大小：" + size);

                //AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

                // TODO: 下载百分比进度
                this.HotUpdateCallback(HotUpdateStatus.Downloading, 1);
            }
        }


        private IEnumerator GetDownloadFileList()
        {
            var www = UnityWebRequest.Get($"{this.m_HotUpdateUrl}/{this.m_ResListFileName}");
            yield return www.SendWebRequest();

            Dictionary<string, Dictionary<string, string>> remoteListDic = null;
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
                if (!JsonUtil.TryDeserializeToDictionary(jsonStr, out remoteListDic))
                    yield break;

                var localListDic = this.GetLocalResourceList();
                foreach (var item in remoteListDic)
                {
                    if (!localListDic.ContainsKey(item.Key))
                    {
                        // 新增的资源
                        this.m_DownloadedResourceListDic.Add(item.Key, item.Value);
                        continue;
                    }

                    var remoteFileInfo = item.Value;
                    var localFileInfo = localListDic[item.Key];
                    if (localFileInfo["md5"] != remoteFileInfo["md5"])
                        this.m_DownloadedResourceListDic.Add(item.Key, item.Value);
                }
            }
        }

        public IEnumerator GetRemoteAppVersion()
        {
            var www = UnityWebRequest.Get($"{this.m_HotUpdateUrl}/{this.m_AppVersionFileName}");
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

        private void HotUpdateCallback(HotUpdateStatus status, float progress)
        {
            // TODO: 简化代码
            var info = new HotUpdateCallbackInfo()
            {
                Progress = progress,
                IsDone = status == HotUpdateStatus.UpdateSuccess || status == HotUpdateStatus.NotUpdate,
                IsFailed = status == HotUpdateStatus.UpdateField,
                UpdateTip = "更新中..."
            };
            this.UpdateCallback?.Invoke(info);
        }
    }
}