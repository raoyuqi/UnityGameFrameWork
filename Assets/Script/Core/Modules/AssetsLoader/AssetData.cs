using FrameWork.Core.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public sealed class AssetData
    {
        private Dictionary<string, UnityObject> m_LoadedAssetsName = new Dictionary<string, UnityObject>();

        private int m_RefCount = 0;

        private long m_ObjectSize = 0;
        private long m_BundleSize = 0;

        public AssetBundle AssetBundle { get; set; }

        public UnityObject[] LoadAllAssets()
        {
            if (this.AssetBundle == null)
                return default;

            var objects = this.AssetBundle.LoadAllAssets();
            return objects;
        }

        public T[] LoadAllAssets<T>() where T : UnityObject
        {
            if (this.AssetBundle == null)
                return default;

            return this.AssetBundle.LoadAllAssets<T>();
        }

        public void LoadAllAssetsAsync(UnityAction<UnityObject[]> callBack = null)
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(LoadAllAssetsIEnumerator(callBack));
        }

        public void LoadAllAssetsAsync<T>(UnityAction<T[]> callBack = null) where T : UnityObject
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(LoadAllAssetsIEnumerator(callBack));
        }

        public UnityObject LoadAsset<T>(string name) where T : UnityObject
        {
            if (this.AssetBundle == null)
                return default;

            var asset = this.AssetBundle.LoadAsset<T>(name);
            this.UpdateObjectsMemorySize(asset);
            return asset;
        }

        public void LoadAssetAsync<T>(string name, UnityAction<UnityObject> callBack = null)
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(LoadAssetIEnumerator<T>(name, callBack));
        }

        private IEnumerator LoadAllAssetsIEnumerator(UnityAction<UnityObject[]> callBack = null)
        {
            if (this.AssetBundle == null)
                throw new System.Exception();

            var request = this.AssetBundle.LoadAllAssetsAsync();
            yield return request;

            this.UpdateObjectsMemorySize(request.allAssets);
            var assets = request.allAssets;
            if (callBack != null)
                callBack(assets);
        }

        private IEnumerator LoadAllAssetsIEnumerator<T>(UnityAction<T[]> callBack = null)
        {
            if (this.AssetBundle == null)
                throw new System.Exception();

            var request = this.AssetBundle.LoadAllAssetsAsync<T>();
            yield return request;

            this.UpdateObjectsMemorySize(request.allAssets);
            var assets = request.allAssets as T[];
            if (callBack != null)
                callBack(assets);
        }

        private IEnumerator LoadAssetIEnumerator<T>(string name, UnityAction<UnityObject> callBack = null)
        {
            if (this.AssetBundle == null)
                throw new System.Exception();

            var request = this.AssetBundle.LoadAssetAsync<T>(name);
            yield return request;

            this.UpdateObjectsMemorySize(request.asset);
            if (callBack != null)
                callBack(request.asset);
        }

        private void UpdateObjectsMemorySize(UnityObject[] objects)
        {
            foreach (var obj in objects)
            {
                if (!this.m_LoadedAssetsName.ContainsKey(obj.name))
                {
                    //this.m_LoadedAssetsName.Add(obj.name, true);
                    this.m_ObjectSize += Profiler.GetRuntimeMemorySizeLong(obj);
                }
            }
        }

        private void UpdateObjectsMemorySize(UnityObject obj)
        {
            if (obj != null && !this.m_LoadedAssetsName.ContainsKey(obj.name))
            {
                //this.m_LoadedAssetsName.Add(obj.name, true);
                this.m_ObjectSize += Profiler.GetRuntimeMemorySizeLong(obj);
            }
        }
    }
}