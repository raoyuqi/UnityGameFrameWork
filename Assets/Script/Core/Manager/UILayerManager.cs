using FrameWork.Core.Modules.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UILayerManager : MonoBehaviour
{
    [Serializable]
    private struct UICameraData
    {
        public Camera m_UICamera;
        public GameObject m_Root;
        public Transform m_NormalLayer;
        public Transform m_PopupLayer;
        public Transform m_TipsLayer;
        public Transform m_TopLayer;
        public Transform m_LoadingLayer;
    }

    [SerializeField]
    private List<UICameraData> m_UICameraList = new List<UICameraData>();

    // 默认UI相机
    private UICameraData m_DefaultUICameraData;

    private void Awake()
    {
        // 检查数据完整性
        if (this.m_UICameraList.Count == 0)
            throw new Exception($"UI相机数据缺失，请检查 GameObject: {this.gameObject.name} 配置");

        var cameraData = this.m_UICameraList[0];
        if (cameraData.m_UICamera == null || cameraData.m_Root == null ||
            cameraData.m_NormalLayer == null || cameraData.m_PopupLayer == null ||
            cameraData.m_TipsLayer == null || cameraData.m_TopLayer == null || cameraData.m_LoadingLayer == null)
        {
            throw new Exception($"UI相机数据缺失，请检查 GameObject: {this.gameObject.name} 配置");
        }

        this.m_DefaultUICameraData = this.m_UICameraList[0];
    }

    public void SetLayer(UIPanelBase panel)
    {
        if (panel == null)
            return;

        // 设置层级
        switch (panel.UILayerType)
        {
            case UILayerType.Normal:
                panel.transform.SetParent(this.m_DefaultUICameraData.m_NormalLayer);
                break;
            case UILayerType.Popup:
                panel.transform.SetParent(this.m_DefaultUICameraData.m_PopupLayer);
                break;
            case UILayerType.Tips:
                panel.transform.SetParent(this.m_DefaultUICameraData.m_TipsLayer);
                break;
            case UILayerType.Top:
                panel.transform.SetParent(this.m_DefaultUICameraData.m_TopLayer);
                break;
            case UILayerType.Loading:
                panel.transform.SetParent(this.m_DefaultUICameraData.m_LoadingLayer);
                break;
            default:
                panel.transform.SetParent(this.m_DefaultUICameraData.m_NormalLayer);
                break;
        }

        // 设坐标
        var rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = Vector2.zero;
        rt.localScale = Vector2.one;
        rt.eulerAngles = Vector2.zero;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        //rt.SetAsLastSibling();
    }
}
