using System;
using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public sealed class AssetBundleLoader : IAssetsLoader
    {
        public UnityObject LoadAssets(string path)
        {
            var assetBundle = this.LoadAssetBundle(path);
            if (assetBundle != null)
            {
                
            }
            // TODO: 返回一个AssetData对象
            return null;
        }

        public T LoadAssets<T>(string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public IEnumerator LoadAssetsAsync<T>(string path, Action<T> callback = null) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
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

