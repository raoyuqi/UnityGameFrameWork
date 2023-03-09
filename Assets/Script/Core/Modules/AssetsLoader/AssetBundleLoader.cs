using FrameWork.Core.Manager;
using System;
using System.Collections;
using UnityEngine;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public sealed class AssetBundleLoader : IAssetsLoader
    {
        public AssetData LoadAssets(string path)
        {
            var assetBundle = this.LoadAssetBundle(path);
            if (assetBundle == null)
                return default(AssetData);

            var assets = assetBundle.LoadAllAssets();
            var assetData = new AssetData(path, assetBundle, assets);
            return assetData;
        }

        public AssetData LoadAssets<T>(string path) where T : UnityEngine.Object
        {
            return this.LoadAssets(path);
        }

        public void LoadAssetsAsync(string path, Action<AssetData> callback = null)
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadAssetsIEnumerator(path, callback));
        }

        private IEnumerator LoadAssetsIEnumerator(string path, Action<AssetData> callback = null)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;

            var assetBundle = bundleRequest.assetBundle;
            if (assetBundle == null)
            {
                Debug.LogError($"AssetBundle 不存在, path = {path}");
                yield break;
            }

            var assetRequest = assetBundle.LoadAllAssetsAsync();
            yield return assetRequest;

            var assets = assetRequest.allAssets;
            var assetData = new AssetData(path, assetBundle, assets);
            if (callback != null)
                callback(assetData);
        }

        private AssetBundle LoadAssetBundle(string path)
        {
            var assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
            {
                Debug.LogError("Failed to load AssetBundle!");
                return null;
            }

            return assetBundle;
        }
    }
}

