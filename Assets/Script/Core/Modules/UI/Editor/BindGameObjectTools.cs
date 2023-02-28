using FrameWork.Core.Modules.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 绑定控件节点工具类
/// </summary>
public static class BindGameObjectTools
{
    // 存储已经绑定的节点名称，用来检测命名冲突
    private static readonly Dictionary<string, bool> s_AlreadyBindGameObjects = new Dictionary<string, bool>();

    /// <summary>
    /// 需要绑定的节点命名前缀
    /// </summary>
    private static readonly string[] s_BindNamePrefix = new string[]
    {
        // 按钮
        "Btn_",
        "Button",

        // 文本
        "Txt_",
        "Text_",

        //InputField
        "Input_",

        // 图片
        "Img_",
        "Image_",

        // RawImage
        "Raw_",

        // 单选框/复选框
        "Tog_",
        "TogGroup_",

        // list
        "List_",

        // Slider
        "Slider_",

        // 组，用来控制显隐
        "Group_"
    };

    private static bool IsBindNode(GameObject gameObject)
    {
        foreach (var prefix in s_BindNamePrefix)
        {
            if (gameObject.name.StartsWith(prefix))
            {
                return true;
            }
        }
        
        return false;
    }

    public static List<GameObject> GetBindingGameObjectList(GameObject gameObject)
    {
        s_AlreadyBindGameObjects.Clear();

        var ret = new List<GameObject>();
        if (!gameObject.TryGetComponent(out UIPanelBase UIPanel))
            throw new Exception($"请挂载脚本：UIPanel");

        var transArray = gameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in transArray)
        {
            if (IsBindNode(child.gameObject))
            {
                var key = child.gameObject.name;
                if (s_AlreadyBindGameObjects.ContainsKey(key))
                    throw new Exception($"存在命名冲突的节点：{key}");

                ret.Add(child.gameObject);
                s_AlreadyBindGameObjects[key] = true;
            }
        }

        return ret;
    }
}
