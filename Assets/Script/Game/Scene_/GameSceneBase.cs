using FrameWork.Core.SingletonManager;
using Game.Config;
using System;
using System.Collections;

namespace Game.Scene
{
    public abstract class GameSceneBase : IGameScene
    {
        public string Name { get; set; }

        public event Action<float> PreLoadResourceCallBack;

        public virtual IEnumerator PreLoad()
        {
            yield break;
        }

        public void TriggerPreLoadingEvent(float progress)
        {
            this.PreLoadResourceCallBack?.Invoke(progress);
        }

        public virtual void Enter()
        {

        }

        public virtual void Exite()
        {
            UIManager.Instance.DestroyAllPanel();
            AppConst.DefaultGameObjectPool.Clean();
            this.PreLoadResourceCallBack = null;
        }
    }
}

