using FrameWork.Core.Mixin;
using FrameWork.Core.Modules.AssetsLoader;
using FrameWork.Core.Utils;
using Game.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

                //#if UNITY_EDITOR
                //        this.m_AssetsLoader = new EditorAssetsLoader();
                //#else
                //        this.m_AssetsLoader = new AssetBundleLoader();
                //#endif

                return this.m_AssetsLoader;
            }
        }

        public UnityObject LoadAssets(string path)
        {
            var assetData = this.m_AssetDataCache[path];
            if (assetData == null)
            {
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
            var assetData = this.m_AssetDataCache[path];
            if (assetData == null)
            {
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
            if (this.m_AssetDataCache.TryGetValue(path, out AssetData assetData))
            {
                assetData.UpdateRefCount(1);
                var asset = assetData.LoadAsset<T>(path);
                if (callback != null)
                    callback(asset);
                return;
            }

            this.AssetsLoader.LoadAssetAsync<T>(path, (ret) =>
            {
                var asset = ret.LoadAsset<T>(path);
                this.m_AssetDataCache.Add(path, ret);
                ret.UpdateRefCount(1);
                if (callback != null)
                    callback(asset);
            });
        }
    }
}