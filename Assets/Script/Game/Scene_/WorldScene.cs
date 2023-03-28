using FrameWork.Core.SingletonManager;
using Game.Config;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Scene
{
    public sealed class WorldScene : GameSceneBase
    {
        public override IEnumerator PreLoad()
        {
            var preLoadRes = "prefabs/nature/rock_02.prefab";
            // 预加载100个
            var total = 100;
            for (int i = 0; i < total; i++)
            {
                var prefab = ResourceManager.Instance.Load<GameObject>(preLoadRes);
                if (prefab != null)
                    AppConst.DefaultGameObjectPool.CreateGameObject(prefab);

                var progress = (float)Math.Round((float)i / total, 2);
                base.TriggerPreLoadingEvent(progress);
                yield return null;
            }
        }

        public override void Enter()
        {

        }
    }
}
