using Game.Config;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FrameWork.Core.Utils
{
    public static class PathTool
    {
        /// <summary>
        /// 获取资源加载路径
        /// </summary>
        /// <param name="relativelyPath">文件相对路径</param>
        /// <returns></returns>
        public static string GetAssetAbsolutePath(string relativelyPath)
        {
            var path = new StringBuilder();
#if UNITY_EDITOR
            if (AppConst.IsAssetBundle)
            {
                path.Append(Application.streamingAssetsPath)
                    .Append("/AssetBundle/")
                    .Append(relativelyPath);
            }
            else
            {
                path.Append(relativelyPath);
            }
#else
        path.Append(Application.streamingAssetsPath)
            .Append("/")
            .Append(relativelyPath);
#endif

            return path.ToString();
        }

        /// <summary>
        /// 热更资源存放在Application.persistentDataPath+"/Resources/"目录下
        /// </summary>
        /// <returns></returns>
        public static string GetAssetsBundlePersistentPath()
        {
            return Application.persistentDataPath + "/AssetBundle/";
        }

        public static string GetAssetsBundleStreamingPath()
        {
            return Application.streamingAssetsPath + "/AssetBundle/";
        }

        /// <summary>
        /// 获取目录文件相对路径
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <param name="fullName">文件绝对路径</param>
        /// <returns></returns>
        public static string GetDirectoryRelativelyPath(string directoryPath, string fullName)
        {
            directoryPath = directoryPath.Replace(@"\", "/");
            fullName = fullName.Replace(@"\", "/");
            return fullName.Replace(directoryPath, "");
        }

        public static string PathCombine(params string[] paths) => string.Join(
            "/",
            paths.Where(r => !string.IsNullOrEmpty(r))
        ).Replace('\\', '/').Replace("//", "/");
    }
}