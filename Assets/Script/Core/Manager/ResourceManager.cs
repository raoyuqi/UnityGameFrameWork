using FrameWork.Core.Mixin;
using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.Manager
{
    public class ResourceManager : SingletonBase<ResourceManager>
    {
        private AssetsLoaderManager m_AssetsLoaderManager;

        public ResourceManager()
        {
            this.m_AssetsLoaderManager = AssetsLoaderManager.Instance;
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

