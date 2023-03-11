using FrameWork.Core.Attributes;
using FrameWork.Core.Manager;
using FrameWork.Core.Modules.Pool;
using Game.Config;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    // 资源管理器
    public static AssetsLoaderManager AssetsLoaderManager;

    [SerializeField, LabelText("使用AssetBundle")]
    private bool IsAssetBundle;

    [SerializeField, LabelText("引用为0的资源缓存数量")]
    private int AssetCacheCount;

    private void Awake()
    {
        AppConst.IsAssetBundle = this.IsAssetBundle;
        AppConst.AssetCacheCount = this.AssetCacheCount;
        AppConst.UIGameObjectPool = new DefaultGameObjectPool();

#if !UNITY_EDITOR
        AppConst.IsAssetBundle = true;
#endif

        this.DoAppLaunch();
    }

    private void DoAppLaunch()
    {
        MemoryManger.Instance.Initialize();
    }
}
