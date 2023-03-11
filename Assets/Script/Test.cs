using FrameWork.Core.Mathf_;
using FrameWork.Core.Modules.AssetsLoader;
using FrameWork.Core.Modules.Signal;
using FrameWork.Core.Modules.UI;
using FrameWork.Core.SingletonManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int Value;
    public Data(int value)
    {
        Value = value;
    }
}

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.OpenPanel<LoginPanel>();

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

        LRUCache<Data> cache = new LRUCache<Data>(2);
        var d1 = new Data(1);
        var d2 = new Data(2);
        var d3 = new Data(3);
        Dictionary<int, Data> datas = new Dictionary<int, Data>();
        datas.Add(1, d1);
        datas.Add(2, d2);
        datas.Add(3, d3);


        cache.FreeOldestNodeCallBack += ((data) =>
        {
            Debug.Log("释放 ： " + data + " value = " + data.Value);
        });
        cache.Put("1", d1);
        cache.Put("2", d2);
        cache.Put("3", d1);

        var assetData = cache.Get("2");
        cache.Remove("2");
        var assetData1 = cache.Get("2");


        Debug.Log(assetData);
        Debug.Log(assetData1);

        cache.Clean();
        cache.Put("1", d3);
        cache.Put("2", d1);
        cache.Put("3", d2);

        assetData = cache.Get("2");
        cache.Remove("2");
        assetData1 = cache.Get("2");


        Debug.Log(assetData.Value);
        Debug.Log(assetData1);
        //Debug.Log(cache.Size);
        //cache.Put("1", "a");
        //cache.Put("2", "b");
        //cache.Put("3", "c");
        //cache.Put("4", "d");
        //cache.Put("5", "e");
        //cache.Put("6", "f");
        //cache.Put("6", "f");
        //cache.Put("7", "g");
        //cache.Put("8", "h");
        //var s = cache.Get("1");
        //var s1 = cache.Get("4");
        //cache.Put("9", "i");
        //cache.Put("10", "j");
        //cache.Put("10", "k");
        //Debug.Log(s + s1);
        //Debug.Log(cache.Size);
    }
}
