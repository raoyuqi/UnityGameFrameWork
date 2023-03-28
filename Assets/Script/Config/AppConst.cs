using FrameWork.Core.Modules.Pool;

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
    }
}