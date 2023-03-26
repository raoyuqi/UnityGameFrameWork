using FrameWork.Core.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public sealed class AssetBundleLoader : IAssetsLoader
    {
        public AssetData LoadAssets(string relativelyPath)
        {
            var path = PathTool.GetAssetAbsolutePath(relativelyPath);
            var assetBundle = this.LoadAssetBundle(path);
            if (assetBundle == null)
                return default(AssetData);

            var assets = assetBundle.LoadAllAssets();
            var assetData = new AssetData(relativelyPath, assets, assetBundle);
            return assetData;
        }

        public AssetData LoadAssets<T>(string relativelyPath) where T : UnityEngine.Object
        {
            return this.LoadAssets(relativelyPath);
        }

        //public void LoadAssetAsync(string relativelyPath, Action<AssetData> callback = null)
        //{
        //    MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadAssetIEnumerator(relativelyPath, callback));
        //}

        //public void LoadAssetAsync<T>(string relativelyPath, Action<AssetData> callback = null) where T : UnityEngine.Object
        //{
        //    MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadAssetIEnumerator(relativelyPath, callback));
        //}

        public IEnumerator LoadAssetAsync(string relativelyPath, Action<AssetData> callback = null)
        {
            var path = PathTool.GetAssetAbsolutePath(relativelyPath);
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;

            var assetBundle = bundleRequest.assetBundle;
            if (assetBundle == null)
            {
                Debug.LogError($"AssetBundle 不存在, path = {relativelyPath}");
                yield break;
            }

            var assetRequest = assetBundle.LoadAllAssetsAsync();
            yield return assetRequest;

            var assets = assetRequest.allAssets;
            var assetData = new AssetData(relativelyPath, assets, assetBundle);
            if (callback != null)
                callback(assetData);
        }

        public IEnumerator LoadSceneIAsync(string relativelyPath, Action<AssetData> callback = null)
        {
            var path = PathTool.GetAssetAbsolutePath(relativelyPath);
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;

            var assetBundle = bundleRequest.assetBundle;
            if (assetBundle == null)
            {
                Debug.LogError($"AssetBundle 不存在, path = {relativelyPath}");
                yield break;
            }

            var assetData = new AssetData(relativelyPath, null, assetBundle);
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

