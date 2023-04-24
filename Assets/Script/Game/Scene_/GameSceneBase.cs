using FrameWork.Core.SingletonManager;
using Game.Config;
using System;
using System.Collections;

namespace Game.Scene
{
    public struct PreLoadResData
    {
        public string Path;
        public ushort Count;

        public PreLoadResData(string path, ushort count)
        {
            this.Path = path;
            this.Count = count;
        }
    }

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

