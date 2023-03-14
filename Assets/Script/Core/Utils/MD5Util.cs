using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace FrameWork.Core.Utils
{
    public static class MD5Util
    {
        public static string GetObjectMD5(object obj)
        {
            if (obj == null)
                return string.Empty;

            var data = ObjectToByteArray(obj);
            return GetMD5(data);
        }

        public static string GetFileInfoMD5(FileInfo fileInfo)
        {
            using (var fileStream = fileInfo.Open(FileMode.Open))
            {
                try
                {
                    // 设置到流的开头
                    fileStream.Position = 0;
                    var md5 = new MD5CryptoServiceProvider();
                    byte[] hashValue = md5.ComputeHash(fileStream);
                    var sBuilder = new StringBuilder();
                    for (int i = 0; i < hashValue.Length; i++)
                        sBuilder.Append(hashValue[i].ToString("x2"));

                    return sBuilder.ToString();
                }
                catch (IOException e)
                {
                    UnityEngine.Debug.LogError($"I/O Exception: {e.Message}");
                }
                catch (UnauthorizedAccessException e)
                {
                    UnityEngine.Debug.LogError($"Access Exception: {e.Message}");
                }

                return string.Empty;
            }
        }

        public static string GetHashByString(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            return GetMD5(data);
        }

        public static string GetMD5(byte[] data)
        {
            var md5 = new MD5CryptoServiceProvider();
            var hashValue = md5.ComputeHash(data);
            var sBuilder = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
                sBuilder.Append(hashValue[i].ToString("x2"));

            return sBuilder.ToString();
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }
    }
}

