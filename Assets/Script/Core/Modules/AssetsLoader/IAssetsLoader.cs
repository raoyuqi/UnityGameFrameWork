using UnityEngine;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public interface IAssetsLoader
    {
        Object LoadAssets(string path);

        T LoadAssets<T>(string path) where T : Object;
    }
}