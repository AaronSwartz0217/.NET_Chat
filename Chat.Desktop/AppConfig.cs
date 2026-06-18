namespace Chat.Desktop;

/// <summary>
/// 应用配置 - 管理服务器连接地址
/// 部署时修改此处的默认值即可，无需改代码
/// </summary>
public static class AppConfig
{
    /// <summary>
    /// 后端REST API基础地址（端口5002）
    /// 本地开发: http://localhost:5002
    /// 局域网/公网: http://你的IP:5002  (如 http://192.168.1.100:5002)
    /// </summary>
    public static string ApiBaseUrl { get; set; } = "http://localhost:5002";

    /// <summary>
    /// WebSocket服务地址（端口5003）
    /// 本地开发: ws://localhost:5003
    /// 局域网/公网: ws://你的IP:5003  (如 ws://192.168.1.100:5003)
    /// </summary>
    public static string WsUrl { get; set; } = "ws://localhost:5003";

    /// <summary>
    /// 设置服务器地址（部署到Linux/Android前调用）
    /// </summary>
    public static void SetServerAddress(string ipOrDomain)
    {
        ApiBaseUrl = $"http://{ipOrDomain}:5002";
        WsUrl = $"ws://{ipOrDomain}:5003";
    }

    /// <summary>
    /// 重置为本地开发环境
    /// </summary>
    public static void ResetToLocalhost()
    {
        ApiBaseUrl = "http://localhost:5002";
        WsUrl = "ws://localhost:5003";
    }
}
