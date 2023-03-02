using FrameWork.Core.Modules.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Modules.Signal
{
    public enum UISignal
    {
        OnOpened = 1,
        OnClosed = 2,
        OnHide = 3,
        OnShow = 4,
        OnInit = 5,
        OnRefresh = 6,
        OnDestroy = 7
    }

    public delegate void UISignalCallBack(UIPanelBase panel, params object[] args);

    public sealed class UISignalSystem
    {
        private Dictionary<UISignal, UISignalCallBack> m_UISignals = new Dictionary<UISignal, UISignalCallBack>();

        public void RaisedSignal(UISignal signal, UIPanelBase UIPanel, params object[] args)
        {
            if (!this.m_UISignals.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"信号未注册:{ signalName }");
            }

            this.m_UISignals[signal]?.Invoke(UIPanel, args);
        }

        public void RegisterSignal(UISignal signal, UISignalCallBack callBack)
        {
            if (this.m_UISignals.ContainsKey(signal))
                this.m_UISignals[signal] += callBack;
            else
                this.m_UISignals.Add(signal, callBack);
        }

        public void RemoveAllSignal(UISignal signal)
        {
            if (!this.m_UISignals.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"信号未注册:{ signalName }");
            }

            this.m_UISignals[signal] = null;
            this.m_UISignals.Remove(signal);
        }

        public void RemoveSignal(UISignal signal, UISignalCallBack callBack)
        {
            if (!this.m_UISignals.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"信号未注册:{ signalName }");
            }

            if (this.m_UISignals[signal] != null)
                this.m_UISignals[signal] -= callBack;
            else
                this.m_UISignals.Remove(signal);
        }
    }

    // 传参基类，TODO：转移到全局事件系统
    public class SignalArgs { }
}