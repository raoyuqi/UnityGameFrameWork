using UnityEngine;

namespace FrameWork.Core.Manager
{
    public interface IGameObjectPool
    {
        int Size { get; set; }

        // 从对象池取出物体
        GameObject GetGameObject();

        // 归还对象池
        void RecycleObject(GameObject gameObject);

        // 销毁物体，且不归还
        void DestroyGameObject(GameObject gameObject);

        // 清空对象池
        void Clean();
    }
}