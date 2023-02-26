namespace FrameWork.Core.Mixin
{
    /// <summary>
    /// 单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBase<T> where T : class, new()
    {
        private readonly object s_LockObj = new object();

        private T s_Instance;
        public T Instance
        {
            get
            {
                if (this.s_Instance == null)
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