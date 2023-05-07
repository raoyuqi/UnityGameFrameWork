using FrameWork.Core.SceneSeparate.Detector;
using FrameWork.Core.SceneSeparate.SceneObject_;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate.Tree
{
    public abstract class LinearSceneTree<T> : ITree<T> where T : ISceneObject, ISOLinkedListNode
	{
		public Bounds Bounds
		{
			get { return m_Bounds; }
		}

		public int MaxDepth
		{
			get { return m_MaxDepth; }
		}

		protected int m_MaxDepth;

		protected Bounds m_Bounds;

		//使用Morton码索引的节点字典
		protected Dictionary<uint, LinearSceneTreeLeaf<T>> m_Nodes;

		protected int m_Cols;

		public LinearSceneTree(Vector3 center, Vector3 size, int maxDepth)
		{
			this.m_MaxDepth = maxDepth;
			this.m_Bounds = new Bounds(center, size);

			this.m_Cols = (int)Mathf.Pow(2, maxDepth);
			this.m_Nodes = new Dictionary<uint, LinearSceneTreeLeaf<T>>();
		}

		public void Clear()
		{
			this.m_Nodes.Clear();
		}

		public bool Contains(T item)
		{
			if (this.m_Nodes == null)
				return false;

			foreach (var node in this.m_Nodes)
			{
				if (node.Value != null && node.Value.Contains(item))
					return true;
			}

			return false;
		}

		public void Remove(T item)
		{
			if (item == null)
				return;

			if (this.m_Nodes == null)
				return;

			var nodes = item.Nodes;
			if (nodes == null)
				return;

			foreach (var node in nodes)
			{
				if (this.m_Nodes.ContainsKey(node.Key))
				{
					var n = this.m_Nodes[node.Key];
					if (n != null && n.Datas != null)
					{
						var value = (LinkedListNode<T>)node.Value;
						if (value.List == n.Datas)
							n.Datas.Remove(value);
					}
				}
			}

			nodes.Clear();
		}

		public abstract void Add(T item);

		public abstract void Trigger(IDetector detector, TriggerHandle<T> handle);

#if UNITY_EDITOR
		public abstract void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj);
#endif
	}

	public class LinearSceneTreeLeaf<T> where T : ISceneObject, ISOLinkedListNode
	{
		private LinkedList<T> m_DataList;
		public LinkedList<T> Datas
		{
			get { return m_DataList; }
		}

		public LinearSceneTreeLeaf()
		{
			this.m_DataList = new LinkedList<T>();
		}

		public LinkedListNode<T> Insert(T obj)
		{
			return this.m_DataList.AddFirst(obj);
		}

		public void Trigger(IDetector detector, TriggerHandle<T> handle)
		{
			if (handle != null)
			{
				LinkedListNode<T> node = this.m_DataList.First;
				while (node != null)
				{
					if (detector.IsDetected(node.Value.Bounds))
						handle(node.Value);

					node = node.Next;
				}
			}
		}

		public bool Contains(T item)
		{
			if (this.m_DataList != null && this.m_DataList.Contains(item))
				return true;

			return false;
		}

#if UNITY_EDITOR
		public bool DrawNode(Color objColor, Color hitObjColor, bool drawObj)
		{
			if (drawObj && this.m_DataList.Count > 0)
			{
				LinkedListNode<T> node = m_DataList.First;

				while (node != null)
				{
					var sceneobj = node.Value as SceneObject;
					if (sceneobj != null)
						sceneobj.DrawArea(objColor, hitObjColor);

					node = node.Next;
				}
				return true;
			}

			return false;
		}
#endif
	}
}