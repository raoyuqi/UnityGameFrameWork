﻿using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public sealed class EditorAssetsLoader : IAssetsLoader
    {
        private const string r_AssetsPathRoot = "Assets/AssetsPackage/";

        public AssetData LoadAssets(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("LoadAssets field: 非法资源路径");
                return default(AssetData);
            }

#if UNITY_EDITOR
            var assetsPath = Path.Combine(r_AssetsPathRoot, path);
            var assets = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(UnityObject));
            if (assets == null)
            {
                Debug.LogError($"资源不存在，Path = {assetsPath}");
                return default(AssetData);
            }

            var assetData = new AssetData(path, new UnityObject[] { assets });
            return assetData;
#else
            return default;
#endif
        }

        public AssetData LoadAssets<T>(string path) where T : UnityObject
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("LoadAssets field: 非法资源路径");
                return default(AssetData);
            }

#if UNITY_EDITOR
            var assetsPath = Path.Combine(r_AssetsPathRoot, path);
            var assets = AssetDatabase.LoadAssetAtPath<T>(assetsPath);
            if (assets == null)
            {
                Debug.LogError($"资源不存在，Path = {assetsPath}");
                return default(AssetData);
            }

            var assetData = new AssetData(path, new UnityObject[] { assets });
            return assetData;
#else
            return default;
#endif
        }

        //public void LoadAssetAsync<T>(string path, System.Action<AssetData> callback = null) where T : UnityObject
        //{
        //    MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadAssetIEnumerator<T>(path, callback));
        //}

        //private IEnumerator LoadAssetIEnumerator<T>(string path, System.Action<AssetData> callback = null) where T : UnityObject
        //{
        //    yield return null;
        //    var assets = this.LoadAssets<T>(path);
        //    if (callback != null)
        //        callback(assets);
        //}

        public IEnumerator LoadAssetAsync(string path, System.Action<AssetData> callback = null)
        {
            yield return null;
            var assets = this.LoadAssets(path);
            if (callback != null)
                callback(assets);
        }

        public IEnumerator LoadSceneIAsync(string path, System.Action<AssetData> callback = null)
        {
            yield return null;

#if UNITY_EDITOR
            var assetsPath = Path.Combine(r_AssetsPathRoot, path);
            var assetData = new AssetData(path, null) {
                ScenePaths = new string[] { assetsPath }
            };
            if (callback != null)
                callback(assetData);
#else
            if (callback != null)
                callback(default);
#endif
        }
    }
}