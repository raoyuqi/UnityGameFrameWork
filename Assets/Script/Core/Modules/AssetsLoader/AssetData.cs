using Game.Config;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public sealed class AssetData
    {
        private long m_ObjectSize;
        private long m_BundleSize;

        private string m_AssetPath;
        private UnityObject[] m_Objects;
        private AssetBundle m_AssetBundle;

        private int m_RefCount;
        public int RefCount { get { return this.m_RefCount; } }

        public string[] ScenePaths { get; set; }

        public AssetData(string assetPath, UnityObject[] objects, AssetBundle assetBundle = null)
        {
            this.m_RefCount = 0;
            this.m_ObjectSize = -1;
            this.m_BundleSize = -1;
            this.m_AssetPath = assetPath;
            this.m_Objects = objects;
            if (AppConst.IsAssetBundle)
                this.m_AssetBundle = assetBundle;
        }

        public UnityObject[] LoadAllAssets()
        {
            return this.m_Objects;
        }

        public T[] LoadAllAssets<T>() where T : UnityObject
        {
            if (this.m_Objects == null)
                return default(T[]);

            // TODO: 这里每次都会创建一个list，可以优化
            List<UnityObject> list = new List<UnityObject>();
            foreach (var obj in m_Objects)
            {
                if (obj.GetType() == typeof(T))
                    list.Add(obj);
            }
            return list.ToArray() as T[];
        }

        public string[] GetAllScenePaths()
        {
            if (this.ScenePaths != null && this.ScenePaths.Length > 0)
                return this.ScenePaths;

            if (this.m_AssetBundle != null)
                this.ScenePaths = this.m_AssetBundle.GetAllScenePaths();

            return this.ScenePaths;
        }

        public void Unload(bool unloadAllLoadedObjects = true)
        {
            if (this.m_AssetBundle != null)
                this.m_AssetBundle.Unload(unloadAllLoadedObjects);
        }

        public void UpdateRefCount(int count)
        {
            this.m_RefCount += count;
            if (this.m_RefCount < 0)
            {
                this.m_RefCount = 0;
                Debug.LogError($"异常：资源引用计数为负数, AssetPath = {this.m_AssetPath}, RefCount = {this.m_RefCount}");
            }

            //var tip = count > 0 ? "加载资源" : "卸载资源";
            //Debug.Log($"{ tip }: path = { this.m_AssetPath }, RefCount = { this.m_RefCount } ");
        }

        public UnityObject LoadAsset(string name)
        {
            if (this.m_Objects == null)
                return default;

            foreach (var obj in this.m_Objects)
            {
                if (obj.name == name)
                    return obj;
            }
            return default;
        }

        public T LoadAsset<T>(string name) where T : UnityObject
        {
            if (this.m_Objects == null)
                return default(T);

            foreach (var obj in this.m_Objects)
            {
                if (obj.GetType() == typeof(T))
                    return (T)obj;
            }
            return default(T);
        }

        /// <summary>
        /// 获取资源占用内存
        /// </summary>
        public long GetObjectMemorySize()
        {
            if (this.m_Objects == null)
                return 0;

            if (this.m_ObjectSize >= 0)
                return this.m_ObjectSize;

            if (this.m_ObjectSize < 0)
            {
                this.m_ObjectSize = 0;
                foreach (var obj in this.m_Objects)
                    this.m_ObjectSize += Profiler.GetRuntimeMemorySizeLong(obj);
            }
            return this.m_ObjectSize;
        }

        /// <summary>
        /// 获取Bundle占用内存
        /// </summary>
        /// <returns></returns>
        public long GetBundleMemorySize()
        {
            if (this.m_AssetBundle == null)
                return 0;

            if (this.m_BundleSize >= 0)
                return this.m_BundleSize;

            this.m_BundleSize = Profiler.GetRuntimeMemorySizeLong(this.m_AssetBundle);
            return this.m_BundleSize;
        }

        /// <summary>
        /// 获取总内存
        /// </summary>
        /// <returns></returns>
        public long GetTotalMemorySize()
        {
            return this.GetBundleMemorySize() + this.GetObjectMemorySize();
        }
    }
}