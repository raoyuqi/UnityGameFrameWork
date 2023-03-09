using System.Collections;
using UnityEngine;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public interface IAssetsLoader
    {
        AssetData LoadAssets(string path);

        AssetData LoadAssets<T>(string path) where T : Object;

        void LoadAssetsAsync(string path, System.Action<AssetData> callback = null);
    }
}