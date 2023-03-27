using FrameWork.Core.SingletonManager;

namespace Game.Scene
{
    public sealed class MainScene : IGameScene
    {
        public string Name { get; set; }

        public void Enter()
        {
            UIManager.Instance.OpenPanel<MainPanel>();
        }

        public void Exite()
        {
            
        }
    }
}
