using FrameWork.Core.Modules.UI;
using UnityEngine;

public class ShopPanel : UIPanelBase
{
    public override void OnInit()
    {
        Debug.Log("登陆界面初始化");
    }

    public override void OnOpen()
    {
        Debug.Log("登陆界面已打开");
    }
}
