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
        var prefab = AssetsLoaderManager.Instance.LoadAssets<GameObject>("UI/Prefabs/Login/LoginPanel.prefab");
        Debug.Log(prefab.name);
        var go = GameObject.Instantiate(prefab, this.gameObject.transform);
        Debug.Log(go.name + go.GetInstanceID());

        var UILayerManager = GameObject.Find("UIManager").GetComponent<UILayerManager>();
        UILayerManager.SetLayer(go.GetComponent<UIPanelBase>());

        UISignalSystem uISignalSystem = UISignalSystem.Instance;
        uISignalSystem.RegisterSignal(UISignal.OnOpened, (uIPanel, args) =>
        {
            Debug.Log("uIPanel == " + uIPanel.name);
            Debug.Log("args[0] == " + args[0]);
            Debug.Log("args[1] == " + args[1]);
        });

        uISignalSystem.RegisterSignal(UISignal.OnOpened, (uIPanel, args) =>
        {
            Debug.Log("uIPanel xx " + uIPanel.name);
            Debug.Log("args[0] xx " + args[0]);
            Debug.Log("args[1] xx " + args[1]);
        });

        this.StartCoroutine(this.TestMethod(go));
        Debug.Log("********************************");
    }
    //IEnumerator
    private IEnumerator TestMethod(GameObject go)
    {
        yield return new WaitForSeconds(3);
        Debug.Log("3秒后");
        UISignalSystem.Instance.RaiseSignal(UISignal.OnOpened, go.GetComponent<UIPanelBase>(), "hello world", 999);
    }
}
