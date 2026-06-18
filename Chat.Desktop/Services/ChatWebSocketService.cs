using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Chat.Desktop.Models;

namespace Chat.Desktop.Services
{

/// <summary>
/// 客户端WebSocket服务
/// 负责连接管理、认证、消息收发、断线重连
/// </summary>
public class ChatWebSocketService : IDisposable
{
    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _cts;
    private string? _accessToken;
    private int _currentUserId;
    private readonly Uri _serverUri;
    private bool _isDisposed;
    private Task? _receiveTask;

    /// <summary>
    /// 连接状态变化事件
    /// </summary>
    public event Action<bool>? OnConnectionChanged;

    /// <summary>
    /// 收到聊天消息事件
    /// </summary>
    public event Action<ChatModel>? OnMessageReceived;

    /// <summary>
    /// 用户上线/下线事件
    /// </summary>
    public event Action<string, bool>? OnUserOnlineChanged;

    /// <summary>
    /// 在线用户列表更新事件（强类型）
    /// </summary>
    public event Action<List<OnlineUserModel>>? OnOnlineListUpdated;

    /// <summary>
    /// 系统消息事件
    /// </summary>
    public event Action<string>? OnSystemMessage;

    /// <summary>
    /// 输入状态通知事件（对方正在输入）
    /// </summary>
    public event Action<int, string>? OnTypingReceived;

    /// <summary>
    /// 是否已连接并认证
    /// </summary>
    public bool IsConnected => _webSocket?.State == WebSocketState.Open;

    public ChatWebSocketService(string? serverUrl = null)
    {
        _serverUri = new Uri(serverUrl ?? AppConfig.WsUrl);
    }

    /// <summary>
    /// 连接WebSocket并进行JWT认证
    /// </summary>
    public async Task ConnectAsync(string token, int userId)
    {
        if (IsConnected)
        {
            System.Diagnostics.Debug.WriteLine("[WS] 已连接，跳过");
            return;
        }

        try
        {
            _accessToken = token;
            _currentUserId = userId;
            _cts = new CancellationTokenSource();

            System.Diagnostics.Debug.WriteLine($"[WS] 正在连接 {_serverUri}...");

            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(_serverUri, _cts.Token);

            System.Diagnostics.Debug.WriteLine("[WS] TCP连接成功，发送认证消息...");

            // 发送认证消息
            var authMsg = new { type = "auth", token = _accessToken };
            var authJson = JsonSerializer.Serialize(authMsg);
            System.Diagnostics.Debug.WriteLine($"[WS] 认证消息: Token长度={_accessToken?.Length ?? 0}");

            await SendRawAsync(authJson);

            System.Diagnostics.Debug.WriteLine("[WS] 认证消息已发送，启动接收循环...");

            // 启动接收循环
            _receiveTask = ReceiveLoopAsync(_cts.Token);

            OnConnectionChanged?.Invoke(true);
            System.Diagnostics.Debug.WriteLine("[WS] 连接完成！");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WS] 连接失败: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
                System.Diagnostics.Debug.WriteLine($"[WS] 内部异常: {ex.InnerException.Message}");
            OnConnectionChanged?.Invoke(false);
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public async Task DisconnectAsync()
    {
        try
        {
            _cts?.Cancel();
            if (_webSocket?.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "客户端主动断开", CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WS客户端] 断开异常: {ex.Message}");
        }
        finally
        {
            _webSocket?.Dispose();
            _webSocket = null;
            OnConnectionChanged?.Invoke(false);
        }
    }

    /// <summary>
    /// 发送聊天消息
    /// </summary>
    public async Task SendChatAsync(string content, int? toUserId = null, int? channelId = null)
    {
        System.Diagnostics.Debug.WriteLine($"[WS] SendChatAsync: content={content}, connected={IsConnected}, state={_webSocket?.State}");

        if (!IsConnected)
        {
            System.Diagnostics.Debug.WriteLine("[WS] SendChatAsync: 未连接，跳过");
            return;
        }

        var msg = new
        {
            type = "chat",
            content,
            toUserId,
            channelId
        };
        var json = JsonSerializer.Serialize(msg);
        System.Diagnostics.Debug.WriteLine($"[WS] SendChatAsync: 发送 {json}");
        await SendRawAsync(json);
    }

    /// <summary>
    /// 发送输入状态（正在输入）
    /// </summary>
    public async Task SendTypingAsync(int toUserId)
    {
        if (!IsConnected) return;

        var msg = new { type = "typing", toUserId };
        await SendRawAsync(JsonSerializer.Serialize(msg));
    }

    /// <summary>
    /// 发送已读回执
    /// </summary>
    public async Task SendReadReceiptAsync(int channelId)
    {
        if (!IsConnected) return;

        var msg = new { type = "read", channelId };
        await SendRawAsync(JsonSerializer.Serialize(msg));
    }

    /// <summary>
    /// 消息接收循环
    /// </summary>
    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];
        var segment = new ArraySegment<byte>(buffer);

        while (!cancellationToken.IsCancellationRequested && IsConnected)
        {
            try
            {
                var result = await _webSocket!.ReceiveAsync(segment, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("[WS客户端] 服务器关闭连接");
                    break;
                }

                var messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                HandleReceivedMessage(messageText);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WS客户端] 接收消息异常: {ex.Message}");
                break;
            }
        }

        OnConnectionChanged?.Invoke(false);
    }

    /// <summary>
    /// 处理接收到的消息
    /// </summary>
    private void HandleReceivedMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var type = root.TryGetProperty("type", out var typeProp)
                ? typeProp.GetString() ?? ""
                : "";

            switch (type)
            {
                case "chat":
                    HandleChatMessage(root);
                    break;

                case "online":
                    HandleUserOnline(root, isOnline: true);
                    break;

                case "offline":
                    HandleUserOnline(root, isOnline: false);
                    break;

                case "online_list":
                    HandleOnlineList(root);
                    break;

                case "system":
                    HandleSystemMessage(root);
                    break;

                case "typing":
                    HandleTypingNotification(root);
                    break;

                case "error":
                    HandleErrorMessage(root);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WS客户端] 消息解析异常: {ex.Message} | 原始数据: {json}");
        }
    }

    #region 消息处理

    private void HandleChatMessage(JsonElement root)
    {
        var fromUserName = root.TryGetProperty("fromUserName", out var nameEl) ? nameEl.GetString() ?? "" : "";
        var content = root.TryGetProperty("content", out var contentEl) ? contentEl.GetString() ?? "" : "";
        var fromUserId = root.TryGetProperty("fromUserId", out var uidEl) ? uidEl.GetInt32() : 0;

        // 跳过自己的消息（SendClick 已在本地显示）
        if (fromUserId == _currentUserId)
            return;

        var chatModel = new ChatModel
        {
            NickName = fromUserName,
            Content = content,
            SendTime = DateTime.Now,
            TextAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            TextDock = Avalonia.Controls.Dock.Left
        };

        OnMessageReceived?.Invoke(chatModel);
    }

    private void HandleUserOnline(JsonElement root, bool isOnline)
    {
        var userName = root.TryGetProperty("fromUserName", out var nameEl) ? nameEl.GetString() ?? "" : "";
        OnUserOnlineChanged?.Invoke(userName, isOnline);
    }

    private void HandleOnlineList(JsonElement root)
    {
        var users = new List<OnlineUserModel>();
        if (root.TryGetProperty("content", out var contentEl))
        {
            try
            {
                using var userDoc = JsonDocument.Parse(contentEl.GetString() ?? "[]");
                foreach (var user in userDoc.RootElement.EnumerateArray())
                {
                    var userId = user.TryGetProperty("userId", out var uid) ? uid.GetInt32() : 0;
                    var userName = user.TryGetProperty("userName", out var un) ? un.GetString() : "";
                    var nickname = user.TryGetProperty("nickname", out var nn) ? nn.GetString() : "";
                    var avatar = user.TryGetProperty("avatar", out var av) ? av.GetString() : null;

                    users.Add(new OnlineUserModel
                    {
                        UserId = userId,
                        UserName = userName,
                        NickName = nickname,
                        Avatar = avatar,
                        OnlineTime = DateTime.Now
                    });
                }
            }
            catch { /* 解析失败忽略 */ }
        }
        OnOnlineListUpdated?.Invoke(users);
    }

    private void HandleSystemMessage(JsonElement root)
    {
        var content = root.TryGetProperty("content", out var c) ? c.GetString() ?? "" : "";
        OnSystemMessage?.Invoke(content);
    }

    private void HandleTypingNotification(JsonElement root)
    {
        var fromUserId = root.TryGetProperty("fromUserId", out var uidEl) ? uidEl.GetInt32() : 0;
        var fromUserName = root.TryGetProperty("fromUserName", out var nameEl) ? nameEl.GetString() ?? "" : "";
        OnTypingReceived?.Invoke(fromUserId, fromUserName);
    }

    private void HandleErrorMessage(JsonElement root)
    {
        var content = root.TryGetProperty("content", out var c) ? c.GetString() ?? "" : "";
        Console.WriteLine($"[WS客户端] 服务端错误: {content}");
        OnSystemMessage?.Invoke($"错误: {content}");
    }

    #endregion

    /// <summary>
    /// 发送原始JSON字符串
    /// </summary>
    private async Task SendRawAsync(string json)
    {
        if (!IsConnected || _webSocket == null)
        {
            System.Diagnostics.Debug.WriteLine($"[WS] SendRawAsync: 未连接或socket为空, connected={IsConnected}, socket={(_webSocket != null ? _webSocket.State.ToString() : "null")}");
            return;
        }

        try
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(bytes);
            await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            System.Diagnostics.Debug.WriteLine("[WS] SendRawAsync: 发送成功");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WS] SendRawAsync 异常: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        DisconnectAsync().GetAwaiter().GetResult();
        _cts?.Dispose();
    }
}
}
