using FrameWork.Core.Attributes;
using FrameWork.Core.Manager;
using FrameWork.Core.Modules.AssetsLoader;
using Game.Config;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    // 资源管理器
    public static AssetsLoaderManager AssetsLoaderManager;

    [SerializeField, LabelText("使用AssetBundle")]
    private bool IsAssetBundle;

    private void Awake()
    {
        AppConst.IsAssetBundle = this.IsAssetBundle;
        if (this.IsAssetBundle)
            AssetsLoaderManager.Instance.AssetsLoader = new AssetBundleLoader();
        else
            AssetsLoaderManager.Instance.AssetsLoader = new EditorAssetsLoader();

        //#if UNITY_EDITOR
        //        AssetsLoaderManager.Instance.AssetsLoader = new EditorAssetsLoader();
        //#else
        //        AssetsLoaderManager.Instance.AssetsLoader = new AssetBundleLoader();
        //#endif
        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //    AssetsLoaderManager.Instance.AssetsLoader = new AssetBundleLoader();

        //#if !UNITY_EDITOR
        //    AssetsLoaderManager.Instance.AssetsLoader = new AssetBundleLoader();
        //#endif
    }
}
