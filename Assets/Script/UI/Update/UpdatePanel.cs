using FrameWork.Core.Attributes;
using FrameWork.Core.HotUpdate;
using FrameWork.Core.Modules.Signal;
using Game.Scene;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePanel : MonoBehaviour
{
    [SerializeField, LabelText("更新提示文本")]
    private Text m_TipText;

    [SerializeField, LabelText("下载百分比")]
    private Text m_RateText;

    [SerializeField, LabelText("进度条")]
    private Image m_ImgProgress;

    private void Awake()
    {
        HotUpdateHandler.Instance.NotUpdateCallback += OnNotUpdateHandler;
        HotUpdateHandler.Instance.ForceUpdateCallback += OnForceUpdateHandler;
        HotUpdateHandler.Instance.UpdateProgressCallback += OnHotUpdateProgressHandler;
        HotUpdateHandler.Instance.UpdateSuccessCallback += OnHotUpdateSuccessHandler;
        HotUpdateHandler.Instance.UpdateFailedCallback += OnHotUpdateFailedHandler;

        ApplicationManager.s_OnGameStarted += OnGameStarted;
    }

    private void OnDestroy()
    {
        HotUpdateHandler.Instance.Dispose();
    }

    private void OnForceUpdateHandler()
    {
        // TODO: 强更逻辑
    }

    private void OnHotUpdateProgressHandler(HotUpdateCallbackInfo info)
    {
        this.m_TipText.text = "更新中...";
        this.m_RateText.text = $"{ info.CurProgress }kb/{ info.TotalProgress }kb";
        this.m_ImgProgress.fillAmount = info.CurProgress / info.TotalProgress;
    }

    private void OnNotUpdateHandler()
    {
        this.m_RateText.gameObject.SetActive(false);
        this.m_TipText.text = "无需更新";
        this.m_ImgProgress.fillAmount = 1;
    }

    private void OnHotUpdateSuccessHandler()
    {
        this.m_TipText.text = "更新完成";
        this.m_ImgProgress.fillAmount = 1;
    }

    private void OnHotUpdateFailedHandler(string msg)
    {
        // TODO: 弹窗提示更新失败
        Debug.LogError($"热更新失败提示: { msg }");
    }

    private void OnGameStarted()
    {
        // 进入游戏，加载主场景
        var scene = new MainScene() { Name = "Main" };
        GlobalSignalSystem.Instance.RaiseSignal(GlobalSignal.TransScene, scene);
    }
}
