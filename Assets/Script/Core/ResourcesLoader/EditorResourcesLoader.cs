﻿using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Core.ResourcesLoader
{
    // TODO: 更新引用计数
    public sealed class EditorResourcesLoader : IResourcesLoader
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
    }
}