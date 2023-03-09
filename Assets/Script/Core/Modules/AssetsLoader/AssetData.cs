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

        public int m_RefCount { get; private set; }

        public AssetData(string assetPath, AssetBundle assetBundle, UnityObject[] objects)
        {
            this.m_RefCount = 0;
            this.m_ObjectSize = -1;
            this.m_BundleSize = -1;
            this.m_AssetPath = assetPath;
            this.m_Objects = objects;
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

        public void Unload(bool unloadAllLoadedObjects = true)
        {
            if (this.m_AssetBundle != null)
                this.m_AssetBundle.Unload(unloadAllLoadedObjects);
        }

        public void UpdateRefCount(int count)
        {
            this.m_RefCount += count;
            if (this.m_RefCount < 0)
                Debug.LogError($"异常：资源引用计数为负数, AssetPath = {this.m_AssetPath}, RefCount = {this.m_RefCount}");
        }

        //public void LoadAllAssetsAsync(UnityAction<UnityObject[]> callBack = null)
        //{
        //    MonoBehaviourRuntime.Instance.StartCoroutine(LoadAllAssetsIEnumerator(callBack));
        //}

        //public void LoadAllAssetsAsync<T>(UnityAction<T[]> callBack = null) where T : UnityObject
        //{
        //    MonoBehaviourRuntime.Instance.StartCoroutine(LoadAllAssetsIEnumerator(callBack));
        //}

        public UnityObject LoadAsset<T>(string name) where T : UnityObject
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

        //public void LoadAssetAsync<T>(string name, UnityAction<UnityObject> callBack = null)
        //{
        //    MonoBehaviourRuntime.Instance.StartCoroutine(LoadAssetIEnumerator<T>(name, callBack));
        //}

        //private IEnumerator LoadAllAssetsIEnumerator(UnityAction<UnityObject[]> callBack = null)
        //{
        //    if (this.AssetBundle == null)
        //        throw new System.Exception();

        //    var request = this.AssetBundle.LoadAllAssetsAsync();
        //    yield return request;

        //    this.UpdateObjectsMemorySize(request.allAssets);
        //    var assets = request.allAssets;
        //    if (callBack != null)
        //        callBack(assets);
        //}

        //private IEnumerator LoadAllAssetsIEnumerator<T>(UnityAction<T[]> callBack = null)
        //{
        //    if (this.m_AssetBundle == null)
        //        throw new System.Exception();

        //    var request = this.m_AssetBundle.LoadAllAssetsAsync<T>();
        //    yield return request;

        //    this.UpdateObjectsMemorySize(request.allAssets);
        //    var assets = request.allAssets as T[];
        //    if (callBack != null)
        //        callBack(assets);
        //}

        //private IEnumerator LoadAssetIEnumerator<T>(string name, UnityAction<UnityObject> callBack = null)
        //{
        //    if (this.m_AssetBundle == null)
        //        throw new System.Exception();

        //    var request = this.m_AssetBundle.LoadAssetAsync<T>(name);
        //    yield return request;

        //    this.UpdateObjectsMemorySize(request.asset);
        //    if (callBack != null)
        //        callBack(request.asset);
        //}

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