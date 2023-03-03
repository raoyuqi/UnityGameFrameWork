using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public sealed class EditorAssetsLoader : IAssetsLoader
    {
        private readonly string r_AssetsPathRoot = "Assets/AssetsPackage/";

        public Object LoadAssets(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("LoadAssets field: 非法资源路径");
                return null;
            }

            var assetsPath = Path.Combine(r_AssetsPathRoot, path);
            var assets = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object));
            if (assets == null)
            {
                Debug.LogError($"资源不存在，Path = {assetsPath}");
                return null;
            }

            return assets;
        }

        public T LoadAssets<T>(string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("LoadAssets field: 非法资源路径");
                return null;
            }

            var assetsPath = Path.Combine(r_AssetsPathRoot, path);
            var assets = AssetDatabase.LoadAssetAtPath<T>(assetsPath);
            if (assets == null)
            {
                Debug.LogError($"资源不存在，Path = {assetsPath}");
                return null;
            }

            return assets;
        }

        public IEnumerator LoadAssetsAsync<T>(string path, System.Action<T> callback = null) where T : Object
        {
            yield return null;
            var assets = this.LoadAssets<T>(path);
            if (callback != null)
                callback(assets);
        }
    }
}