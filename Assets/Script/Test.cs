using FrameWork.Core.Mathf_;
using FrameWork.Core.Modules.AssetsLoader;
using FrameWork.Core.Modules.Signal;
using FrameWork.Core.Modules.UI;
using FrameWork.Core.SingletonManager;
using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            UnityEngine.Debug.Log("打开完成 " + uIPanel.name);
        });

        uISignalSystem.RegisterSignal(UISignal.initialized, (uIPanel, args) =>
        {
            UnityEngine.Debug.Log("初始化完成 " + uIPanel.name);
        });


        uISignalSystem.RegisterSignal(UISignal.OnOpen, (uIPanel, args) =>
        {
            UnityEngine.Debug.Log("正在打开界面 " + uIPanel.name);
        });

        //this.StartCoroutine(this.TestMethod(go));
        UnityEngine.Debug.Log("********************************");

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
            UnityEngine.Debug.Log("释放 ： " + data + " value = " + data.Value);
        });
        cache.Put("1", d1);
        cache.Put("2", d2);
        cache.Put("3", d1);

        var assetData = cache.Get("2");
        cache.Remove("2");
        var assetData1 = cache.Get("2");


        UnityEngine.Debug.Log(assetData);
        UnityEngine.Debug.Log(assetData1);

        cache.Clean();
        cache.Put("1", d3);
        cache.Put("2", d1);
        cache.Put("3", d2);

        assetData = cache.Get("2");
        cache.Remove("2");
        assetData1 = cache.Get("2");


        UnityEngine.Debug.Log(assetData.Value);
        UnityEngine.Debug.Log(assetData1);
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

        //string jsonStr = "{\"id\":10001,\"name\":\"test\"}";

        var dic = new List<Dictionary<string, string>>();
        dic.Add(new Dictionary<string, string>()
            {
                { "name", "file1" },{ "md5", "xxx1" },{ "path", "xxxpath" }
            });
        dic.Add(new Dictionary<string, string>()
            {
                { "name", "file2" },{ "md5", "xxx2" },{ "path", "xxxpath2" }
            });
        dic.Add(new Dictionary<string, string>()
            {
                { "name", "file3" },{ "md5", "xxx3" },{ "path", "xxxpath3" }
            });

        var watch = new Stopwatch();
        watch.Reset();
        watch.Start();
        for (int i = 0; i < 2; i++)
        {
            var jsonStr = Json.Serialize(dic);

            var obj = Json.Deserialize(jsonStr) as List<object>;
            foreach (var item in obj)
            {
                var o = item as Dictionary<string, object>;
                UnityEngine.Debug.Log(o["name"] + "   " + o["md5"] + "   " + o["path"]);
            }
            UnityEngine.Debug.Log("jsonStr" + "   " + jsonStr);
        }
        watch.Stop();
        UnityEngine.Debug.Log("SimpleJson Parse Time(ms):" + watch.ElapsedMilliseconds);
    }
}
