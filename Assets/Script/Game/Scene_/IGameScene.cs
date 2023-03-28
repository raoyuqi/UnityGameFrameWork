using System;
using System.Collections;

namespace Game.Scene
{
    public interface IGameScene
    {
        event Action<float> PreLoadResourceCallBack;

        string Name { get; set; }
        IEnumerator PreLoad();
        void Enter();
        void Exite();
    }
}