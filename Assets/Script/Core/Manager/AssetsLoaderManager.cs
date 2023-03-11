﻿using FrameWork.Core.Mixin;
using FrameWork.Core.Modules.AssetsLoader;
using Game.Config;
using System.Collections;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.Manager
{
    // TODO: 更新引用计数，资源卸载，加载依赖
    public sealed class AssetsLoaderManager : SingletonBase<AssetsLoaderManager>
    {
        private Dictionary<string, AssetData> m_AssetDataCache = new Dictionary<string, AssetData>();

        private IAssetsLoader m_AssetsLoader;
        public IAssetsLoader AssetsLoader
        {
            get
            {
                if (this.m_AssetsLoader != null)
                    return this.m_AssetsLoader;

                if (AppConst.IsAssetBundle)
                    this.m_AssetsLoader = new AssetBundleLoader();
                else
                    this.m_AssetsLoader = new EditorAssetsLoader();

                return this.m_AssetsLoader;
            }
        }

        public UnityObject LoadAssets(string path)
        {
            this.LoadDependencies(path);

            var assetData = this.m_AssetDataCache[path];
            if (assetData == null)
            {
                // 尝试从缓存中获取
                if (!MemoryManger.Instance.TryGetAssetData(path, out assetData))
                    assetData = this.AssetsLoader.LoadAssets(path);

                this.m_AssetDataCache.Add(path, assetData);
            }

            if (assetData == null)
                return default;

            assetData.UpdateRefCount(1);
            return assetData.LoadAsset(path);
        }

        public T LoadAssets<T>(string path) where T : UnityObject
        {
            this.LoadDependencies(path);

            var assetData = this.m_AssetDataCache[path];
            if (assetData == null)
            {
                // 尝试从缓存中获取
                if (!MemoryManger.Instance.TryGetAssetData(path, out assetData))
                    assetData = this.AssetsLoader.LoadAssets<T>(path);

                this.m_AssetDataCache.Add(path, assetData);
            }

            if (assetData == null)
                return default(T);

            assetData.UpdateRefCount(1);
            return assetData.LoadAsset<T>(path);
        }

        public void LoadAssetAsync<T>(string path, System.Action<T> callback = null) where T : UnityObject
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadAssetIEnumerator<T>(path, callback));
        }

        public void FreeAsset(string path)
        {
            if (this.m_AssetDataCache.TryGetValue(path, out AssetData assetData))
            {
                assetData.UpdateRefCount(-1);
                if (assetData.RefCount == 0)
                {
                    // 添加到卸载队列
                    this.m_AssetDataCache.Remove(path);
                    MemoryManger.Instance.AddToNoUseCache(path, assetData);
                }
            }
        }

        private void LoadDependencies(string relativelyPath)
        {
            if (AppConst.IsAssetBundle)
            {
                var dependencies = ManifestManager.Instance.GetAssetBundleDependencies(relativelyPath);
                foreach (var bundleName in dependencies)
                    this.LoadAssets(bundleName);
            }
        }

        private IEnumerator LoadAssetIEnumerator<T>(string path, System.Action<T> callback = null) where T : UnityObject
        {
            yield return this.LoadDependenciesIEnumerator<T>(path);

            if (this.m_AssetDataCache.TryGetValue(path, out AssetData assetData))
            {
                assetData.UpdateRefCount(1);
                var asset = assetData.LoadAsset<T>(path);
                if (callback != null)
                    callback(asset);
            }
            else if (MemoryManger.Instance.TryGetAssetData(path, out assetData))
            {
                // 尝试从缓存中获取
                assetData.UpdateRefCount(1);
                this.m_AssetDataCache.Add(path, assetData);
                var asset = assetData.LoadAsset<T>(path);
                if (callback != null)
                    callback(asset);
            }
            else
            {
                yield return this.AssetsLoader.LoadAssetIEnumerator(path, (ret) => {
                    var asset = ret.LoadAsset<T>(path);
                    this.m_AssetDataCache.Add(path, ret);
                    ret.UpdateRefCount(1);
                    if (callback != null)
                        callback(asset);
                });
            }

            yield return 0;
        }

        private IEnumerator LoadDependenciesIEnumerator<T>(string path) where T : UnityObject
        {
            if (AppConst.IsAssetBundle)
            {
                var dependencies = ManifestManager.Instance.GetAssetBundleDependencies(path);
                foreach (var bundleName in dependencies)
                    yield return this.LoadAssetIEnumerator<T>(bundleName);
            }
        }
    }
}