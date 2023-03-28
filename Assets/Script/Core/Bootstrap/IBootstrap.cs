using System.Collections;

namespace FrameWork.Core.Bootstrap
{
    public enum BootstrapMode
    {
        Editor,
        Mobile
    }

    public enum Status
    {
        Success,
        Failed
    }

    /// <summary>
    /// 启动流程
    /// </summary>
    public interface IBootstrap
    {
        Status Status { get; }

        BootstrapMode BootstrapMode { get; }

        IEnumerator BootstrapAsync();
    }
}