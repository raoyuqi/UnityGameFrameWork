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
            // 可以改成配置
            var preLoadResDataArray = new PreLoadResData[]
            {
                new PreLoadResData("prefabs/nature/tree_03.prefab", 10),
                new PreLoadResData("prefabs/nature/tree_05.prefab", 10),
            };

            // 预加载
            var total = 0;
            foreach (var preLoadResData in preLoadResDataArray)
                total += preLoadResData.Count;

            foreach (var preLoadResData in preLoadResDataArray)
            {
                for (int i = 0; i < preLoadResData.Count; i++)
                {
                    var prefab = ResourceManager.Instance.Load<GameObject>(preLoadResData.Path);
                    if (prefab != null)
                        AppConst.DefaultGameObjectPool.CreateGameObject(prefab);

                    var progress = (float)Math.Round((float)i / total, 2);
                    base.TriggerPreLoadingEvent(progress);
                    yield return null;
                }
            }
        }

        public override void Enter()
        {

        }
    }
}
