using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Utils
{
    public static class GameUtils
    {
        public static void GetScreenDimensions(out int width, out int height)
        {
#if UNITY_EDITOR
            width = Screen.width;
            height = Screen.height;
#else
            var resolution = Screen.currentResolution;
            width = resolution.width;
            height = resolution.height;
#endif
        }
    }
}