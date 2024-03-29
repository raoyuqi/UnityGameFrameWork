﻿using System.Collections;
using UnityEngine;

namespace FrameWork.Core.Modules.AssetsLoader
{
    public interface IAssetsLoader
    {
        AssetData LoadAssets(string path);

        AssetData LoadAssets<T>(string path) where T : Object;

        //void LoadAssetAsync<T>(string path, System.Action<AssetData> callback = null) where T : Object;

        IEnumerator LoadAssetAsync(string path, System.Action<AssetData> callback = null);

        IEnumerator LoadSceneIAsync(string path, System.Action<AssetData> callback = null);
    }
}