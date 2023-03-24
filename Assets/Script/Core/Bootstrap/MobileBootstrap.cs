using FrameWork.Core.HotUpdate;
using FrameWork.Core.SingletonManager;
using System.Collections;

namespace FrameWork.Core.Bootstrap
{
    public class MobileBootstrap : IBootstrap
    {
        private HotUpdateHandler m_HotUpdateHandler = HotUpdateHandler.Instance;

        public BootstrapMode BootstrapMode { get => BootstrapMode.Mobile; }

        private Status m_Status = Status.Success;
        public Status Status { get => this.m_Status; }

        public MobileBootstrap()
        {
            this.m_HotUpdateHandler.UpdateFailedCallback += (msg) => { this.m_Status = Status.Failed; };
        }

        public IEnumerator BootstrapAsync()
        {
            ResourceManager.Instance.Initialize(this.BootstrapMode);

            this.m_HotUpdateHandler.Initialize();
            yield return this.m_HotUpdateHandler.StartHotUpdateProcessAsync();
        }
    }
}
