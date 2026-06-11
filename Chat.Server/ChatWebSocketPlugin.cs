using Chat.Application.Dtos;
using Chat.Application.Services;
using Chat.Core.Models;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using TouchSocket.Core;
using TouchSocket.Http.WebSockets;

namespace Chat.Server;

/// <summary>
/// WebSocket聊天处理插件
/// 支持JWT认证、私聊/群聊、在线状态管理
/// 仅使用 IWebSocketReceivedPlugin（兼容TouchSocket.Http 2.0.2）
/// 通过Furion.App解析依赖
/// </summary>
public class ChatWebSocketPlugin : PluginBase,
    IWebSocketReceivedPlugin
{
    /// <summary>
    /// 在线用户映射: userId -> WebSocket连接
    /// </summary>
    private static readonly ConcurrentDictionary<int, IWebSocket> _onlineUsers = new();

    /// <summary>
    /// 连接认证信息: socketId -> userId
    /// </summary>
    private static readonly ConcurrentDictionary<string, int> _authConnections = new();

    // 通过Furion全局容器延迟解析依赖
    private static ILogger<ChatWebSocketPlugin> Logger => Furion.App.GetRequiredService<ILogger<ChatWebSocketPlugin>>();
    private static ISqlSugarClient Db => Furion.App.GetRequiredService<ISqlSugarClient>();
    private static IJwtService JwtService => Furion.App.GetRequiredService<IJwtService>();

    #region 消息接收（统一入口）

    public async Task OnWebSocketReceived(IWebSocket webSocket, WSDataFrameEventArgs e)
    {
        var dataFrame = e.DataFrame;

        switch (dataFrame.Opcode)
        {
            case WSDataType.Text:
                await HandleTextMessage(webSocket, dataFrame.ToText());
                break;

            case WSDataType.Close:
                await HandleDisconnect(webSocket);
                break;

            case WSDataType.Binary:
                Logger.LogDebug("[WS] 收到二进制数据");
                break;
        }

        await e.InvokeNext();
    }

    #endregion

    #region 文本消息分发

    private async Task HandleTextMessage(IWebSocket webSocket, string text)
    {
        var clientId = GetClientId(webSocket);

        try
        {
            using var doc = JsonDocument.Parse(text);
            var root = doc.RootElement;
            var msgType = root.TryGetProperty("type", out var typeEl) ? typeEl.GetString() ?? "chat" : "chat";

            switch (msgType)
            {
                case "auth":
                    await HandleAuth(webSocket, text);
                    break;
                case "chat":
                    await HandleChatMessage(webSocket, text);
                    break;
                case "typing":
                    await HandleTyping(webSocket, text);
                    break;
                case "read":
                    await HandleReadReceipt(webSocket, text);
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[WS] 处理消息失败: {Message}", text);
        }
    }

    #endregion

    #region 断开连接处理

    private async Task HandleDisconnect(IWebSocket webSocket)
    {
        var clientId = GetClientId(webSocket);

        if (_authConnections.TryRemove(clientId, out var userId))
        {
            _onlineUsers.TryRemove(userId, out _);

            await UpdateUserOnlineStatus(userId, false);

            var offlineMsg = new WsMessage
            {
                Type = "offline",
                FromUserId = userId,
                Timestamp = DateTime.UtcNow
            };
            await BroadcastToAllExcept(offlineMsg, userId);

            Logger.LogInformation("[WS] 用户 {UserId} 已下线", userId);
        }

        Logger.LogInformation("[WS] 客户端断开: {ClientId}", clientId);
    }

    #endregion

    #region 认证处理

    private async Task HandleAuth(IWebSocket webSocket, string rawText)
    {
        try
        {
            var authReq = JsonSerializer.Deserialize<WsAuthRequest>(rawText);
            if (authReq?.Token == null) return;

            var principal = JwtService.ValidateToken(authReq.Token);
            if (principal == null)
            {
                await SendError(webSocket, "认证失败：无效的Token");
                return;
            }

            var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return;

            var user = await Db.Queryable<User>().FirstAsync(u => u.Id == userId);
            if (user == null)
            {
                await SendError(webSocket, "用户不存在");
                return;
            }

            var clientId = GetClientId(webSocket);

            // 踢掉旧连接（同账号互斥）
            if (_onlineUsers.TryGetValue(userId, out var oldSocket))
            {
                _authConnections.TryRemove(GetClientId(oldSocket), out _);
            }

            _onlineUsers[userId] = webSocket;
            _authConnections[clientId] = userId;

            await UpdateUserOnlineStatus(userId, true);

            // 发送认证成功 + 在线列表
            var authSuccess = new WsMessage
            {
                Type = "system",
                FromUserId = 0,
                FromUserName = "系统",
                Content = $"欢迎 {user.Nickname ?? user.UserName}，您已上线！",
                Timestamp = DateTime.UtcNow
            };
            await SendMessage(webSocket, authSuccess);

            var onlineList = await GetOnlineUsersInfo();
            var onlineMsg = new WsMessage
            {
                Type = "online_list",
                Content = JsonSerializer.Serialize(onlineList),
                Timestamp = DateTime.UtcNow
            };
            await SendMessage(webSocket, onlineMsg);

            // 广播上线通知给其他人
            var onlineNotify = new WsMessage
            {
                Type = "online",
                FromUserId = userId,
                FromUserName = user.Nickname ?? user.UserName,
                Content = $"{user.Nickname ?? user.UserName} 已上线",
                Timestamp = DateTime.UtcNow
            };
            await BroadcastToAllExcept(onlineNotify, userId);

            Logger.LogInformation("[WS] 用户 {UserName}(Id={UserId}) 认证成功", user.UserName, userId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[WS] 认证处理异常");
            await SendError(webSocket, "认证处理异常");
        }
    }

    #endregion

    #region 聊天消息处理

    private async Task HandleChatMessage(IWebSocket webSocket, string rawText)
    {
        var clientId = GetClientId(webSocket);
        if (!_authConnections.TryGetValue(clientId, out var fromUserId)) return;

        try
        {
            var msg = JsonSerializer.Deserialize<WsMessage>(rawText);
            if (msg == null || string.IsNullOrWhiteSpace(msg.Content)) return;

            msg.FromUserId = fromUserId;

            var sender = await Db.Queryable<User>().FirstAsync(u => u.Id == fromUserId);
            if (sender != null)
            {
                msg.FromUserName = sender.Nickname ?? sender.UserName;
            }
            msg.Timestamp = DateTime.UtcNow;

            // 保存到数据库
            if (msg.ChannelId.HasValue && msg.ChannelId.Value > 0)
            {
                var isMember = await Db.Queryable<ChannelMember>()
                    .AnyAsync(cm => cm.ChannelId == msg.ChannelId.Value && cm.UserId == fromUserId);
                if (isMember)
                {
                    var message = new Message
                    {
                        ChannelId = msg.ChannelId.Value,
                        UserId = fromUserId,
                        Content = msg.Content,
                        Type = MessageType.Text,
                        CreatedTime = DateTime.UtcNow
                    };
                    msg.MessageId = await Db.Insertable(message).ExecuteReturnIdentityAsync();

                    await Db.Updateable<Post>()
                        .SetColumns(p => p.UpdatedTime == DateTime.UtcNow)
                        .Where(p => p.Id == msg.ChannelId.Value)
                        .ExecuteCommandAsync();
                }
            }

            // 投递消息
            if (msg.ToUserId.HasValue && msg.ToUserId.Value > 0)
            {
                await SendToUser(msg, msg.ToUserId.Value);
                await SendToUser(msg, fromUserId);
            }
            else if (msg.ChannelId.HasValue)
            {
                await BroadcastToChannel(msg, msg.ChannelId.Value);
            }
            else
            {
                await BroadcastToAll(msg);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[WS] 聊天消息处理异常");
        }
    }

    #endregion

    #region 输入状态

    private async Task HandleTyping(IWebSocket webSocket, string rawText)
    {
        var clientId = GetClientId(webSocket);
        if (!_authConnections.TryGetValue(clientId, out var fromUserId)) return;

        try
        {
            var msg = JsonSerializer.Deserialize<WsMessage>(rawText);
            if (msg == null || !msg.ToUserId.HasValue) return;

            var sender = await Db.Queryable<User>().FirstAsync(u => u.Id == fromUserId);
            var typingMsg = new WsMessage
            {
                Type = "typing",
                FromUserId = fromUserId,
                FromUserName = sender?.Nickname ?? sender?.UserName ?? "",
                ToUserId = msg.ToUserId,
                Timestamp = DateTime.UtcNow
            };

            await SendToUser(typingMsg, msg.ToUserId.Value);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[WS] 输入状态异常");
        }
    }

    #endregion

    #region 已读回执

    private async Task HandleReadReceipt(IWebSocket webSocket, string rawText)
    {
        var clientId = GetClientId(webSocket);
        if (!_authConnections.TryGetValue(clientId, out var userId)) return;

        try
        {
            var msg = JsonSerializer.Deserialize<WsMessage>(rawText);
            if (msg?.ChannelId.HasValue != true) return;

            await Db.Updateable<ChannelMember>()
                .SetColumns(cm => cm.LastReadTime == DateTime.UtcNow)
                .Where(cm => cm.ChannelId == msg.ChannelId.Value && cm.UserId == userId)
                .ExecuteCommandAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[WS] 已读回执异常");
        }
    }

    #endregion

    #region 消息发送

    private async Task SendToUser(WsMessage msg, int toUserId)
    {
        if (_onlineUsers.TryGetValue(toUserId, out var socket))
            await SendMessage(socket, msg);
    }

    private async Task BroadcastToAll(WsMessage msg)
    {
        var json = JsonSerializer.Serialize(msg);
        foreach (var (_, socket) in _onlineUsers)
        {
            try { await socket.SendAsync(json); } catch { /* 忽略 */ }
        }
    }

    private async Task BroadcastToAllExcept(WsMessage msg, int exceptUid)
    {
        var json = JsonSerializer.Serialize(msg);
        foreach (var (uid, socket) in _onlineUsers)
        {
            if (uid == exceptUid) continue;
            try { await socket.SendAsync(json); } catch { /* 忽略 */ }
        }
    }

    private async Task BroadcastToChannel(WsMessage msg, int channelId)
    {
        var members = await Db.Queryable<ChannelMember>()
            .Where(cm => cm.ChannelId == channelId)
            .Select(cm => cm.UserId).ToListAsync();

        var json = JsonSerializer.Serialize(msg);
        foreach (var mid in members)
        {
            if (_onlineUsers.TryGetValue(mid, out var s))
            {
                try { await s.SendAsync(json); } catch { /* 忽略 */ }
            }
        }
    }

    private async Task SendMessage(IWebSocket socket, WsMessage msg)
    {
        try { await socket.SendAsync(JsonSerializer.Serialize(msg)); }
        catch (Exception ex) { Logger.LogWarning(ex, "[WS] 发送失败"); }
    }

    private async Task SendError(IWebSocket socket, string err)
    {
        await SendMessage(socket, new WsMessage
        {
            Type = "error",
            FromUserId = 0,
            FromUserName = "系统",
            Content = err,
            Timestamp = DateTime.UtcNow
        });
    }

    #endregion

    #region 数据库

    private async Task UpdateUserOnlineStatus(int uid, bool online)
    {
        try
        {
            await Db.Updateable<User>()
                .SetColumns(u => u.OnlineStatus == online)
                .Where(u => u.Id == uid)
                .ExecuteCommandAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[WS] 更新在线状态失败 UserId={Uid}", uid);
        }
    }

    private async Task<List<WsOnlineUser>> GetOnlineUsersInfo()
    {
        var list = new List<WsOnlineUser>();
        foreach (var uid in _onlineUsers.Keys)
        {
            var u = await Db.Queryable<User>().FirstAsync(x => x.Id == uid);
            if (u != null)
            {
                list.Add(new WsOnlineUser
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Nickname = u.Nickname,
                    Avatar = u.Avatar,
                    OnlineTime = DateTime.UtcNow
                });
            }
        }
        return list;
    }

    #endregion

    #region 工具方法

    private static string GetClientId(IWebSocket s) => s.GetHashCode().ToString("X8");

    public static int GetOnlineCount() => _onlineUsers.Count;
    public static bool IsUserOnline(int uid) => _onlineUsers.ContainsKey(uid);

    #endregion
}
