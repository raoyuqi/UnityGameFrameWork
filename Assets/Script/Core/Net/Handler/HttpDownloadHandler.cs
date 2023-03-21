using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FrameWork.Core.Net.Handler
{
    public delegate void CompleteEventHandler();
    public delegate void ErrorEventHandler(string msg);
    public delegate void ProgressEventHandler(float curLength, float totalLength);

    public sealed class HttpDownloadHandler : DownloadHandlerScript
    {
        public CompleteEventHandler OnComplete;

        public ErrorEventHandler OnError;

        public ProgressEventHandler OnProgress;

        // 文件总长度
        private long m_TotalLength;
        // 本次下载长度
        private long m_ContentLength;
        // 本地文件已下载长度
        public long DownloadedLength { get; private set; }
        // 文件保存路径
        private string m_SaveFilePath;

        public HttpDownloadHandler(string filePath, byte[] buffer) : base(buffer)
        {
            this.m_SaveFilePath = filePath;
            try
            {
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    this.DownloadedLength = fs.Length;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"msg = {ex.Message}");
                this.OnError?.Invoke(ex.Message);
            }
        }

        ~HttpDownloadHandler()
        {
            // 清理操作
        }

        // 接收到带有Content-Length标头的回调
        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            this.m_ContentLength = (long)contentLength;
            this.m_TotalLength = this.DownloadedLength + this.m_ContentLength;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength <= 0)
                return false;

            using (var fs = new FileStream(this.m_SaveFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                // 当前文件大小
                //var fileLength = fs.Length;

                // 设置本地文件流起始位置
                fs.Seek(fs.Length, SeekOrigin.Begin);
                fs.Write(data, 0, dataLength);
                this.DownloadedLength += dataLength;
                // 回调
                this.OnProgress?.Invoke(dataLength, this.m_TotalLength);
                return true;
            }
        }

        protected override void CompleteContent()
        {
            this.OnComplete?.Invoke();
        }
    }
}