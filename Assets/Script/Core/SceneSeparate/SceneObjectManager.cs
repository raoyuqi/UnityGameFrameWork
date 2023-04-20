using FrameWork.Core.SceneSeparate.SceneObject_;
using FrameWork.Core.SceneSeparate.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate
{
	public enum TreeType
	{
		/// <summary>
		/// 线性四叉树
		/// </summary>
		LinearQuadTree,
		/// <summary>
		/// 线性八叉树
		/// </summary>
		LinearOcTree,
		/// <summary>
		/// 四叉树
		/// </summary>
		QuadTree,
		/// <summary>
		/// 八叉树
		/// </summary>
		OcTree,
	}

	public class SceneObjectManager : MonoBehaviour
    {
		private bool m_IsInitialized;

		// 当前场景资源四叉树/八叉树
		private ITree<SceneObject> m_Tree;

		/// <summary>
		/// 已加载的物体列表（频繁移除与添加使用双向链表）
		/// </summary>
		private LinkedList<SceneObject> m_LoadedObjectLinkedList;

		/// <summary>
		/// 待销毁物体列表
		/// </summary>
		//private PriorityQueue<SceneObject> m_PreDestroyObjectQueue;

		private int m_MaxCreateCount;
		private int m_MinCreateCount;
		private float m_MaxRefreshTime;
		private float m_MaxDestroyTime;
		private bool m_Asyn;

		/// <summary>
		/// 刷新时间
		/// </summary>
		private float m_RefreshTime;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="center">场景区域中心</param>
		/// <param name="size">场景区域大小</param>
		/// <param name="asyn">是否异步</param>
		public void Init(Vector3 center, Vector3 size, bool asyn, TreeType treeType)
		{
			this.Init(center, size, asyn, 25, 15, 1, 5, treeType);
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="center">场景区域中心</param>
		/// <param name="size">场景区域大小</param>
		/// <param name="asyn">是否异步</param>
		/// <param name="maxCreateCount">最大创建数量</param>
		/// <param name="minCreateCount">最小创建数量</param>
		/// <param name="maxRefreshTime">更新区域时间间隔</param>
		/// <param name="maxDestroyTime">检查销毁时间间隔</param>
		/// <param name="quadTreeDepth">四叉树深度</param>
		private void Init(Vector3 center, Vector3 size, bool asyn, int maxCreateCount, int minCreateCount, float maxRefreshTime, float maxDestroyTime, TreeType treeType, int quadTreeDepth = 5)
		{
			if (this.m_IsInitialized)
				return;

			//switch (treeType)
			//{
			//	case TreeType.LinearOcTree:
			//		this.m_Tree = new LinearSceneOcTree<SceneObject>(center, size, quadTreeDepth);
			//		break;
			//	case TreeType.LinearQuadTree:
			//		this.m_Tree = new LinearSceneQuadTree<SceneObject>(center, size, quadTreeDepth);
			//		break;
			//	case TreeType.OcTree:
			//		this.m_Tree = new SceneTree<SceneObject>(center, size, quadTreeDepth, true);
			//		break;
			//	case TreeType.QuadTree:
			//		this.m_Tree = new SceneTree<SceneObject>(center, size, quadTreeDepth, false);
			//		break;
			//	default:
			//		this.m_Tree = new LinearSceneQuadTree<SceneObject>(center, size, quadTreeDepth);
			//		break;
			//}

			this.m_Tree = new SceneTree<SceneObject>(center, size, quadTreeDepth, false);

			this.m_LoadedObjectLinkedList = new LinkedList<SceneObject>();
			//m_PreDestroyObjectQueue = new PriorityQueue<SceneObject>(new SceneObjectWeightComparer());
			//m_TriggerHandle = new TriggerHandle<SceneObject>(this.TriggerHandle);

			this.m_MaxCreateCount = Mathf.Max(0, maxCreateCount);
			this.m_MinCreateCount = Mathf.Clamp(minCreateCount, 0, m_MaxCreateCount);
			this.m_MaxRefreshTime = maxRefreshTime;
			this.m_MaxDestroyTime = maxDestroyTime;
			this.m_Asyn = asyn;

			this.m_IsInitialized = true;

			this.m_RefreshTime = maxRefreshTime;
		}
	}
}