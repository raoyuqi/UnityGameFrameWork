using FrameWork.Core.SingletonManager;
using System.Collections;

namespace FrameWork.Core.Bootstrap
{
    public class EditorBootstrap : IBootstrap
    {
        public BootstrapMode BootstrapMode { get => BootstrapMode.Editor; }

        public Status Status { get => Status.Success; }

        public IEnumerator BootstrapAsync()
        {
            yield return null;
            ResourceManager.Instance.Initialize(this.BootstrapMode);
        }
    }
}