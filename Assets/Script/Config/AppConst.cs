using FrameWork.Core.Modules.Pool;
using UnityEngine;

namespace Game.Config
{
    public static class AppConst
    {
        public static bool IsAssetBundle;

        public static int AssetCacheCount;

        // 默认使用的对象池 TODO: 转移到模块管理里面
        public static IGameObjectPool DefaultGameObjectPool;

        public static string AppVersionKey = "app_version";

        // 热更地址
        public static string HotUpdateUrl;
        // 热更版本信息文件名
        public static string AppVersionFileName;
        // 资源清单文件名
        public static string AppResourceListFileName;

        // UI 使用的基准分辨率
        public static readonly Vector2 ReferenceResolution = new Vector2(1920, 1080);

        // UI 使用的基准宽高比，大于这个值的设备会通过添加填充适配到这个比值上
        // 超过该比值一般为异形屏，需要适配
        public const float ReferenceAspectRatio = 2.05f;

        // 支持的最高宽高比，少数设备的分辨率，如：折叠机
        public const float MaxAspectRatio = 22f / 9;
    }
}