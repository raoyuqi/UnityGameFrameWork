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
    }
}