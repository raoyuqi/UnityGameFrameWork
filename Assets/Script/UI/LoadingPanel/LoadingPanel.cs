using FrameWork.Core.Modules.UI;
using FrameWork.Core.Service;
using FrameWork.Core.SingletonManager;
using System;
using UnityEngine;

public class LoadingPanel : UIPanelBase
{
    private SceneService m_SceneService;

    public override void OnInit()
    {
        Debug.Log("loading界面初始化");
        this.m_SceneService = GameServiceManager.Instance.GetGameService<SceneService>();
        this.m_SceneService.OnLoadSceneProgress += OnLoadSceneProgressHandler;
    }

    public override void OnOpen()
    {
        Debug.Log("loading界面已打开");
        base.GetText("Text_Rate").text = "0%";
    }

    // TODO: 重写基类的方法
    private void OnDestroy()
    {
        this.m_SceneService.Dispose();
        this.m_SceneService = null;
    }

    private void OnLoadSceneProgressHandler(float progress)
    {
        base.GetText("Text_Tip").text = "场景加载中...";
        base.GetText("Text_Rate").text = $"{ Math.Round(progress * 100, 2) }%";
        base.GetImage("Image_Fill").fillAmount = progress;

        if (progress == 1)
            this.gameObject.SetActive(false);
    }
}