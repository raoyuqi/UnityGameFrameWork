using FrameWork.Core.Mixin;
using FrameWork.Core.Service;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SingletonManager
{
    public sealed class GameServiceManager : SingletonBase<GameServiceManager>
    {
        private Dictionary<string, IGameService> m_ServiceContainer = new Dictionary<string, IGameService>();

        public T CreateGameService<T>() where T : IGameService, new()
        {
            var serviceName = typeof(T).Name;
            if (this.m_ServiceContainer.ContainsKey(serviceName))
            {
                Debug.LogError($"游戏服务已存在：{ serviceName }");
                return default(T);
            }

            var service = new T();
            this.m_ServiceContainer.Add(serviceName, service);
            return service;
        }

        public T GetGameService<T>() where T : IGameService
        {
            var serviceName = typeof(T).Name;
            if (!this.m_ServiceContainer.ContainsKey(serviceName))
            {
                Debug.LogError($"游戏服务不存在：{ serviceName }");
                return default(T);
            }

            return (T)this.m_ServiceContainer[serviceName];
        }
    }
}