using System.Collections;
using UnityEngine;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public interface IAssetsLoader
    {
        Object LoadAssets(string path);

        T LoadAssets<T>(string path) where T : Object;

        IEnumerator LoadAssetsAsync<T>(string path, System.Action<T> callback = null) where T : Object;
    }
}