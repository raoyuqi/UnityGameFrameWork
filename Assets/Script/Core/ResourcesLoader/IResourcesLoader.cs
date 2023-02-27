﻿using UnityEngine;

namespace FrameWork.Core.ResourcesLoader
{
    public interface IResourcesLoader
    {
        Object LoadAssets(string path);

        T LoadAssets<T>(string path) where T : Object;
    }
}