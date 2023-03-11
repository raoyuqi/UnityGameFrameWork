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

#if !UNITY_EDITOR
        AppConst.IsAssetBundle = true;
#endif
        
    }
}
