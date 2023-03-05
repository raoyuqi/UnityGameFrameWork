using FrameWork.Core.Mixin;

namespace FrameWork.Core.Manager
{
    /// <summary>
    /// 统一管理MonoBehaviourRuntime
    /// </summary>
    public class MonoBehaviourRuntime : MonoSingletoBase<MonoBehaviourRuntime>
    {
        protected override void OnInit()
        {
            DontDestroyOnLoad(this);
        }
    }
}