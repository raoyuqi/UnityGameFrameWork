using FrameWork.Core.Net.Handler;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HttpDownloadErrorEventHandler = FrameWork.Core.Net.Handler.ErrorEventHandler;

namespace FrameWork.Core.HotUpdate
{
    public sealed class HotFileDownloader
    {
        public HttpDownloadErrorEventHandler OnError;

        public CompleteEventHandler OnComplete;

        public ProgressEventHandler OnProgress;

        private int m_ByteCount;

        public HotFileDownloader(int byteCount = 1024)
        {
            this.m_ByteCount = byteCount;
        }

        public IEnumerator GetRange(string uri, string savePath)
        {
            if (string.IsNullOrEmpty(savePath) || string.IsNullOrEmpty(uri))
            {
                // 错误回调
                this.OnError?.Invoke("地址错误");
                yield break;
            }

            var downloadHandler = new HttpDownloadHandler(savePath, new byte[this.m_ByteCount]) {
                OnError = this.OnError,
                OnProgress = this.OnProgress,
                OnComplete = this.OnComplete
            };
            yield return null;

            using (var www = UnityWebRequest.Get(uri))
            {
                //www.disposeDownloadHandlerOnDispose = true;

                // 从本地已下载文件大小，请求到文件末尾的所有bytes
                www.SetRequestHeader("Range", $"bytes={ downloadHandler.DownloadedLength }-");
                www.downloadHandler = downloadHandler;
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError(www.error);
                    // 错误回调
                    this.OnError?.Invoke(www.error);
                }
            }
        }

        public void Dispose()
        {
            this.OnError = null;
            this.OnComplete = null;
            this.OnProgress = null;
        }
    }
}