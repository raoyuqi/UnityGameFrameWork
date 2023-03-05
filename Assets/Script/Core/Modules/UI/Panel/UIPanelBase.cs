using FrameWork.Core.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.Core.Modules.UI
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
        Popup = 1,   // 常用弹窗显示层级
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
        [SerializeField, ReadOnly, Header("控件节点")]
        private List<GameObject> m_BindObjects = new List<GameObject>();

        // 所有需要访问到的节点集合
        //private Dictionary<string, GameObject> m_BindObjects = new Dictionary<string, GameObject>();

        [SerializeField, LabelEnum("展示类型")]
        private UIShowType m_UIShowType;
        public UIShowType UIShowType
        {
            get { return this.m_UIShowType; }
        }

        [SerializeField, LabelEnum("所属层级")]
        private UILayerType m_UILayerType;
        public UILayerType UILayerType
        {
            get { return this.m_UILayerType; }
        }

        private List<UIPanelBase> m_PopupList = new List<UIPanelBase>(); 

        // 缓存界面使用到的控件
        private Dictionary<string, Text> m_TextCache = new Dictionary<string, Text>();
        private Dictionary<string, Image> m_ImageCache = new Dictionary<string, Image>();
        private Dictionary<string, RawImage> m_RawImageCache = new Dictionary<string, RawImage>();
        private Dictionary<string, Button> m_ButtonCache = new Dictionary<string, Button>();
        private Dictionary<string, Toggle> m_ToggleCache = new Dictionary<string, Toggle>();
        private Dictionary<string, Slider> m_SliderCache = new Dictionary<string, Slider>();
        private Dictionary<string, ScrollRect> m_ScrollRectCache = new Dictionary<string, ScrollRect>();
        private Dictionary<string, InputField> m_InputFieldCache = new Dictionary<string, InputField>();

        #region 重载方法
        public virtual void Dispose() { }

        public virtual void OnHide() { }

        public virtual void OnInit() { }

        public virtual void OnOpen() { }
        #endregion

        #region UI常用方法
        public GameObject GetGameObject(string name)
        {
            var go = this.m_BindObjects.Find(gameObject => gameObject.name == name) ?? default;
            if (go == null)
                throw new Exception("试图访问不存在的空间");

            return go;
        }

        public Text GetText(string name)
        {
            if (this.m_TextCache.ContainsKey(name))
                return this.m_TextCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<Text>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 TextComponent");

            this.m_TextCache.Add(name, component);
            return component;
        }

        public Image GetImage(string name)
        {
            if (this.m_ImageCache.ContainsKey(name))
                return this.m_ImageCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<Image>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 ImageComponent");

            this.m_ImageCache.Add(name, component);
            return component;
        }

        public RawImage GetRawImage(string name)
        {
            if (this.m_RawImageCache.ContainsKey(name))
                return this.m_RawImageCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<RawImage>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 RawImageComponent");

            this.m_RawImageCache.Add(name, component);
            return component;
        }

        public Button GetButton(string name)
        {
            if (this.m_ButtonCache.ContainsKey(name))
                return this.m_ButtonCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<Button>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 ButtonComponent");

            this.m_ButtonCache.Add(name, component);
            return component;
        }

        public Toggle GetToggle(string name)
        {
            if (this.m_ToggleCache.ContainsKey(name))
                return this.m_ToggleCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<Toggle>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 ToggleComponent");

            this.m_ToggleCache.Add(name, component);
            return component;
        }

        public Slider GetSlider(string name)
        {
            if (this.m_SliderCache.ContainsKey(name))
                return this.m_SliderCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<Slider>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 SliderComponent");

            this.m_SliderCache.Add(name, component);
            return component;
        }

        public ScrollRect GetScrollRect(string name)
        {
            if (this.m_ScrollRectCache.ContainsKey(name))
                return this.m_ScrollRectCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<ScrollRect>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 ScrollRectComponent");

            this.m_ScrollRectCache.Add(name, component);
            return component;
        }

        public InputField GetInputField(string name)
        {
            if (this.m_InputFieldCache.ContainsKey(name))
                return this.m_InputFieldCache[name];

            var go = this.GetGameObject(name);
            var component = go.GetComponent<InputField>();
            if (component == null)
                throw new Exception($"{name} 节点不存在 InputFieldComponent");

            this.m_InputFieldCache.Add(name, component);
            return component;
        }
        #endregion

        public void BindingGameObjectList(List<GameObject> gameObjects)
        {
            this.m_BindObjects = gameObjects;
        }

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