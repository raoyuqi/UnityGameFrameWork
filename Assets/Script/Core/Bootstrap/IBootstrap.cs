using System.Collections;

namespace FrameWork.Core.Bootstrap
{
    public enum BootstrapMode
    {
        Editor,
        Mobile
    }

    /// <summary>
    /// 启动流程
    /// </summary>
    public interface IBootstrap
    {
        BootstrapMode BootstrapMode { get; }

        IEnumerator BootstrapAsync();
    }
}