using FrameWork.Core.Mixin;
using FrameWork.Core.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Manager
{
    /// <summary>
    /// 单例UI管理器
    /// </summary>
    public sealed class UIManager : SingletonBase<UIManager>
    {
        // 所有隐藏的UI，便于恢复
        private Stack<IUIPanel> m_AllHideUIPanelStacks = new Stack<IUIPanel>();
        // 所有打开的UI
        // TODO: LRU存储
        private Stack<IUIPanel> m_AllShowUIPanelStacks = new Stack<IUIPanel>();

        public UIManager()
        {

        }

        public void OpenPanel(string panelName)
        {
            // TODO: 从缓存池获取GameObject
        }

        public void ShowPanel(string panelName)
        {

        }

        public void HidePanel(string panelName)
        {

        }

        public void ClosePanel(string panelName)
        {

        }
    }
}