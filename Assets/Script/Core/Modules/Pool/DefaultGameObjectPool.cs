using FrameWork.Core.SingletonManager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FrameWork.Core.Modules.Pool
{
    public class DefaultGameObjectPool : IGameObjectPool
    {
        private int m_Size;
        public int Size
        {
            get { return this.m_Size; }
            set
            {
                if (value < 0)
                    throw new Exception("GameObjectPool Size 不能为负数");

                this.m_Size = value;
            }
        }

        private Transform m_PoolRoot;
        public Transform PoolRoot
        {
            get
            {
                if (this.m_PoolRoot == null)
                {
                    var root = new GameObject("[ObjectPool]");
                    this.m_PoolRoot = root.transform;
                    if (Application.isPlaying)
                        UnityObject.DontDestroyOnLoad(this.m_PoolRoot);
                }
                return this.m_PoolRoot;
            }
        }

        // 回收利用池
        private Dictionary<string, Queue<GameObject>> m_RecyclePool = new Dictionary<string, Queue<GameObject>>();

        public void CreateGameObject(GameObject prefab)
        {
            var poolName = prefab.name.Replace("(Clone)", "");
            if (!this.m_RecyclePool.ContainsKey(poolName))
                this.m_RecyclePool.Add(poolName, new Queue<GameObject>());

            var gameObject = UnityObject.Instantiate(prefab);
            gameObject.SetActive(false);
            gameObject.transform.SetParent(this.PoolRoot);
            this.m_RecyclePool[poolName].Enqueue(gameObject);
        }

        /// <summary>
        /// 使用预制体获取一个对象
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns></returns>
        public GameObject GetGameObject(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("GetGameObject 加载失败, prefab is null");
                return null;
            }

            var poolName = prefab.name.Replace("(Clone)", "");
            if (!this.m_RecyclePool.ContainsKey(poolName) || this.m_RecyclePool[poolName].Count == 0)
                this.CreateGameObject(prefab);

            //this.m_RecyclePool.Add(poolName, new Queue<GameObject>());

            //GameObject gameObject = null;
            //var pools = this.m_RecyclePool[poolName];
            //if (pools.Count == 0)
            //{
            //    gameObject = UnityObject.Instantiate(prefab);
            //    gameObject.SetActive(true);
            //    gameObject.transform.SetParent(this.PoolRoot);
            //    return gameObject;
            //}

            // 从对象池中取出一个物体
            var pools = this.m_RecyclePool[poolName];
            var gameObject = pools.Dequeue();
            gameObject.SetActive(true);
            if (gameObject == null)
            {
                Debug.LogError("GetGameObject 加载失败");
                return gameObject;
            }

            return gameObject;
        }

        /// <summary>
        /// 将物体放回对象池
        /// </summary>
        /// <param name="gameObject"></param>
        public void RecycleObject(GameObject gameObject)
        {
            var poolName = gameObject.name.Replace("(Clone)", "");
            if (!this.m_RecyclePool.ContainsKey(poolName))
            {
                Debug.LogError($"RecyclePool failed: 对象池不存在 poolName = {poolName}");
                return;
            }

            if (gameObject == null)
            {
                Debug.LogError("不允许把空物体加入到 RecyclePool");
                return;
            }

            gameObject.SetActive(false);
            gameObject.transform.SetParent(this.PoolRoot);
            this.m_RecyclePool[poolName].Enqueue(gameObject);
        }

        /// <summary>
        /// 销毁物体，且不放回对象池
        /// </summary>
        /// <param name="gameObject"></param>
        public void DestroyGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Debug.LogError("不允许销毁空物体");
                return;
            }

            this.Free(gameObject);
            UnityObject.Destroy(gameObject);
        }

        /// <summary>
        /// 清理所有对象池
        /// </summary>
        public void Clean()
        {
            foreach (var item in this.m_RecyclePool)
            {
                while (item.Value.Count > 0)
                {
                    var gameObject = item.Value.Dequeue();
                    this.Free(gameObject);
                    UnityObject.Destroy(gameObject);
                }
            }
            this.m_RecyclePool.Clear();
        }

        /// <summary>
        /// 清理指定的对象池
        /// </summary>
        /// <param name="poolName"></param>
        public void Clean(string poolName)
        {
            if (this.m_RecyclePool.TryGetValue(poolName, out Queue<GameObject> pool))
            {
                while (pool.Count > 0)
                {
                    var gameObject = pool.Dequeue();
                    this.Free(gameObject);
                    UnityObject.Destroy(gameObject);
                }
            }
            this.m_RecyclePool.Remove(poolName);
        }

        /// <summary>
        /// 判断物体是否在对象池中
        /// </summary>
        /// <param name="gameObject">需要确定的物体</param>
        /// <returns></returns>
        public bool IsExistInPool(GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            var poolName = gameObject.name.Replace("(Clone)", "");
            if (!this.m_RecyclePool.ContainsKey(poolName))
                return false;

            return this.m_RecyclePool[poolName].Contains(gameObject);
        }

        private void Free(GameObject gameObject)
        {
            var objName = gameObject.name.Replace("(Clone)", "");
            ResourceManager.Instance.FreeRefCountByName(objName);
        }
    }
}