using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate.SceneObject_
{
    /// <summary>
    /// 场景物件创建标记
    /// </summary>
    public enum CreateFlag
    {
        /// <summary>
        /// 未创建
        /// </summary>
        None,
        /// <summary>
        /// 标记为新物体
        /// </summary>
        New,
        /// <summary>
        /// 标记为旧物体
        /// </summary>
        Old,
        /// <summary>
        /// 标记为离开视野区域
        /// </summary>
        OutofBounds,
    }

    /// <summary>
    /// 场景物体加载标记
    /// </summary>
    public enum CreatingProcessFlag
    {
        None,
        /// <summary>
        /// 准备加载
        /// </summary>
        IsPrepareCreate,
        /// <summary>
        /// 准备销毁
        /// </summary>
        IsPrepareDestroy,
    }

    /// <summary>
    /// 场景物件
    /// </summary>
    public sealed class SceneObject : ISceneObject, ISOLinkedListNode
    {
        public CreateFlag Flag { get; set; }
        public CreatingProcessFlag ProcessFlag { get; set; }

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