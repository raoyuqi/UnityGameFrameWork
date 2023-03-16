using FrameWork.Core.Modules.Pool;

namespace Game.Config
{
    public static class AppConst
    {
        public static bool IsAssetBundle;

        public static int AssetCacheCount;

        // UI使用的对象池
        public static IGameObjectPool UIGameObjectPool;

        public static string AppVersionKey = "app_version";

        // 热更地址
        public static string HotUpdateUrl;
        // 热更版本信息文件名
        public static string AppVersionFileName;
        // 资源清单文件名
        public static string AppResourceListFileName;
    }
}