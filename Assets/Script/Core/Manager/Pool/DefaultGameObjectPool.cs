using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.Manager
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

        // 回收利用池
        private Dictionary<int, GameObject> m_RecyclePool = new Dictionary<int, GameObject>();
        // 使用中的对象
        private Dictionary<int, GameObject> m_UsingPool = new Dictionary<int, GameObject>();

        public GameObject GetGameObject()
        {
            if (this.m_RecyclePool.Count == 0)
            {
                // TODO: 创建一个对象并加入UsingPool，需要使用资源加载模块
            }

            // 从对象池中取出一个物体
            var objectIdList = new List<int>();
            objectIdList.AddRange(this.m_RecyclePool.Keys);
            var objectId = objectIdList[0];
            var gameObject = this.m_RecyclePool[objectId];
            this.m_RecyclePool.Remove(objectId);

            if (gameObject == null)
            {
                Debug.LogError("GetGameObject 加载失败");
                return gameObject;
            }

            this.AddToUsingPool(objectId, gameObject);
            return gameObject;
        }

        public void RecycleObject(GameObject gameObject)
        {
            var objectId = gameObject.GetInstanceID();
            if (this.m_RecyclePool.ContainsKey(objectId))
            {
                Debug.LogError($"RecyclePool 中已存在 key = {objectId} 的gameObject");
                return;
            }

            GameObject go = null;
            var ok = this.TryRemoveGameObjectFromUsingPool(objectId, out go);
            if (ok)
            {
                go.SetActive(false);
                this.m_RecyclePool.Add(objectId, go);
            }
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            GameObject go = null;
            var objectId = gameObject.GetInstanceID();
            var ok = this.TryRemoveGameObjectFromUsingPool(objectId, out go);
            if (ok)
            {
                UnityEngine.Object.Destroy(go);
                // 考虑是否触发回调
                // TODO: 删除引用计数
            }
        }


        public void Clean()
        {
            foreach (var item in this.m_RecyclePool)
            {
                var go = item.Value;
                UnityEngine.Object.Destroy(go);
                // 考虑是否触发回调
                // TODO: 删除引用计数
            }
            this.m_RecyclePool.Clear();
        }

        private bool TryRemoveGameObjectFromUsingPool(int objectId, out GameObject gameObject)
        {
            gameObject = null;
            if (!this.IsExistInUsingPool(gameObject))
                return false;

            gameObject = this.m_UsingPool[objectId];
            this.m_UsingPool.Remove(objectId);
            return true;
        }

        private void AddToUsingPool(int key, GameObject gameObject)
        {
            if (this.m_UsingPool.ContainsKey(key))
            {
                Debug.LogError($"UsingPool 中已存在 key = {key} 的gameObject");
                return;
            }

            if (gameObject == null)
            {
                Debug.LogError("不允许把空物体加入到 UsingPool");
                return;
            }

            this.m_UsingPool.Add(key, gameObject);
        }

        //判断对象是否在使用中
        private bool IsExistInUsingPool(GameObject go)
        {
            var key = go.GetInstanceID();
            if (!this.m_UsingPool.ContainsKey(key))
            {
                Debug.LogError($"UsingPool 中不存在 objectId = {key} 的gameObject");
                return false;
            }

            return true;
        }
    }
}