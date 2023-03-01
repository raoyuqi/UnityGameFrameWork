using FrameWork.Core.Modules.UI;
using System;
using System.Collections;
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

    public delegate void UICallBack(UIPanelBase panel, params object[] args);

    public sealed class UISignalSystem
    {
        public void RaisedSignal()
        {
            throw new NotImplementedException();
        }

        public void RegisterSignal(UISignal name, UICallBack callBack)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllSignal(UISignal name)
        {
            throw new NotImplementedException();
        }

        public void RemoveSignal(UISignal name, UICallBack callBack)
        {
            throw new NotImplementedException();
        }
    }

    // 传参基类，TODO：转移到全局事件系统
    public class SignalArgs { }
}