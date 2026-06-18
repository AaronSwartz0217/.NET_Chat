using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace Chat.Desktop.Models;

/// <summary>
/// 聊天消息模型
/// </summary>
public class ChatModel
{
    /// <summary>
    /// 发送者昵称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 发送者用户ID
    /// </summary>
    public int FromUserId { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime SendTime { get; set; }

    /// <summary>
    /// 文本对齐方式（左=他人，右=自己）
    /// </summary>
    public HorizontalAlignment TextAlignment { get; set; } = HorizontalAlignment.Left;

    /// <summary>
    /// Dock位置（左=他人，右=自己）
    /// </summary>
    public Dock TextDock { get; set; } = Dock.Left;

    /// <summary>
    /// 头像（预留）
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 消息类型：system/chat/online/offline/typing
    /// </summary>
    public string MessageType { get; set; } = "chat";

    /// <summary>
    /// 是否为系统消息
    /// </summary>
    public bool IsSystem => MessageType == "system";

    /// <summary>
    /// 是否为自己的消息（右侧显示）
    /// </summary>
    public bool IsOwnMessage => TextAlignment == HorizontalAlignment.Right;

    /// <summary>
    /// 是否为聊天消息（非系统消息）
    /// </summary>
    public bool IsChatMessage => !IsSystem;

    /// <summary>
    /// 是否为他人消息（左侧显示）
    /// </summary>
    public bool IsOthersMessage => !IsOwnMessage;

    /// <summary>
    /// 是否为在线/离线通知
    /// </summary>
    public bool IsNotification => MessageType is "online" or "offline";

    /// <summary>
    /// 格式化后的时间显示文本
    /// </summary>
    public string FormattedTime => FormatTime(SendTime);

    private static string FormatTime(DateTime time)
    {
        var span = DateTime.Now - time;
        if (span.TotalMinutes < 1) return "刚刚";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}分钟前";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours}小时前";
        if (span.Days == 1) return "昨天";
        if (span.Days < 7) return $"{span.Days}天前";
        return time.ToString("MM-dd HH:mm");
    }
}

/// <summary>
/// 在线用户模型
/// </summary>
public class OnlineUserModel
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? NickName { get; set; }
    public string? Avatar { get; set; }
    public DateTime OnlineTime { get; set; }

    public string DisplayName => !string.IsNullOrEmpty(NickName) ? NickName : UserName ?? "未知用户";
}
