using FrameWork.Core.Attributes;
using FrameWork.Core.Bootstrap;
using FrameWork.Core.Modules.Pool;
using FrameWork.Core.Service;
using FrameWork.Core.SingletonManager;
using Game.Config;
using System;
using System.Collections;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    //private static ApplicationManager s_Instance;
    //public static ApplicationManager Instance
    //{
    //    get
    //    {
    //        if (s_Instance == null)
    //            s_Instance = FindObjectOfType<ApplicationManager>();
    //        return s_Instance;
    //    }
    //}

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

    private GameServiceManager m_GameServiceManager;

    private IBootstrap m_Bootstrap;

    public static event Action s_OnGameStarted;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        this.m_GameServiceManager = GameServiceManager.Instance;

        AppConst.IsAssetBundle = this.IsAssetBundle;
        AppConst.AssetCacheCount = this.AssetCacheCount;
        AppConst.HotUpdateUrl = this.HotUpdateUrl;
        AppConst.AppVersionFileName = this.AppVersionFileName;
        AppConst.AppResourceListFileName = this.AppResourceListFileName;
        AppConst.DefaultGameObjectPool = new DefaultGameObjectPool();

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

        if (this.m_Bootstrap.Status == Status.Failed)
            yield break;

        this.m_GameServiceManager.CreateGameService<SceneService>().Initialize();

        yield return this.LaunchGame();

        s_OnGameStarted = null;
    }

    private IEnumerator LaunchGame()
    {
        yield return null;
        s_OnGameStarted?.Invoke();
    }
}
