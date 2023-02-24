using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

/// <summary>
/// Prefab保存监听器
/// </summary>
public class PrefabStageListener
{
    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod()
    {
        PrefabStage.prefabSaved += OnPrefabSaved;
        PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdated;
    }

    public static void OnPrefabSaved(GameObject gameObject)
    {
        Debug.Log($"预制体保存成功回调：, { gameObject.name }");
    }

    public static void OnPrefabInstanceUpdated(GameObject gameObject)
    {
        Debug.Log($"预制体变更成功回调：, { gameObject.name }");
    }
}
