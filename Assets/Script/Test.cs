using FrameWork.Core.Modules.ResourcesLoader;
using FrameWork.Core.Modules.UI;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var resourcesLoader = new EditorResourcesLoader();
        var prefab = resourcesLoader.LoadAssets<GameObject>("UI/Prefabs/Login/LoginPanel.prefab");
        Debug.Log(prefab.name);
        var go = GameObject.Instantiate(prefab, this.gameObject.transform);
        Debug.Log(go.name + go.GetInstanceID());

        var UILayerManager = GameObject.Find("UIManager").GetComponent<UILayerManager>();
        UILayerManager.SetLayer(go.GetComponent<UIPanelBase>());
    }
}
