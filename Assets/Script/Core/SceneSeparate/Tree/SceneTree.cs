using FrameWork.Core.SceneSeparate.SceneObject_;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate.Tree
{
    /// <summary>
    /// 场景树（非线性结构）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SceneTree<T> : ITree<T> where T : ISceneObject, ISOLinkedListNode
    {
        public Bounds Bounds
        {
            get
            {
                if (this.m_Root != null)
                    return this.m_Root.Bounds;

                return default(Bounds);
            }
        }

        // 最大深度
        protected int m_MaxDepth;
        public int MaxDepth
        {
            get { return this.m_MaxDepth; }
        }

        protected SceneTreeNode<T> m_Root;

        public void Add(T item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new System.NotImplementedException();
        }

#if UNITY_EDITOR
        public void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
        {
            throw new System.NotImplementedException();
        }
#endif

        public void Remove(T item)
        {
            throw new System.NotImplementedException();
        }

        public SceneTree(Vector3 center, Vector3 size, int maxDepth, bool ocTree)
        {
            this.m_MaxDepth = maxDepth;
            this.m_Root = new SceneTreeNode<T>(new Bounds(center, size), 0, ocTree ? 8 : 4);
        }
    }

    public class SceneTreeNode<T> where T : ISceneObject, ISOLinkedListNode
    {
        private Bounds m_Bounds;
        public Bounds Bounds
        {
            get { return this.m_Bounds; }
        }

        private int m_CurrentDepth;
        public int CurrentDepth
        {
            get { return this.m_CurrentDepth; }
        }

        // 节点数据列表
        private LinkedList<T> m_ObjectList;
        public LinkedList<T> ObjectList
        {
            get { return this.m_ObjectList; }
        }

        // 子节点
        private SceneTreeNode<T>[] m_ChildNodes;

        private int m_ChildCount;

        private Vector3 m_HalfSize;

        public SceneTreeNode(Bounds bounds, int depth, int childCount)
        {
            this.m_Bounds = bounds;
            this.m_CurrentDepth = depth;
            this.m_ObjectList = new LinkedList<T>();
            this.m_ChildNodes = new SceneTreeNode<T>[childCount];

            if (childCount == 8)
                this.m_HalfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y / 2, m_Bounds.size.z / 2);
            else
                this.m_HalfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y, m_Bounds.size.z / 2);

            this.m_ChildCount = childCount;
        }
    }
}