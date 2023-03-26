using FrameWork.Core.Mixin;
using FrameWork.Core.Modules.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Modules.Signal
{
    public enum UISignal
    {
        OnOpen = 1,
        OnOpened = 2,
        OnClosed = 3,
        OnHide = 4,
        OnShow = 5,
        initialized = 6,
        OnRefresh = 7,
        OnDestroy = 8
    }

    public delegate void UISignalHandle(UIPanelBase panel, params object[] args);

    public sealed class UISignalSystem : SingletonBase<UISignalSystem>
    {
        private Dictionary<UISignal, UISignalHandle> m_UISignals = new Dictionary<UISignal, UISignalHandle>();

        public void RaiseSignal(UISignal signal, UIPanelBase UIPanel, params object[] args)
        {
            if (!this.m_UISignals.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                //Debug.LogError($"UI信号未注册:{ signalName }");
                return;
            }

            var index = 0;
            foreach (UISignalHandle handle in this.m_UISignals[signal].GetInvocationList())
            {
                index++;
                try
                {
                    handle.Invoke(UIPanel, args);
                }
                catch (Exception ex)
                {

                    Debug.LogError($"RaiseUISignal Exception : {ex.Message}, signal = {Enum.GetName(typeof(UISignal), signal)}, index = {index}");
                }
            }
        }

        public void RegisterSignal(UISignal signal, UISignalHandle handle)
        {
            if (this.m_UISignals.ContainsKey(signal))
                this.m_UISignals[signal] += handle;
            else
                this.m_UISignals.Add(signal, handle);
        }

        public void RemoveAllSignal(UISignal signal)
        {
            if (!this.m_UISignals.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"UI信号未注册:{ signalName }");
                return;
            }

            this.m_UISignals[signal] = null;
            this.m_UISignals.Remove(signal);
        }

        public void RemoveSignal(UISignal signal, UISignalHandle handle)
        {
            if (!this.m_UISignals.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"UI信号未注册:{ signalName }");
                return;
            }

            if (this.m_UISignals[signal] != null)
                this.m_UISignals[signal] -= handle;
            else
                this.m_UISignals.Remove(signal);
        }
    }
}