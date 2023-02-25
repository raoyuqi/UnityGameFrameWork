namespace Core.UI
{
    /// <summary>
    /// 面板的生命周期
    /// </summary>
    public interface IUIPanel
    {
        //创建出来时调用，调用时机在OnOpen之前
        void OnInit();

        //每次打开时调用
        void OnOpen();

        //隐藏时调用
        void OnHide();

        //关闭(销毁)时调用
        void OnClose();
    }
}
