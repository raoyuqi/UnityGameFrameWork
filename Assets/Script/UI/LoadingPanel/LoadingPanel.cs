using FrameWork.Core.Attributes;
using FrameWork.Core.Service;
using FrameWork.Core.SingletonManager;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField, LabelText("加载场景提示文本")]
    private Text m_TipText;

    [SerializeField, LabelText("加载百分比")]
    private Text m_RateText;

    [SerializeField, LabelText("加载进度条")]
    private Image m_ImgProgress;

    private SceneService m_SceneService;

    private void Awake()
    {
        this.m_SceneService = GameServiceManager.Instance.GetGameService<SceneService>();
        this.m_SceneService.OnLoadSceneProgress += OnLoadSceneProgressHandler;
    }

    private void OnDestroy()
    {
        this.m_SceneService.Dispose();
        this.m_SceneService = null;
    }

    private void OnLoadSceneProgressHandler(float progress)
    {
        this.m_TipText.text = "场景加载中...";
        this.m_RateText.text = $"{ progress * 100 }%";
        this.m_ImgProgress.fillAmount = progress;
    }
}
