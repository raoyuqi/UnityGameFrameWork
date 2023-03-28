using UnityEngine;

namespace FrameWork.Core.Modules.Pool
{
    public interface IGameObjectPool
    {
        int Size { get; set; }

        // 创建物体并添加到对象池
        void CreateGameObject(GameObject prefab);

        // 从对象池取出物体
        GameObject GetGameObject(GameObject prefab);

        // 归还对象池
        void RecycleObject(GameObject gameObject);

        // 销毁物体，且不归还
        void DestroyGameObject(GameObject gameObject);

        // 清空所有对象池
        void Clean();

        // 清空指定对象池
        void Clean(string poolName);
    }
}