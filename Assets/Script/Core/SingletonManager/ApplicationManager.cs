using FrameWork.Core.Attributes;
using FrameWork.Core.Modules.Pool;
using FrameWork.Core.SingletonManager;
using Game.Config;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    [SerializeField, LabelText("使用AssetBundle")]
    private bool IsAssetBundle;

    [SerializeField, LabelText("引用为0的资源缓存数量")]
    private int AssetCacheCount;

    [SerializeField, LabelText("热更地址")]
    private string HotUpdateUrl;

    [SerializeField, LabelText("热更版本信息文件名")]
    private string AppVersionFileName;

    [SerializeField, LabelText("资源清单文件名")]
    private string AppResourceListFileName;

    private void Awake()
    {
        AppConst.IsAssetBundle = this.IsAssetBundle;
        AppConst.AssetCacheCount = this.AssetCacheCount;
        AppConst.HotUpdateUrl = this.HotUpdateUrl;
        AppConst.AppVersionFileName = this.AppVersionFileName;
        AppConst.AppResourceListFileName = this.AppResourceListFileName;
        AppConst.UIGameObjectPool = new DefaultGameObjectPool();

#if !UNITY_EDITOR
        AppConst.IsAssetBundle = true;
#endif

        this.DoAppLaunch();
    }

    private void DoAppLaunch()
    {
        MemoryManger.Instance.Initialize();

        // 测试
        //this.StartCoroutine(HotUpdateHandler.Instance.CheckVersion());
        HotUpdateHandler.Instance.StartHotUpdateProcess();
    }
}
