using Avalonia;
using Avalonia.Collections;
using Avalonia.Threading;
using Chat.Desktop.Models;
using Chat.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 聊天室ViewModel
/// </summary>
public partial class ChatViewModel : ViewModelBase
{
    private readonly ChatWebSocketService _wsService;

    [ObservableProperty]
    private string? _sendContent = string.Empty;

    [ObservableProperty]
    private bool _isConnected = false;

    [ObservableProperty]
    private string? _statusText = "未连接";

    /// <summary>
    /// 输入框占位符（根据连接状态变化）
    /// </summary>
    public string InputPlaceholder => IsConnected ? "Type something..." : "Connecting to server...";

    [ObservableProperty]
    private string? _currentUserName = "未登录";

    [ObservableProperty]
    private int? _currentUserId;

    /// <summary>
    /// 聊天消息列表
    /// </summary>
    public AvaloniaList<ChatModel> ChatLists { get; } = [];

    /// <summary>
    /// 在线用户列表（强类型）
    /// </summary>
    public AvaloniaList<OnlineUserModel> OnlineUsers { get; } = [];

    /// <summary>
    /// 当前选中的在线用户（用于私聊）
    /// </summary>
    [ObservableProperty]
    private OnlineUserModel? _selectedUser;

    /// <summary>
    /// 是否处于私聊模式
    /// </summary>
    [ObservableProperty]
    private bool _isPrivateChat = false;

    /// <summary>
    /// 退出登录事件
    /// </summary>
    public event Action? OnLogoutRequested;

    public ChatViewModel()
    {
        _wsService = new ChatWebSocketService();

        // 订阅连接状态变化
        _wsService.OnConnectionChanged += (connected) =>
        {
            IsConnected = connected;
            StatusText = connected ? "已连接服务器" : "已断开连接";
            OnPropertyChanged(nameof(InputPlaceholder));
        };

        // 订阅消息接收
        _wsService.OnMessageReceived += (chat) =>
        {
            ChatLists.Add(chat);
        };

        // 订阅用户上线/下线
        _wsService.OnUserOnlineChanged += (name, isOnline) =>
        {
            if (isOnline)
            {
                // 上线：添加到列表
                var user = new OnlineUserModel { NickName = name, OnlineTime = DateTime.Now };
                OnlineUsers.Add(user);
            }
            else
            {
                // 下线：从列表移除
                for (var i = OnlineUsers.Count - 1; i >= 0; i--)
                {
                    if (OnlineUsers[i]?.NickName == name)
                    {
                        OnlineUsers.RemoveAt(i);
                        break;
                    }
                }
            }
        };

        // 订阅在线列表更新
        _wsService.OnOnlineListUpdated += (users) =>
        {
            OnlineUsers.Clear();
            foreach (var u in users)
            {
                OnlineUsers.Add(new OnlineUserModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    NickName = u.NickName,
                    Avatar = u.Avatar,
                    OnlineTime = u.OnlineTime
                });
            }
        };

        // 订阅系统消息
        _wsService.OnSystemMessage += (msg) =>
        {
            var sysMsg = new ChatModel
            {
                NickName = "系统",
                Content = msg,
                SendTime = DateTime.Now,
                MessageType = "system",
                TextAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                TextDock = Avalonia.Controls.Dock.Top
            };
            ChatLists.Add(sysMsg);
        };
    }

    /// <summary>
    /// 连接到WebSocket服务器（登录成功后调用）
    /// </summary>
    public async Task ConnectAsync(string token, int userId, string userName)
    {
        System.Diagnostics.Debug.WriteLine($"[ChatVM] ConnectAsync 被调用: userId={userId}, userName={userName}, token长度={token?.Length ?? 0}");
        CurrentUserId = userId;
        CurrentUserName = userName;
        StatusText = "正在连接...";

        try
        {
            await _wsService.ConnectAsync(token, userId);
            System.Diagnostics.Debug.WriteLine($"[ChatVM] ConnectAsync 完成, IsConnected={_wsService.IsConnected}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ChatVM] ConnectAsync 异常: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
                System.Diagnostics.Debug.WriteLine($"[ChatVM] 内部异常: {ex.InnerException.Message}");

            // 显示具体错误原因
            ChatLists.Add(new ChatModel
            {
                NickName = "系统",
                Content = $"WS连接失败: {ex.Message}\n({ex.GetType().Name})",
                SendTime = DateTime.Now,
                MessageType = "system",
                TextAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                TextDock = Avalonia.Controls.Dock.Top
            });
            return;  // 不再显示下面的通用失败消息
        }

        // 如果连接失败（无异常但状态为false）
        if (!_wsService.IsConnected)
        {
            ChatLists.Add(new ChatModel
            {
                NickName = "系统",
                Content = "WebSocket 连接被拒绝 (ws://localhost:5003)\n请确认后端服务已启动",
                SendTime = DateTime.Now,
                MessageType = "system",
                TextAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                TextDock = Avalonia.Controls.Dock.Top
            });
        }
    }

    /// <summary>
    /// 断开WebSocket连接
    /// </summary>
    public async Task DisconnectAsync()
    {
        await _wsService.DisconnectAsync();
        OnlineUsers.Clear();
        IsPrivateChat = false;
        SelectedUser = null;
    }

    /// <summary>
    /// 设置Token（登录成功后调用）
    /// </summary>
    public void SetToken(string token)
    {
        // Token 由 MainViewModel 统一管理
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    [RelayCommand]
    private void SendClick()
    {
        if (string.IsNullOrWhiteSpace(SendContent)) return;

        var content = SendContent.Trim();
        if (string.IsNullOrEmpty(content)) return;

        // 未连接时显示提示
        if (!_wsService.IsConnected)
        {
            ChatLists.Add(new ChatModel
            {
                NickName = "系统",
                Content = "未连接到服务器，消息无法发送",
                SendTime = DateTime.Now,
                MessageType = "system",
                TextAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                TextDock = Avalonia.Controls.Dock.Top
            });
            return;
        }

        // 先在本地显示自己的消息（右侧）- 同步操作，立即生效
        ChatLists.Add(new ChatModel
        {
            NickName = CurrentUserName ?? "我",
            FromUserId = CurrentUserId ?? 0,
            Content = content,
            SendTime = DateTime.Now,
            MessageType = "chat",
            TextAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            TextDock = Avalonia.Controls.Dock.Right
        });

        // 清空输入框
        SendContent = string.Empty;

        // 异步发送到服务器（不阻塞UI）
        _ = SendToServerAsync(content);
    }

    /// <summary>
    /// 异步发送消息到服务器
    /// </summary>
    private async Task SendToServerAsync(string content)
    {
        try
        {
            int? toUserId = null;
            int? channelId = null;

            // 私聊模式：发送给选中的用户
            if (IsPrivateChat && SelectedUser != null)
            {
                toUserId = SelectedUser.UserId;
            }

            await _wsService.SendChatAsync(content, toUserId, channelId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Chat] 发送失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 选择在线用户（切换到私聊）
    /// </summary>
    [RelayCommand]
    private void SelectUser(OnlineUserModel? user)
    {
        if (user == null || user.UserId == CurrentUserId) return;

        SelectedUser = user;
        IsPrivateChat = true;
        StatusText = $"与 {user.DisplayName} 私聊中";
    }

    /// <summary>
    /// 切换回群聊模式
    /// </summary>
    [RelayCommand]
    private void SwitchToGroupChat()
    {
        IsPrivateChat = false;
        SelectedUser = null;
        StatusText = IsConnected ? "群聊模式" : "未连接";
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [RelayCommand]
    private async Task LogoutAsync()
    {
        await DisconnectAsync();
        ChatLists.Clear();
        OnLogoutRequested?.Invoke();
    }

    public void Dispose()
    {
        _wsService.Dispose();
    }
}
