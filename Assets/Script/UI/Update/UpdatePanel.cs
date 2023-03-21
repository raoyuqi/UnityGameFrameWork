using FrameWork.Core.Attributes;
using FrameWork.Core.HotUpdate;
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
        HotUpdateHandler.Instance.UpdateProgressCallback += OnHotUpdateProgress;
    }

    private void OnHotUpdateProgress(HotUpdateCallbackInfo info)
    {
        Debug.Log("--------------更新进度条: " + info.CurProgress + info.UpdateTip + info.TotalProgress);
        this.m_TipText.text = info.UpdateTip;
        this.m_RateText.text = $"{ info.CurProgress }kb/{ info.TotalProgress }kb";
        this.m_ImgProgress.fillAmount = info.CurProgress / info.TotalProgress;
    }
}
