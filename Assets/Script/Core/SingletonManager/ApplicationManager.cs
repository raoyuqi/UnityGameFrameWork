using FrameWork.Core.Attributes;
using FrameWork.Core.Bootstrap;
using FrameWork.Core.Modules.Pool;
using FrameWork.Core.SingletonManager;
using Game.Config;
using System;
using System.Collections;
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

    private bool m_IsEditor
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    private IBootstrap m_Bootstrap;

    public event Action OnGameStarted;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        AppConst.IsAssetBundle = this.IsAssetBundle;
        AppConst.AssetCacheCount = this.AssetCacheCount;
        AppConst.HotUpdateUrl = this.HotUpdateUrl;
        AppConst.AppVersionFileName = this.AppVersionFileName;
        AppConst.AppResourceListFileName = this.AppResourceListFileName;
        AppConst.UIGameObjectPool = new DefaultGameObjectPool();

        if (!this.m_IsEditor)
            AppConst.IsAssetBundle = true;

        MemoryManger.Instance.Initialize();

        if (this.IsAssetBundle)
            this.m_Bootstrap = new MobileBootstrap();
        else
            this.m_Bootstrap = new EditorBootstrap();
    }

    private void Start()
    {
        this.StartCoroutine(this.Bootstrap());
    }

    private IEnumerator Bootstrap()
    {
        yield return this.m_Bootstrap.BootstrapAsync();

        yield return this.LaunchGame();
    }

    private IEnumerator LaunchGame()
    {
        yield return null;
        this.OnGameStarted?.Invoke();
        Debug.Log("进入游戏，加载主场景");
    }
}
