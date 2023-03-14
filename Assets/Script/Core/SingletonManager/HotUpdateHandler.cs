using FrameWork.Core.Mixin;
using FrameWork.Core.Utils;
using Game.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace FrameWork.Core.SingletonManager
{
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
    public sealed class HotUpdateHandler : SingletonBase<MemoryManger>
    {
        // 远程app版本号
        private string m_RemoteAppVersion;
        private string m_LocalAppVersion;

        public IEnumerator CheckVersion()
        {
            var www = UnityWebRequest.Get("");
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
                byte[] results = www.downloadHandler.data;
                var jsonStr = www.downloadHandler.text;
                if(JsonUtil.TryDeserializeToDictionary(jsonStr, out Dictionary<string, string> dic))
                    this.m_RemoteAppVersion = dic[AppConst.AppVersionKey];
                else
                    yield break;
            }


        }
    }
}