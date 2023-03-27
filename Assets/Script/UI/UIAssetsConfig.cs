using System.Collections.Generic;

/// <summary>
/// UI 配置 TODO：改成.Text文件解析
/// </summary>
public static class UIAssetsConfig
{
    public static Dictionary<string, string> PathConfit = new Dictionary<string, string>()
    {
        { "LoadingPanel", "Prefabs/Loading/LoadingPanel.prefab" },
        { "LoginPanel", "Prefabs/Login/LoginPanel.prefab" },
        { "MainPanel", "Prefabs/Main/MainPanel.prefab" },
    };
}
