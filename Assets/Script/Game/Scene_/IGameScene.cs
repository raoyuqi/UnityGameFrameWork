namespace Game.Scene
{
    public interface IGameScene
    {
        string Name { get; set; }
        void Enter();
        void Exite();
    }
}