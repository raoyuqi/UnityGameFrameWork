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
        base.GetText("Text_Title").text = "游戏主界面";
    }
}
