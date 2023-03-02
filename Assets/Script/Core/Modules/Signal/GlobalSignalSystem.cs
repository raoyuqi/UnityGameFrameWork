using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Modules.Signal
{
    public delegate void GlobalSignalHandle(params object[] args);

    public sealed class GlobalSignalSystem
    {
        private Dictionary<Enum, GlobalSignalHandle> m_GlobalSignalsByEnum = new Dictionary<Enum, GlobalSignalHandle>();
        private Dictionary<string, GlobalSignalHandle> m_GlobalSignalsByString = new Dictionary<string, GlobalSignalHandle>();

        public void RaisedSignal(UISignal signal, params object[] args)
        {
            if (!this.m_GlobalSignalsByEnum.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"全局信号未注册:{ signalName }");
                return;
            }

            this.m_GlobalSignalsByEnum[signal]?.Invoke(args);
        }

        public void RegisterSignal(Enum signal, GlobalSignalHandle callBack)
        {
            if (this.m_GlobalSignalsByEnum.ContainsKey(signal))
                this.m_GlobalSignalsByEnum[signal] += callBack;
            else
                this.m_GlobalSignalsByEnum.Add(signal, callBack);
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

        public void RemoveSignal(Enum signal, GlobalSignalHandle callBack)
        {
            if (!this.m_GlobalSignalsByEnum.ContainsKey(signal))
            {
                var signalName = Enum.GetName(typeof(UISignal), signal);
                Debug.LogError($"全局信号未注册:{ signalName }");
                return;
            }

            if (this.m_GlobalSignalsByEnum[signal] != null)
                this.m_GlobalSignalsByEnum[signal] -= callBack;
            else
                this.m_GlobalSignalsByEnum.Remove(signal);
        }

        public void RaisedSignal(string signal, params object[] args)
        {
            if (!this.m_GlobalSignalsByString.ContainsKey(signal))
            {
                Debug.LogError($"全局信号未注册:{ signal }");
                return;
            }

            this.m_GlobalSignalsByString[signal]?.Invoke(args);
        }

        public void RegisterSignal(string signal, GlobalSignalHandle callBack)
        {
            if (this.m_GlobalSignalsByString.ContainsKey(signal))
                this.m_GlobalSignalsByString[signal] += callBack;
            else
                this.m_GlobalSignalsByString.Add(signal, callBack);
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

        public void RemoveSignal(string signal, GlobalSignalHandle callBack)
        {
            if (!this.m_GlobalSignalsByString.ContainsKey(signal))
            {
                Debug.LogError($"全局信号未注册:{ signal }");
                return;
            }

            if (this.m_GlobalSignalsByString[signal] != null)
                this.m_GlobalSignalsByString[signal] -= callBack;
            else
                this.m_GlobalSignalsByString.Remove(signal);
        }
    }

    // 传参基类，TODO：是否需要
    public class SignalArgs { }
}