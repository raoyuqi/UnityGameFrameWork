using FrameWork.Core.Mixin;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Modules.Signal
{
    public delegate void GlobalSignalHandle(params object[] args);

    public sealed class GlobalSignalSystem : SingletonBase<GlobalSignalSystem>
    {
        private Dictionary<Enum, GlobalSignalHandle> m_GlobalSignalsByEnum = new Dictionary<Enum, GlobalSignalHandle>();
        private Dictionary<string, GlobalSignalHandle> m_GlobalSignalsByString = new Dictionary<string, GlobalSignalHandle>();

        public void RaiseSignal(UISignal signal, params object[] args)
        {
            if (!this.m_GlobalSignalsByEnum.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"全局信号未注册:{ signalName }");
                return;
            }

            var index = 0;
            foreach (GlobalSignalHandle handle in this.m_GlobalSignalsByEnum[signal].GetInvocationList())
            {
                index++;
                try
                {
                    handle.Invoke(args);
                }
                catch (Exception ex)
                {

                    Debug.LogError($"RaiseGlobalSignal Exception : {ex.Message}, signal = {Enum.GetName(typeof(UISignal), signal)}, index = {index}");
                }
            }
        }

        public void RegisterSignal(Enum signal, GlobalSignalHandle handle)
        {
            if (this.m_GlobalSignalsByEnum.ContainsKey(signal))
                this.m_GlobalSignalsByEnum[signal] += handle;
            else
                this.m_GlobalSignalsByEnum.Add(signal, handle);
        }

        public void RemoveAllSignal(Enum signal)
        {
            if (!this.m_GlobalSignalsByEnum.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"全局信号未注册:{ signalName }");
                return;
            }

            this.m_GlobalSignalsByEnum[signal] = null;
            this.m_GlobalSignalsByEnum.Remove(signal);
        }

        public void RemoveSignal(Enum signal, GlobalSignalHandle handle)
        {
            if (!this.m_GlobalSignalsByEnum.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"全局信号未注册:{ signalName }");
                return;
            }

            if (this.m_GlobalSignalsByEnum[signal] != null)
                this.m_GlobalSignalsByEnum[signal] -= handle;
            else
                this.m_GlobalSignalsByEnum.Remove(signal);
        }

        public void RaiseSignal(string signal, params object[] args)
        {
            if (!this.m_GlobalSignalsByString.ContainsKey(signal))
            {
                Debug.LogError($"全局信号未注册:{ signal }");
                return;
            }

            var index = 0;
            foreach (GlobalSignalHandle handle in this.m_GlobalSignalsByString[signal].GetInvocationList())
            {
                index++;
                try
                {
                    handle.Invoke(args);
                }
                catch (Exception ex)
                {

                    Debug.LogError($"RaiseGlobalSignal Exception : {ex.Message}, signal = {signal}, index = {index}");
                }
            }
        }

        public void RegisterSignal(string signal, GlobalSignalHandle handle)
        {
            if (this.m_GlobalSignalsByString.ContainsKey(signal))
                this.m_GlobalSignalsByString[signal] += handle;
            else
                this.m_GlobalSignalsByString.Add(signal, handle);
        }

        public void RemoveAllSignal(string signal)
        {
            if (!this.m_GlobalSignalsByString.ContainsKey(signal))
            {
                Debug.LogError($"全局信号未注册:{ signal }");
                return;
            }

            this.m_GlobalSignalsByString[signal] = null;
            this.m_GlobalSignalsByString.Remove(signal);
        }

        public void RemoveSignal(string signal, GlobalSignalHandle handle)
        {
            if (!this.m_GlobalSignalsByString.ContainsKey(signal))
            {
                Debug.LogError($"全局信号未注册:{ signal }");
                return;
            }

            if (this.m_GlobalSignalsByString[signal] != null)
                this.m_GlobalSignalsByString[signal] -= handle;
            else
                this.m_GlobalSignalsByString.Remove(signal);
        }
    }

    // 传参基类，TODO：是否需要
    public class SignalArgs { }
}