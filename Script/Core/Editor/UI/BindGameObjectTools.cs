using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 绑定控件节点工具类
/// </summary>
public static class BindGameObjectTools
{
    /// <summary>
    /// 需要绑定的节点命名前缀
    /// </summary>
    private static readonly string[] s_BindNamePrefix = new string[]
    {
        // 按钮
        "Btn_",
        "Button",

        // 文本
        "Txt",
        "Text",

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

    public static List<GameObject> GetBindingGameObjects(GameObject gameObject)
    {
        var ret = new List<GameObject>();
        //if (!gameObject.TryGetComponent<IUIPanel>())
        //{
        //    throw new Exception($"请挂载脚本：UIPanel");
        //    return ret;
        //}

        var parent = gameObject.transform;
        foreach (Transform child in parent)
        {

        }
        return ret;
    }
}
