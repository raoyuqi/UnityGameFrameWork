using FrameWork.Core.Mathf_;
using FrameWork.Core.Mixin;
using FrameWork.Core.Modules.AssetsLoader;
using Game.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Manager
{
    /// <summary>
    /// 内存管控
    /// </summary>
    public sealed class MemoryManger : SingletonBase<MemoryManger>
    {
        private LRUCache<AssetData> m_NoUseAssetCache;
        private List<string> m_NoUseAssetPath;

        //public delegate void FreeMemoryCallback(AssetData assetData);
        //public event FreeMemoryCallback FreeMemory;

        public MemoryManger()
        {
            this.m_NoUseAssetCache = new LRUCache<AssetData>(AppConst.AssetCacheCount);
            this.m_NoUseAssetPath = new List<string>();
            this.m_NoUseAssetCache.FreeOldestNodeCallBack += ((assetData) =>
            {
                assetData.Unload();
            });
        }

        public void Initialize()
        {
            Application.lowMemory += OnLowMemoryCallBack;
        }

        public void AddToNoUseCache(string path, AssetData assetData)
        {
            this.m_NoUseAssetCache.Put(path, assetData);
            if (!this.m_NoUseAssetPath.Contains(path))
                this.m_NoUseAssetPath.Add(path);
        }

        public bool TryGetAssetData(string path, out AssetData assetData)
        {
            assetData = this.m_NoUseAssetCache.Get(path);
            if (assetData == null)
                return false;

            this.m_NoUseAssetCache.Remove(path);
            if (this.m_NoUseAssetPath.Contains(path))
                this.m_NoUseAssetPath.Remove(path);
            return true;
        }

        private void OnLowMemoryCallBack()
        {
            foreach (var path in this.m_NoUseAssetPath)
            {
                var assetData = this.m_NoUseAssetCache.Get(path);
                assetData.Unload();
            }
            this.m_NoUseAssetCache.Clean();

            AppConst.UIGameObjectPool.Clean();
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}