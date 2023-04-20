using FrameWork.Core.SceneSeparate.Detector;
using FrameWork.Core.SceneSeparate.SceneObject_;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameWork.Core.SceneSeparate.Tree
{
    public class LinearSceneQuadTree<T> : LinearSceneTree<T> where T : ISceneObject, ISOLinkedListNode
    {
        private float m_DeltaWidth;
        private float m_DeltaHeight;


        public LinearSceneQuadTree(Vector3 center, Vector3 size, int maxDepth) : base(center, size, maxDepth)
        {
            m_DeltaWidth = m_Bounds.size.x / m_Cols;
            m_DeltaHeight = m_Bounds.size.z / m_Cols;
        }

        public override void Add(T item)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
        {
            throw new System.NotImplementedException();
        }

        public override void Trigger(IDetector detector, TriggerHandle<T> handle)
        {
            throw new System.NotImplementedException();
        }
    }
}