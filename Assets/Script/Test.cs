using FrameWork.Core.ResourcesLoader;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //this.StartCoroutine(TestMethod("UI/Prefabs/Login/LoginPanel.prefab"));
        //var resourcesLoader = new EditorResourcesLoader();
        //var go = resourcesLoader.LoadAssets<GameObject>("UI/Prefabs/Login/LoginPanel.prefab");
        //Debug.Log(go.name);
        //GameObject.Instantiate(go, this.gameObject.transform);

        var resourcesLoader = new EditorResourcesLoader();
        this.StartCoroutine(resourcesLoader.LoadAssetsAsync<GameObject>("UI/Prefabs/Login/LoginPanel.prefab", (ret) => {
            Debug.Log("资源加载完成");
            var inst = GameObject.Instantiate(ret, this.gameObject.transform);
            inst.name = "IEnumeratorGo";
        }));
    }
}
