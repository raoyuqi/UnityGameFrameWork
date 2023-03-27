using FrameWork.Core.AssetsLoader;
using FrameWork.Core.Bootstrap;
using FrameWork.Core.Mixin;
using FrameWork.Core.Modules.AssetsLoader;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.SingletonManager
{
    public class ResourceManager : SingletonBase<ResourceManager>
    {
        private AssetsLoaderManager m_AssetsLoaderManager;

        public void Initialize(BootstrapMode mode)
        {
            if (mode == BootstrapMode.Editor)
                this.m_AssetsLoaderManager = new AssetsLoaderManager() { AssetsLoader = new EditorAssetsLoader() };
            else
                this.m_AssetsLoaderManager = new AssetsLoaderManager() { AssetsLoader = new AssetBundleLoader() };
        }

        public UnityObject Load(string path)
        {
            var assetData = this.m_AssetsLoaderManager.LoadAsset(path);
            if (assetData == null)
                return default;

            return assetData.LoadAsset(path);
        }

        public T Load<T>(string path) where T : UnityObject
        {
            var assetData = this.m_AssetsLoaderManager.LoadAsset<T>(path);
            if (assetData == null)
                return default;

            return assetData.LoadAsset<T>(path);
        }

        public void LoadAsync<T>(string path, Action<T> callback = null) where T : UnityObject
        {
            this.m_AssetsLoaderManager.LoadAssetAsync<T>(path, (assetData) => {
                T asset = null;
                if (assetData != null)
                    asset = assetData.LoadAsset<T>(path);

                if (callback != null)
                    callback(asset);
            });
        }

        public IEnumerator LoadSceneAsync(string path, Action<float> callback = null, LoadSceneMode sceneMode = LoadSceneMode.Single)
        {
            var scenePath = "";
            yield return this.m_AssetsLoaderManager.LoadSceneAsync(path, (assetData) => {
                var paths = assetData.GetAllScenePaths();
                if (paths != null && paths.Length > 0)
                    scenePath = paths[0];
            });

            if (string.IsNullOrEmpty(scenePath))
                yield break;

            // 测试待删除
            //for (int i = 0; i < 1000; i++)
            //{
            //    yield return new WaitForEndOfFrame();
            //    if (callback != null)
            //    {
            //        var progress = Math.Round((float)i/1000, 2);
            //        callback((float)progress);
            //    }
            //}

            var asyncOperation = SceneManager.LoadSceneAsync(scenePath, sceneMode);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                if (callback != null)
                {
                    var progress = Math.Round(asyncOperation.progress, 2);
                    callback((float)progress);
                }

                if (Mathf.Approximately(asyncOperation.progress, 0.9f))
                {
                    asyncOperation.allowSceneActivation = true;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            //var scene = SceneManager.GetSceneByPath(scenePath);

            yield return asyncOperation;

            if (callback != null)
                callback(1);
        }

        public SpriteAtlas LoadSpriteAtlas(string path)
        {
            return this.Load<SpriteAtlas>(path);
        }

        public void LoadSpriteAtlasAsync(string path, Action<SpriteAtlas> callback = null)
        {
            this.LoadAsync<SpriteAtlas>(path, (spriteAtlas) =>
            {
                if (callback != null)
                    callback(spriteAtlas);
            });
        }

        public Texture LoadTexture(string path)
        {
            return this.Load<Texture>(path);
        }

        public void LoadTextureAsync(string path, Action<Texture> callback = null)
        {
            this.LoadAsync<Texture>(path, (texture) =>
            {
                if (callback != null)
                    callback(texture);
            });
        }

        public void FreeRefCount(string path)
        {
            this.m_AssetsLoaderManager.FreeAsset(path);
        }

        public void FreeRefCountByName(string name)
        {
            if (UIAssetsConfig.PathConfit.TryGetValue(name, out string path))
                this.m_AssetsLoaderManager.FreeAsset(path);
        }
    }
}

