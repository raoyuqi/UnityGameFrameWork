using FrameWork.Core.Manager;
using FrameWork.Core.Modules.AssetsLoader;
using FrameWork.Core.Modules.Signal;
using FrameWork.Core.Modules.UI;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.OpenPanel<LoginPanel>();
        //var prefab = AssetsLoaderManager.Instance.LoadAssets<GameObject>("UI/Prefabs/Login/LoginPanel.prefab");
        //Debug.Log(prefab.name);
        //var go = GameObject.Instantiate(prefab, this.gameObject.transform);
        //Debug.Log(go.name + go.GetInstanceID());

        //var UILayerManager = GameObject.Find("UIManager").GetComponent<UILayerManager>();
        //UILayerManager.SetLayer(go.GetComponent<UIPanelBase>());

        UISignalSystem uISignalSystem = UISignalSystem.Instance;
        uISignalSystem.RegisterSignal(UISignal.OnOpened, (uIPanel, args) =>
        {
            Debug.Log("打开完成 " + uIPanel.name);
        });

        uISignalSystem.RegisterSignal(UISignal.initialized, (uIPanel, args) =>
        {
            Debug.Log("初始化完成 " + uIPanel.name);
        });


        uISignalSystem.RegisterSignal(UISignal.OnOpen, (uIPanel, args) =>
        {
            Debug.Log("正在打开界面 " + uIPanel.name);
        });

        //this.StartCoroutine(this.TestMethod(go));
        Debug.Log("********************************");
    }
}
