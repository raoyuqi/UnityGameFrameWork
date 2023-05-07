using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate.SceneObject_
{
    /// <summary>
    /// 需要添加到场景四叉树中的物体需要实现该接口
    /// </summary>
    public interface ISceneObject
    {
        // 物体包围盒
        Bounds Bounds { get; }

        bool OnShow(Transform parent);

        void OnHide();
    }

    public interface ISOLinkedListNode
    {

        Dictionary<uint, object> Nodes { get; }

        LinkedListNode<T> GetLinkedListNode<T>(uint morton) where T : ISceneObject;

        void SetLinkedListNode<T>(uint morton, LinkedListNode<T> node);
    }
}
