﻿using FrameWork.Core.Mixin;
using FrameWork.Core.Modules.Pool;
using FrameWork.Core.Modules.Signal;
using FrameWork.Core.Modules.UI;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Manager
{
    /// <summary>
    /// 单例UI管理器
    /// </summary>
    public sealed class UIManager : SingletonBase<UIManager>
    {
        private const string UI_MANAGER_ROOT_NAME = "UIManager";
        // 所有UI
        // TODO: LRU存储
        private Dictionary<string, UIPanelBase> m_UIPanelDic = new Dictionary<string, UIPanelBase>();
        // 隐藏的UI栈
        //private Stack<UIPanelBase> m_HideUIStack = new Stack<UIPanelBase>();

        // 打开的独占界面和它的子弹窗
        private Dictionary<string, List<UIPanelBase>> m_ExclusiveUIPopups = new Dictionary<string, List<UIPanelBase>>();

        // TODO: 从其它界面跳回主界面需要清空栈中数据,实现一个跳回主界面的方法
        // 隐藏的独占UI
        private Stack<UIPanelBase> m_HideExclusiveUIStack = new Stack<UIPanelBase>();

        // 显示的独占UI
        private Stack<UIPanelBase> m_ShowExclusiveUIStack = new Stack<UIPanelBase>();

        private IGameObjectPool m_GameObjectPool;
        private UISignalSystem m_UISignalSystem;
        private AssetsLoaderManager m_AssetsLoaderManager;
        private UILayerManager m_UILayerManager;

        //private const string r_PathRoot = "UI/Prefabs/";

        public UIManager()
        {
            this.m_UISignalSystem = UISignalSystem.Instance;
            this.m_AssetsLoaderManager = AssetsLoaderManager.Instance;
            this.m_UILayerManager = GameObject.Find(UI_MANAGER_ROOT_NAME).GetComponent<UILayerManager>();
            this.m_GameObjectPool = new DefaultGameObjectPool();
        }

        #region 管理独占UI
        /// <summary>
        /// 打开独占UI
        /// </summary>
        /// <typeparam name="T">独占UI类型</typeparam>
        public void OpenPanel<T>() where T : UIPanelBase
        {
            this.OpenPanel(typeof(T).Name);
        }

        /// <summary>
        /// 关闭独占UI
        /// </summary>
        /// <typeparam name="T">独占UI类型</typeparam>
        public void ClosePanel<T>() where T : UIPanelBase
        {
            this.ClosePanel(typeof(T).Name);
        }

        private void OpenPanel(string panelName)
        {
            if (!UIAssetsConfig.PathConfit.ContainsKey(panelName))
            {
                Debug.LogError($"界面不存在: panelName = {panelName}");
                return;
            }

            if (this.m_UIPanelDic.ContainsKey(panelName))
            {
                this.ShowPanel(panelName);
                return;
            }

            // TODO: 从缓存池获取GameObject

            var fullPath = UIAssetsConfig.PathConfit[panelName];
            this.m_AssetsLoaderManager.LoadAssetsAsync<GameObject>(fullPath, (prefab) =>
            {
                var panel = this.CreateUIPanel(prefab, panelName);
                this.m_UISignalSystem.RaiseSignal(UISignal.OnOpen, panel);
                panel.OnOpen();

                this.OpenPanelHandle(panel, panelName);
                this.m_ShowExclusiveUIStack.Push(panel);

                this.m_UISignalSystem.RaiseSignal(UISignal.OnOpened, panel);
            });
            // TODO：将go加入对象池，派发一些UI事件
        }

        private void ShowPanel(string panelName)
        {
            if (this.m_UIPanelDic.TryGetValue(panelName, out UIPanelBase panel))
            {
                var go = panel.gameObject;
                go.SetActive(true);
                panel.OnOpen();
                go.transform.SetAsLastSibling();
                this.OpenPanelHandle(panel, panelName);
                this.m_UISignalSystem.RaiseSignal(UISignal.OnShow, panel);
            }
        }

        private void ClosePanel(string panelName)
        {
            if (this.m_UIPanelDic.TryGetValue(panelName, out UIPanelBase panel))
            {
                var go = panel.gameObject;
                go.SetActive(false);
                panel.OnHide();
                this.ClosePanelHandle(panel, panelName);
                this.m_UISignalSystem.RaiseSignal(UISignal.OnClosed, panel);
                return;
            }


            // 使用放回对象池代替Destroy
            //GameObject.Destroy(panel.gameObject);
        }

        /// <summary>
        /// 打开新的独占UI时，需要隐藏当前显示的独占UI以及属于它的所有弹窗
        /// </summary>
        /// <param name="panel">独占UI</param>
        /// <param name="panelName">独占UI名称</param>
        private void OpenPanelHandle(UIPanelBase panel, string panelName)
        {
            if (panel.UIShowType != UIShowType.Exclusive)
                return;

            while (this.m_ShowExclusiveUIStack.Count > 0)
            {
                var exclusivePanel = this.m_ShowExclusiveUIStack.Pop();
                exclusivePanel.gameObject.SetActive(false);
                exclusivePanel.OnHide();
                this.m_HideExclusiveUIStack.Push(exclusivePanel);

                if (this.m_ExclusiveUIPopups.TryGetValue(panelName, out List<UIPanelBase> popups))
                {
                    foreach (var popup in popups)
                    {
                        popup.gameObject.SetActive(false);
                        popup.OnHide();
                    } 
                }
            }
        }

        /// <summary>
        /// 恢复上一个被关闭的全屏UI的状态，如果有的话
        /// </summary>
        /// <param name="panel">独占UI</param>
        /// <param name="panelName">独占UI名称</param>
        private void ClosePanelHandle(UIPanelBase panel, string panelName)
        {
            if (panel.UIShowType != UIShowType.Exclusive)
                return;

            if (this.m_HideExclusiveUIStack.Count > 0)
            {
                var exclusivePanel = this.m_HideExclusiveUIStack.Pop();
                exclusivePanel.gameObject.SetActive(true);
                exclusivePanel.OnOpen();
                this.m_ShowExclusiveUIStack.Push(exclusivePanel);

                if (this.m_ExclusiveUIPopups.TryGetValue(panelName, out List<UIPanelBase> popups))
                {
                    foreach (var popup in popups)
                    {
                        popup.gameObject.SetActive(true);
                        popup.OnOpen();
                    }
                }
            }
        }
        #endregion

        #region 管理弹窗
        /// <summary>
        /// 打开弹窗
        /// </summary>
        /// <typeparam name="T1">弹窗类型</typeparam>
        /// <param name="T2">弹窗所属的独占UI</param>
        public void OpenPopup<T1, T2>() where T1 : UIPanelBase where T2 : UIPanelBase
        {
            this.OpenPopup(typeof(T1).Name, typeof(T2).Name);
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        /// <typeparam name="T1">弹窗类型</typeparam>
        /// <typeparam name="T2">弹窗所属界面</typeparam>
        public void ClosePopup<T1, T2>() where T1 : UIPanelBase where T2 : UIPanelBase
        {
            this.ClosePopup(typeof(T1).Name, typeof(T2).Name);
        }

        private void OpenPopup(string popupName, string panelName)
        {
            if (!UIAssetsConfig.PathConfit.ContainsKey(popupName))
            {
                Debug.LogError($"界面不存在: popupName = {popupName}");
                return;
            }

            if (this.m_UIPanelDic.ContainsKey(popupName))
            {
                this.ShowPopup(popupName, panelName);
                return;
            }

            var fullPath = UIAssetsConfig.PathConfit[popupName];
            this.m_AssetsLoaderManager.LoadAssetsAsync<GameObject>(fullPath, (prefab) =>
            {
                var popup = this.CreateUIPanel(prefab, popupName);
                this.m_UISignalSystem.RaiseSignal(UISignal.OnOpen, popup);
                popup.OnOpen();
                this.m_UISignalSystem.RaiseSignal(UISignal.OnOpened, popup);
                this.OpenPopupHandle(popup, panelName);
            });
        }

        private void ShowPopup(string popupName, string panelName)
        {
            if (this.m_UIPanelDic.TryGetValue(popupName, out UIPanelBase popup))
            {
                var go = popup.gameObject;
                go.SetActive(true);
                popup.OnOpen();
                go.transform.SetAsLastSibling();
                this.OpenPopupHandle(popup, panelName);
                this.m_UISignalSystem.RaiseSignal(UISignal.OnShow, popup);
            }
        }

        private void ClosePopup(string popupName, string panelName)
        {
            if (this.m_UIPanelDic.TryGetValue(popupName, out UIPanelBase popup))
            {
                popup.gameObject.SetActive(false);
                popup.OnHide();
                this.ClosePopupHandle(popup, panelName);
                this.m_UISignalSystem.RaiseSignal(UISignal.OnClosed, popup);
                return;
            }
        }

        /// <summary>
        /// 打开弹窗后需要将弹窗添加到当前打开的全屏UI中
        /// </summary>
        /// <param name="popup">弹窗</param>
        /// <param name="panelName">弹窗所属的独占UI名称</param>
        private void OpenPopupHandle(UIPanelBase popup, string panelName)
        {
            if (popup.UIShowType != UIShowType.Popup)
                return;

            if (!this.m_ExclusiveUIPopups.ContainsKey(panelName))
                this.m_ExclusiveUIPopups.Add(panelName, new List<UIPanelBase>());

            this.m_ExclusiveUIPopups[panelName].Add(popup);
        }

        /// <summary>
        /// 关闭弹窗后，将它从所属独占UI中移除
        /// </summary>
        /// <param name="popup">弹窗</param>
        /// <param name="panelName">弹窗所属的独占UI名称</param>
        private void ClosePopupHandle(UIPanelBase popup, string panelName)
        {
            if (popup.UIShowType != UIShowType.Popup)
                return;

            if (this.m_ExclusiveUIPopups.TryGetValue(panelName, out List<UIPanelBase> popups))
            {
                if (popups.Count > 0)
                    popups.Remove(popup);
            }
        }
        #endregion

        private UIPanelBase CreateUIPanel(GameObject prefab, string uiName)
        {
            var go = this.m_GameObjectPool.GetGameObject(prefab);
            var panel = go.GetComponent<UIPanelBase>();

            panel.OnInit();
            this.m_UISignalSystem.RaiseSignal(UISignal.initialized, panel);

            this.m_UILayerManager.SetLayer(panel);
            go.transform.SetAsLastSibling();
            this.m_UIPanelDic.Add(uiName, panel);
            return panel;
        }
    }
}