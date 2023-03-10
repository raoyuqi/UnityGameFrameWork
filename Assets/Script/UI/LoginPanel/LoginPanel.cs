using FrameWork.Core.Modules.UI;
using FrameWork.Core.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class LoginPanel : UIPanelBase
{
    public override void OnInit()
    {
        Debug.Log("登陆界面初始化");
    }

    public override void OnOpen()
    {
        Debug.Log("登陆界面已打开");

        // 加载逻辑
        var path = Path.Combine(Application.streamingAssetsPath, "spritealtas/login.spriteatlas");
        var path1 = Path.Combine(Application.streamingAssetsPath, "prefabs/main/mainpanel.prefab");
        var path2 = Path.Combine(Application.streamingAssetsPath, "prefabs/shop/mainpanel.prefab");
        
        var ab = AssetBundle.LoadFromFile(path);
        var ab1 = AssetBundle.LoadFromFile(path1);
        var ab2 = AssetBundle.LoadFromFile(path2);

        var prefab = ab1.LoadAsset<GameObject>("Assets/AssetsPackage/Prefabs/Main/MainPanel.prefab");
        Debug.Log(prefab + "   " + prefab.name);
        Debug.Log(ab1);
        //GameObject.Instantiate(prefab, GameObject.Find("Canvas").transform);

        var asset = ab.LoadAsset<SpriteAtlas>("Assets/AssetsPackage/SpriteAltas/Login.spriteatlas");

        Sprite[] res = new Sprite[asset.spriteCount];
        var count = asset.GetSprites(res);
        foreach (var item in res)
        {
            Debug.Log(item.name);
        }

        Debug.Log("***************** " + asset.GetSprite("img_xian2").name);

        Debug.Log(ab2);
        prefab = ab2.LoadAsset<GameObject>("Assets/AssetsPackage/Prefabs/Shop/MainPanel.prefab");
        Debug.Log(prefab + "   " + prefab.name);


        Debug.Log("count === " + count);
        Debug.Log(asset);
        Debug.Log("***************** ");
    }
}
