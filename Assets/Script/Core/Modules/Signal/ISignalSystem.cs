using System;

namespace FrameWork.Core.Modules.Signal
{
    public delegate void CallBack(object sender, params object[] args);

    public interface ISignalSystem
    {
        // 发信号
        void RaisedSignal();

        // 注册信号
        void RegisterSignal(Enum name, CallBack callBack);

        //void RegisterSignal(string signalName, CallBack callBack);

        // 移除单个信号
        void RemoveSignal(Enum name, CallBack callBack);

        //void RemoveSignal(string signalName, CallBack callBack);

        // 移除指定类型的所有信号
        void RemoveAllSignal(Enum name);

        //void RemoveAllSignal(string signalName);
    }
}