using System.Text;
using UnityEngine;

namespace FrameWork.Core.Mixin
{
    /// <summary>
    /// 单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBase<T> where T : class, new()
    {
        private readonly static object s_LockObj = new object();

        private static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_LockObj)
                    {
                        if (s_Instance == null)
                            s_Instance = new T();
                    }
                }
                return s_Instance;
            }
        }
    }

    /// <summary>
    /// Mono单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingletoBase<T> : MonoBehaviour where T : MonoSingletoBase<T>
    {
        private readonly static object s_LockObj = new object();

        private static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_LockObj)
                    {
                        var objName = new StringBuilder("[").AppendFormat("{0}{1}", typeof(T).Name, "]").ToString();
                        var obj = new GameObject(objName);
                        s_Instance = obj.AddComponent<T>();
                        s_Instance.OnInit();
                    }
                }
                return s_Instance;
            }
        }

        protected virtual void OnInit() { }
    }
}