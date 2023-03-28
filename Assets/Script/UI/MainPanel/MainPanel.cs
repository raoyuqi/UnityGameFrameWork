using FrameWork.Core.Modules.Signal;
using FrameWork.Core.Modules.UI;
using Game.Scene;
using UnityEngine;

public class MainPanel : UIPanelBase
{
    public override void OnInit()
    {
        Debug.Log("主界面初始化");
        base.GetButton("Button_Battle").onClick.AddListener(() =>
        {
            Debug.Log("加载世界地图场景");
            var scene = new WorldScene() { Name = "World" };
            GlobalSignalSystem.Instance.RaiseSignal(GlobalSignal.TransScene, scene);

            //UIManager.Instance.OpenPanel<MainPanel>();
        });
    }

    public override void OnOpen()
    {
        base.GetText("Text_Title").text = "游戏主界面";
    }
}
