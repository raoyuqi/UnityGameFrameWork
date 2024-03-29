﻿using FrameWork.Core.Modules.Signal;
using FrameWork.Core.Modules.UI;
using FrameWork.Core.SingletonManager;
using FrameWork.Core.Utils;
using Game.Scene;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class LoginPanel : UIPanelBase
{
    public override void OnInit()
    {
        Debug.Log("登陆界面初始化");
        base.GetButton("Button_Login").onClick.AddListener(() =>
        {
            Debug.Log("登录游戏");
            var scene = new MainScene() { Name = "Main" };
            GlobalSignalSystem.Instance.RaiseSignal(GlobalSignal.TransScene, scene);

            //UIManager.Instance.OpenPanel<MainPanel>();
        });
    }

    public override void OnOpen()
    {
        Debug.Log("登陆界面已打开");

        // 加载逻辑
        //var path = Path.Combine(Application.streamingAssetsPath, "spritealtas/login.spriteatlas");
        //var ab = AssetBundle.LoadFromFile(path);
        //var asset = ab.LoadAsset<SpriteAtlas>("Assets/AssetsPackage/SpriteAltas/Login.spriteatlas");

        //Sprite[] res = new Sprite[asset.spriteCount];
        //var count = asset.GetSprites(res);
        //foreach (var item in res)
        //{
        //    Debug.Log(item.name);
        //}

        //Debug.Log("count === " + count);
        //Debug.Log(asset);
    }
}
