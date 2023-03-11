using FrameWork.Core.Modules.UI;
using UnityEngine;

public class MainPanel : UIPanelBase
{
    public override void OnInit()
    {
        Debug.Log("主界面初始化");
    }

    public override void OnOpen()
    {
        Debug.Log("主界面已打开");
    }
}
