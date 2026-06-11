using System.Text.Json.Serialization;

namespace Chat.Application.Dtos;

/// <summary>
/// WebSocket消息协议
/// </summary>
public class WsMessage
{
    /// <summary>
    /// 消息类型: chat(聊天), system(系统), online(上线), offline(下线), typing(输入中), read(已读)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "chat";

    /// <summary>
    /// 发送者用户ID
    /// </summary>
    [JsonPropertyName("fromUserId")]
    public int FromUserId { get; set; }

    /// <summary>
    /// 发送者用户名
    /// </summary>
    [JsonPropertyName("fromUserName")]
    public string FromUserName { get; set; } = string.Empty;

    /// <summary>
    /// 目标用户ID（私聊时使用，群聊/广播为null）
    /// </summary>
    [JsonPropertyName("toUserId")]
    public int? ToUserId { get; set; }

    /// <summary>
    /// 频道ID（会话ID）
    /// </summary>
    [JsonPropertyName("channelId")]
    public int? ChannelId { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 关联的消息数据库ID（服务端填充）
    /// </summary>
    [JsonPropertyName("messageId")]
    public int? MessageId { get; set; }
}

/// <summary>
/// WebSocket连接认证请求
/// </summary>
public class WsAuthRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "auth";

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// WebSocket在线用户信息
/// </summary>
public class WsOnlineUser
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("onlineTime")]
    public DateTime OnlineTime { get; set; } = DateTime.UtcNow;
}
