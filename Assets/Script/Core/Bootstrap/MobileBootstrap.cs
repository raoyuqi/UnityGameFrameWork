using FrameWork.Core.HotUpdate;
using FrameWork.Core.SingletonManager;
using System.Collections;

namespace FrameWork.Core.Bootstrap
{
    public class MobileBootstrap : IBootstrap
    {
        private HotUpdateHandler m_HotUpdateHandler = HotUpdateHandler.Instance;

        public BootstrapMode BootstrapMode { get => BootstrapMode.Mobile; }

        public IEnumerator BootstrapAsync()
        {
            ResourceManager.Instance.Initialize(this.BootstrapMode);

            this.m_HotUpdateHandler.Initialize();
            yield return this.m_HotUpdateHandler.StartHotUpdateProcessAsync();
        }
    }
}
