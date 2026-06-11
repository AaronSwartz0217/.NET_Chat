using Avalonia.Controls;
using Avalonia.Interactivity;
using Chat.Desktop.ViewModels;
using Chat.Desktop.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Http.WebSockets;

namespace Chat.Desktop;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
        this.DataContext = new ChatViewModel();
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not ChatViewModel vm)
        {
            return;
        }

        // 初始化WebSocket客户端连接
        try
        {
            vm.Client = new WebSocketClient();

            // 配置WebSocket客户端
            await vm.Client.SetupAsync(new TouchSocketConfig()
                .ConfigurePlugins(a =>
                {
                    // 添加消息接收插件（传入当前ViewModel实例）
                    a.Add(new ClientWebSocketReceivePlugin(vm));
                }));

            // 连接到服务器（使用超时时间5000毫秒，传入CancellationToken.None）
            await vm.Client.ConnectAsync(5000, CancellationToken.None);
            Console.WriteLine("[客户端] WebSocket连接成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[客户端] WebSocket连接失败: {ex.Message}");
        }
    }
}

/// <summary>
/// 客户端WebSocket消息接收插件
/// </summary>
public class ClientWebSocketReceivePlugin : PluginBase, IWebSocketReceivedPlugin
{
    private readonly ChatViewModel _viewModel;

    public ClientWebSocketReceivePlugin(ChatViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public async Task OnWebSocketReceived(IWebSocket webSocket, WSDataFrameEventArgs e)
    {
        var dataFrame = e.DataFrame;

        switch (dataFrame.Opcode)
        {
            case WSDataType.Text:
                // 处理文本消息
                string text = dataFrame.ToText();
                Console.WriteLine($"[客户端] 收到消息: {text}");

                // 将收到的消息添加到聊天列表（显示为左侧，表示他人消息）
                _viewModel.ChatLists.Add(new ChatModel()
                {
                    NickName = "其他用户",
                    Content = text,
                    SendTime = DateTime.Now,
                    TextAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                    TextDock = Avalonia.Controls.Dock.Left
                });

                break;

            case WSDataType.Binary:
                Console.WriteLine($"[客户端] 收到二进制数据");
                break;

            case WSDataType.Close:
                Console.WriteLine("[客户端] 服务器断开连接");
                break;

            case WSDataType.Ping:
            case WSDataType.Pong:
                // 心跳包，自动处理
                break;

            default:
                break;
        }

        await e.InvokeNext();
    }
}
