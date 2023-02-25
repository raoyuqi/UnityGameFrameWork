using Core.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    public enum UIShowType
    {
        [LabelEnum("普通UI")]
        Normal   = 0,   // 普通UI类型
        [LabelEnum("弹窗UI")]
        Popup    = 1,   // 弹窗UI类型
        /*独占的UI页面，同一时刻只能存在一个这种类型的页面,
        独占页面打开时
         * 隐藏所有打开的 [Normal] 类型的页面
         * 关闭所有打开的 [Popup] 类型的页面
        独占页面退出时
         * 恢复上个打开的独占页面
         * 恢复所有隐蔽的【Normal】类型页面
         * [Popup] 类型页面不会恢复，因为已经关闭了*/
        [LabelEnum("独占UI")]
        Exclusive = 2,
    }

    public enum UILayerType
    {
        [LabelEnum("普通层,显示最下面")]
        Normal   = 0,   // 普通层, 显示最下面
        [LabelEnum("常用弹窗显示层级")]
        Function = 1,   // 常用弹窗显示层级
        [LabelEnum("Tips显示层级")]
        Tips     = 3,   // Tips显示层级
        [LabelEnum("非Loading界面最高层级")]
        Top      = 4,   // 非Loading界面最高层级
        [LabelEnum("Loading层级")]
        Loading  = 5,   // Loading层级
    }

    /// <summary>
    /// 所有面板的基类
    /// </summary>
    public class UIPanelBase : MonoBehaviour, IUIPanel
    {
        // 所有需要访问到的节点集合
        [SerializeField, ReadOnly, LabelText("控件节点")]
        private List<GameObject> m_ObjectList = new List<GameObject>();

        [SerializeField, LabelEnum("展示类型")]
        private UIShowType m_UIShowType;

        [SerializeField, LabelEnum("所属层级")]
        private UILayerType m_UILayerType;

        #region 重载方法
        public virtual void OnClose() { }

        public virtual void OnHide() { }

        public virtual void OnInit() { }

        public virtual void OnOpen() { }
        #endregion

        public void AddEventListener()
        {
            ///TODO: 实现事件监听
        }

        public void RemoveAllEventListener()
        {
            ///TODO: 移除所有事件监听
        }
    }
}