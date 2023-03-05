using FrameWork.Core.Manager;
using FrameWork.Core.Modules.AssetsLoader;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    // 资源管理器
    public static AssetsLoaderManager AssetsLoaderManager;

    private void Awake()
    {
        AssetsLoaderManager.Instance.AssetsLoader = new EditorAssetsLoader();

    }
}
