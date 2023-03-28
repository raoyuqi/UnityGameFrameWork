using FrameWork.Core.SingletonManager;

namespace Game.Scene
{
    public sealed class MainScene : GameSceneBase
    {
        public override void Enter()
        {
            UIManager.Instance.OpenPanel<MainPanel>();
        }
    }
}
