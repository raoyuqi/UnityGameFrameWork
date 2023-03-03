using FrameWork.Core.Mixin;
using FrameWork.Core.Modules.AssetsLoader;
using UnityEngine;

namespace FrameWork.Core.Manager
{
    // TODO: 更新引用计数
    public sealed class AssetsLoaderManager : SingletonBase<AssetsLoaderManager>
    {
        private IAssetsLoader m_AssetsLoader;
        public IAssetsLoader AssetsLoader
        {
            get { return this.m_AssetsLoader; }
            set
            {
                if (this.m_AssetsLoader != null)
                    throw new System.Exception("资源加载器初始化后，不允许修改！");

                this.m_AssetsLoader = value;
            }
        }

        public Object LoadAssets(string path)
        {
            return this.AssetsLoader.LoadAssets(path);
        }

        public T LoadAssets<T>(string path) where T : Object
        {
            return this.AssetsLoader.LoadAssets<T>(path);
        }

        public void LoadAssetsAsync<T>(string path, System.Action<T> callback = null) where T : Object
        {
            this.AssetsLoader.LoadAssetsAsync<T>(path, callback);
        }
    }
}