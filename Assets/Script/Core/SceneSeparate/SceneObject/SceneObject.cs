using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate.SceneObject_
{
    /// <summary>
    /// 场景物件
    /// </summary>
    public sealed class SceneObject : ISceneObject, ISOLinkedListNode
    {
        private ISceneObject m_TargetObj;
        public ISceneObject TargetObj
        {
            get { return this.m_TargetObj; }
        }

        private float m_Weight;
        public float Weight
        {
            get { return this.m_Weight; }
            set { this.m_Weight = value; }
        }

        private Dictionary<uint, object> m_Nodes;
        public Dictionary<uint, object> Nodes
        {
            get { return this.m_Nodes; }
        }

        public Bounds Bounds
        {
            get
            {
                return this.m_TargetObj.Bounds;
            }
        }

        public SceneObject(ISceneObject obj)
        {
            this.m_Weight = 0;
            this.m_TargetObj = obj;
        }

        public void OnHide()
        {
            this.m_Weight = 0;
            this.m_TargetObj.OnHide();
        }

        public bool OnShow(Transform parent)
        {
            if (this.m_TargetObj == null)
                return false;

            return this.m_TargetObj.OnShow(parent);
        }

        public LinkedListNode<T> GetLinkedListNode<T>(uint morton) where T : ISceneObject
        {
            if (this.Nodes != null && this.Nodes.ContainsKey(morton))
                return (LinkedListNode<T>)this.Nodes[morton];

            return null;
        }

        public void SetLinkedListNode<T>(uint morton, LinkedListNode<T> node)
        {
            if (this.Nodes == null)
                this.m_Nodes = new Dictionary<uint, object>();

            this.Nodes[morton] = node;
        }

#if UNITY_EDITOR
        public void DrawArea(Color color, Color hitColor)
        {
            //if (Flag == CreateFlag.New || Flag == CreateFlag.Old)
            //{
            //    m_TargetObj.Bounds.DrawBounds(hitColor);
            //}
            //else
            //    m_TargetObj.Bounds.DrawBounds(color);
        }
#endif
    }
}