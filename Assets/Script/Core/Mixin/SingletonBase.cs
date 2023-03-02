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
}