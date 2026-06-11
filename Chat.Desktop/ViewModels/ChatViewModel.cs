using Avalonia.Collections;
using Chat.Desktop.Models;
using Chat.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;

namespace Chat.Desktop.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    private readonly ChatWebSocketService _wsService;

    [ObservableProperty]
    private string? _sendContent = string.Empty;

    [ObservableProperty]
    private bool _isConnected = false;

    [ObservableProperty]
    private string? _statusText = "未连接";

    public AvaloniaList<ChatModel> ChatLists { get; } = [];

    /// <summary>
    /// 在线用户列表
    /// </summary>
    public AvaloniaList<string> OnlineUsers { get; } = [];

    public ChatViewModel()
    {
        _wsService = new ChatWebSocketService();

        // 订阅事件
        _wsService.OnConnectionChanged += (connected) =>
        {
            IsConnected = connected;
            StatusText = connected ? "已连接" : "已断开";
        };

        _wsService.OnMessageReceived += (chat) =>
        {
            // 在UI线程添加消息
            ChatLists.Add(chat);
        };

        _wsService.OnUserOnlineChanged += (name, online) =>
        {
            if (online)
                OnlineUsers.Add(name);
            else
                OnlineUsers.Remove(name);
        };

        _wsService.OnOnlineListUpdated += (users) =>
        {
            OnlineUsers.Clear();
            foreach (var user in users)
                OnlineUsers.Add(user);
        };

        _wsService.OnSystemMessage += (msg) =>
        {
            var sysMsg = new ChatModel
            {
                NickName = "系统",
                Content = msg,
                SendTime = DateTime.Now,
                TextAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                TextDock = Avalonia.Controls.Dock.Top
            };
            ChatLists.Add(sysMsg);
        };
    }

    /// <summary>
    /// 连接到WebSocket服务器（登录成功后调用）
    /// </summary>
    public async Task ConnectAsync(string token, int userId)
    {
        await _wsService.ConnectAsync(token, userId);
    }

    /// <summary>
    /// 断开WebSocket连接
    /// </summary>
    public async Task DisconnectAsync()
    {
        await _wsService.DisconnectAsync();
    }

    [RelayCommand]
    private async Task SendClick()
    {
        if (!_wsService.IsConnected || string.IsNullOrWhiteSpace(SendContent)) return;

        // 先显示在聊天列表（右侧，自己的消息）
        var chat = new ChatModel
        {
            NickName = "我",
            Content = SendContent,
            SendTime = DateTime.Now,
            TextAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            TextDock = Avalonia.Controls.Dock.Right
        };
        ChatLists.Add(chat);

        try
        {
            // 通过WebSocket发送消息
            await _wsService.SendChatAsync(SendContent);
            SendContent = string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Chat] 发送失败: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _wsService.Dispose();
    }
}
