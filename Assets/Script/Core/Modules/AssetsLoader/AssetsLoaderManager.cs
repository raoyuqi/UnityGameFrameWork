using FrameWork.Core.Modules.AssetsLoader;
using FrameWork.Core.SingletonManager;
using Game.Config;
using System.Collections;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.AssetsLoader
{
    public sealed class AssetsLoaderManager
    {
        private Dictionary<string, AssetData> m_AssetDataCache;

        private ManifestManager m_ManifestManager;

        public IAssetsLoader AssetsLoader { get; set; }

        public AssetsLoaderManager()
        {
            this.m_AssetDataCache = new Dictionary<string, AssetData>();
            this.m_ManifestManager = new ManifestManager();
        }

        public AssetData LoadAsset(string path)
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
            return assetData;
        }

        public AssetData LoadAsset<T>(string path) where T : UnityObject
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
                return default;

            assetData.UpdateRefCount(1);
            return assetData;
        }

        public void LoadAssetAsync<T>(string path, System.Action<AssetData> callback = null) where T : UnityObject
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadAssetIEnumerator<T>(path, callback));
        }

        public void LoadAssetAsync(string path, System.Action<AssetData> callback = null)
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadAssetIEnumerator(path, callback));
        }

        public IEnumerator LoadSceneAsync(string path, System.Action<AssetData> callback = null)
        {
            yield return this.LoadAssetIEnumerator(path, callback, true);
        }

        public void FreeAsset(string path)
        {
            this.FreeDependencies(path);

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
                var dependencies = this.m_ManifestManager.GetAssetBundleDependencies(relativelyPath);
                foreach (var bundleName in dependencies)
                    this.LoadAsset(bundleName);
            }
        }

        private IEnumerator LoadAssetIEnumerator(string path, System.Action<AssetData> callback = null, bool isScene = false)
        {
            yield return this.LoadDependenciesIEnumerator(path);

            if (this.m_AssetDataCache.TryGetValue(path, out AssetData assetData))
            {
                assetData.UpdateRefCount(1);
                if (callback != null)
                    callback(assetData);
            }
            else if (MemoryManger.Instance.TryGetAssetData(path, out assetData))
            {
                // 尝试从缓存中获取
                assetData.UpdateRefCount(1);
                this.m_AssetDataCache.Add(path, assetData);
                if (callback != null)
                    callback(assetData);
            }
            else
            {
                if (!isScene)
                {
                    yield return this.AssetsLoader.LoadAssetAsync(path, (ret) => {
                        this.m_AssetDataCache.Add(path, ret);
                        ret.UpdateRefCount(1);
                        if (callback != null)
                            callback(ret);
                    });
                }
                else
                {
                    yield return this.AssetsLoader.LoadSceneIAsync(path, (ret) => {
                        //this.m_AssetDataCache.Add(path, ret);
                        //ret.UpdateRefCount(1);
                        if (callback != null)
                            callback(ret);
                    });
                }
            }

            yield return 0;
        }

        private IEnumerator LoadAssetIEnumerator<T>(string path, System.Action<AssetData> callback = null) where T : UnityObject
        {
            yield return this.LoadAssetIEnumerator(path, callback);
        }

        private IEnumerator LoadDependenciesIEnumerator(string path)
        {
            if (AppConst.IsAssetBundle)
            {
                var dependencies = this.m_ManifestManager.GetAssetBundleDependencies(path);
                foreach (var bundleName in dependencies)
                    yield return this.LoadAssetIEnumerator(bundleName);
            }
        }

        private void FreeDependencies(string relativelyPath)
        {
            if (AppConst.IsAssetBundle)
            {
                var dependencies = this.m_ManifestManager.GetAssetBundleDependencies(relativelyPath);
                foreach (var bundleName in dependencies)
                    this.FreeAsset(bundleName);
            }
        }
    }
}