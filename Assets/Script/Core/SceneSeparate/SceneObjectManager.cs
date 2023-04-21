using FrameWork.Core.SceneSeparate.Detector;
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
		private Queue<SceneObject> m_PreDestroyObjectQueue;

		// 异步任务队列
		private Queue<SceneObject> m_ProcessTaskQueue;

		private TriggerHandle<SceneObject> m_TriggerHandle;

		private int m_MaxCreateCount;
		private int m_MinCreateCount;
		private float m_MaxRefreshTime;
		private float m_MaxDestroyTime;
		private bool m_Asyn;

		/// <summary>
		/// 销毁时间
		/// </summary>
		private float m_DestroyRefreshTime;

		/// <summary>
		/// 刷新时间
		/// </summary>
		private float m_RefreshTime;

		private Vector3 m_OldRefreshPosition;
		private Vector3 m_OldDestroyRefreshPosition;

		private bool m_IsTaskRunning;

		private WaitForEndOfFrame m_WaitForFrame;

		// 当前触发器
		private IDetector m_CurrentDetector;

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

			//this.m_Tree = new SceneTree<SceneObject>(center, size, quadTreeDepth, false);
			this.m_Tree = new LinearSceneQuadTree<SceneObject>(center, size, quadTreeDepth);

			this.m_LoadedObjectLinkedList = new LinkedList<SceneObject>();
            //m_PreDestroyObjectQueue = new PriorityQueue<SceneObject>(new SceneObjectWeightComparer());
			this.m_PreDestroyObjectQueue = new Queue<SceneObject>();

			this.m_TriggerHandle = new TriggerHandle<SceneObject>(this.TriggerHandle);

            this.m_MaxCreateCount = Mathf.Max(0, maxCreateCount);
			this.m_MinCreateCount = Mathf.Clamp(minCreateCount, 0, m_MaxCreateCount);
			this.m_MaxRefreshTime = maxRefreshTime;
			this.m_MaxDestroyTime = maxDestroyTime;
			this.m_Asyn = asyn;

			this.m_IsInitialized = true;

			this.m_RefreshTime = maxRefreshTime;
		}

		/// <summary>
		/// 四叉树触发处理函数
		/// </summary>
		/// <param name="data">与当前包围盒发生触发的场景物体</param>
		private void TriggerHandle(SceneObject data)
		{
			if (data == null)
				return;

			//如果发生触发的物体已经被创建则标记为新物体，以确保不会被删掉
			if (data.Flag == CreateFlag.Old)
			{
				data.Weight++;
				data.Flag = CreateFlag.New;
			}
			else if (data.Flag == CreateFlag.OutofBounds)
			{
				//如果发生触发的物体已经被标记为超出区域，则从待删除列表移除该物体，并标记为新物体
				data.Flag = CreateFlag.New;
				//if (m_PreDestroyObjectList.Remove(data))
				{
					m_LoadedObjectLinkedList.AddFirst(data);

				}
			}
			else if (data.Flag == CreateFlag.None)
			{
				//如果发生触发的物体未创建则创建该物体并加入已加载的物体列表
				this.DoCreateInternal(data);
			}
		}

		//执行创建物体
		private void DoCreateInternal(SceneObject data)
		{
			//加入已加载列表
			this.m_LoadedObjectLinkedList.AddFirst(data);
			//创建物体
			this.CreateObject(data, this.m_Asyn);
		}

		private void CreateObject(SceneObject obj, bool asyn)
		{
			if (obj == null)
				return;

			if (obj.TargetObj == null)
				return;

			if (obj.Flag == CreateFlag.None)
			{
				if (!asyn)
					CreateObjectSync(obj);
				else
					ProcessObjectAsyn(obj, true);

				//被创建的物体标记为New
				obj.Flag = CreateFlag.New;
			}
		}

		private void CreateObjectSync(SceneObject obj)
		{
			// 如果标记为IsPrepareDestroy表示物体已经创建并正在等待删除，则直接设为None并返回
			if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareDestroy)
			{
				obj.ProcessFlag = CreatingProcessFlag.None;
				return;
			}

			obj.OnShow(transform);
		}

		private void ProcessObjectAsyn(SceneObject obj, bool create)
		{
			if (create)
			{
				// 表示物体已经创建并等待销毁，则设置为None并跳过
				if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareDestroy)
				{
					obj.ProcessFlag = CreatingProcessFlag.None;
					return;
				}

				// 已经开始等待创建，则跳过
				if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareCreate)
					return;

				//设置为等待开始创建
				obj.ProcessFlag = CreatingProcessFlag.IsPrepareCreate;
			}
			else
			{
				// 表示物体未创建并等待创建，则设置为None并跳过
				if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareCreate)
				{
					obj.ProcessFlag = CreatingProcessFlag.None;
					return;
				}

				// 已经开始等待销毁，则跳过
				if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareDestroy)
					return;

				// 设置为等待开始销毁
				obj.ProcessFlag = CreatingProcessFlag.IsPrepareDestroy;
			}
			if (this.m_ProcessTaskQueue == null)
				m_ProcessTaskQueue = new Queue<SceneObject>();

			this.m_ProcessTaskQueue.Enqueue(obj);
			if (!m_IsTaskRunning)
			{
				// 开始协程执行异步任务
				this.StartCoroutine(this.AsynTaskProcess());
			}
		}

		private IEnumerator AsynTaskProcess()
		{
			if (this.m_ProcessTaskQueue == null)
				yield return 0;

			this.m_IsTaskRunning = true;
			while (this.m_ProcessTaskQueue.Count > 0)
			{
				var obj = this.m_ProcessTaskQueue.Dequeue();
				if (obj != null)
				{
					// 等待创建
					if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareCreate)
					{
						obj.ProcessFlag = CreatingProcessFlag.None;
						if (obj.OnShow(transform))
						{
							if (this.m_WaitForFrame == null)
								this.m_WaitForFrame = new WaitForEndOfFrame();

							yield return this.m_WaitForFrame;
						}
					}
					else if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareDestroy)
					{
						// 等待销毁
						obj.ProcessFlag = CreatingProcessFlag.None;
						obj.OnHide();
						if (this.m_WaitForFrame == null)
							this.m_WaitForFrame = new WaitForEndOfFrame();

						yield return this.m_WaitForFrame;
					}
				}
			}

			this.m_IsTaskRunning = false;
		}

		// 添加场景物体
		public void AddSceneBlockObject(ISceneObject obj)
		{
			if (!this.m_IsInitialized)
				return;

			if (this.m_Tree == null || obj == null)
				return;

			//使用SceneObject包装
			var sceneObj = new SceneObject(obj);
			this.m_Tree.Add(sceneObj);
			//如果当前触发器存在，直接物体是否可触发，如果可触发，则创建物体
			if (this.m_CurrentDetector != null && this.m_CurrentDetector.IsDetected(sceneObj.Bounds))
				this.DoCreateInternal(sceneObj);
		}

		// 刷新触发器
		public void RefreshDetector(IDetector detector)
		{
			if (!this.m_IsInitialized)
				return;

			//只有坐标发生改变才调用
			if (this.m_OldRefreshPosition != detector.Position)
			{
				this.m_RefreshTime += Time.deltaTime;

				//达到刷新时间才刷新，避免区域更新频繁
				if (this.m_RefreshTime > this.m_MaxRefreshTime)
				{
					this.m_OldRefreshPosition = detector.Position;
					this.m_RefreshTime = 0;
					this.m_CurrentDetector = detector;
					//进行触发检测
					this.m_Tree.Trigger(detector, this.m_TriggerHandle);
					//标记超出区域的物体
					this.MarkOutofBoundsObjs();
					//m_IsInitLoadComplete = true;
				}
			}
			if (this.m_OldDestroyRefreshPosition != detector.Position)
			{
				if (this.m_PreDestroyObjectQueue != null && this.m_PreDestroyObjectQueue.Count >= this.m_MaxCreateCount && this.m_PreDestroyObjectQueue.Count > this.m_MinCreateCount)
				//if (m_PreDestroyObjectList != null && m_PreDestroyObjectList.Count >= m_MaxCreateCount)
				{
					this.m_DestroyRefreshTime += Time.deltaTime;
					if (this.m_DestroyRefreshTime > this.m_MaxDestroyTime)
					{
						this.m_OldDestroyRefreshPosition = detector.Position;
						this.m_DestroyRefreshTime = 0;
						this.DestroyOutOfBoundsObjs();
					}
				}
			}
		}

		// 标记离开视野的物体
		void MarkOutofBoundsObjs()
		{
			if (this.m_LoadedObjectLinkedList == null)
				return;

			var node = this.m_LoadedObjectLinkedList.First;
			while (node != null)
			{
				var obj = node.Value;
				// 已加载物体标记仍然为Old，说明该物体没有进入触发区域，即该物体在区域外
				if (obj.Flag == CreateFlag.Old)
				{
					obj.Flag = CreateFlag.OutofBounds;
					//如果最小创建数为0直接删除
					if (this.m_MinCreateCount == 0)
						this.DestroyObject(obj, this.m_Asyn);
					else
						this.m_PreDestroyObjectQueue.Enqueue(obj);

					var next = node.Next;
					this.m_LoadedObjectLinkedList.Remove(node);
					node = next;
				}
				else
				{
					obj.Flag = CreateFlag.Old;
					node = node.Next;
				}
			}
		}

		/// <summary>
		/// 删除超出区域外的物体
		/// </summary>
		void DestroyOutOfBoundsObjs()
		{
			while (this.m_PreDestroyObjectQueue.Count > this.m_MinCreateCount)
			{

				var obj = m_PreDestroyObjectQueue.Dequeue();
				if (obj == null)
					continue;

				if (obj.Flag == CreateFlag.OutofBounds)
				{
					this.DestroyObject(obj, m_Asyn);
				}
			}
		}

		// 删除物体
		private void DestroyObject(SceneObject obj, bool asyn)
		{
			if (obj == null || obj.Flag == CreateFlag.None || obj.TargetObj == null)
				return;

			if (!asyn)
				this.DestroyObjectSync(obj);
			else
				this.ProcessObjectAsyn(obj, false);

			// 被删除的物体标记为None
			obj.Flag = CreateFlag.None;
		}
		
		private void DestroyObjectSync(SceneObject obj)
		{
			// 如果物体标记为IsPrepareCreate表示物体未创建并正在等待创建，则直接设为None并放回
			if (obj.ProcessFlag == CreatingProcessFlag.IsPrepareCreate)
			{
				obj.ProcessFlag = CreatingProcessFlag.None;
				return;
			}

			obj.OnHide();
		}

		void OnDestroy()
		{
			if (this.m_Tree != null)
				this.m_Tree.Clear();

			m_Tree = null;
			if (this.m_ProcessTaskQueue != null)
				this.m_ProcessTaskQueue.Clear();

			if (this.m_LoadedObjectLinkedList != null)
				this.m_LoadedObjectLinkedList.Clear();

			this.m_ProcessTaskQueue = null;
			this.m_LoadedObjectLinkedList = null;
			m_TriggerHandle = null;
		}

#if UNITY_EDITOR
        public int debug_DrawMinDepth = 0;
        public int debug_DrawMaxDepth = 5;
        public bool debug_DrawObj = true;
        void OnDrawGizmosSelected()
        {
            var mindcolor = new Color32(0, 66, 255, 255);
            var maxdcolor = new Color32(133, 165, 255, 255);
            var objcolor = new Color32(0, 210, 255, 255);
            var hitcolor = new Color32(255, 216, 0, 255);
            if (this.m_Tree != null)
                this.m_Tree.DrawTree(mindcolor, maxdcolor, objcolor, hitcolor, debug_DrawMinDepth, debug_DrawMaxDepth, debug_DrawObj);
        }
#endif
    }
}