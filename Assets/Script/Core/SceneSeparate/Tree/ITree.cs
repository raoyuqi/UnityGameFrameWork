using FrameWork.Core.SceneSeparate.SceneObject_;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate.Tree
{
    public delegate void TriggerHandle<T>(T trigger);

    public interface ITree<T> where T : ISceneObject
    {
        // 根节点包围盒
        Bounds Bounds { get; }

        // 最大深度
        int MaxDepth { get; }

        void Add(T item);

        void Clear();

        bool Contains(T item);

        void Remove(T item);

        //void Trigger(IDetector detector, TriggerHandle<T> handle);

#if UNITY_EDITOR
        void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth,
            int drawMaxDepth, bool drawObj);
#endif
    }
}