using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Chat.Desktop;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
        // DataContext 由 MainView 或 App 设置
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // ViewModel 已由外部设置，无需在此初始化
        Console.WriteLine("[ChatView] 聊天界面已加载");
    }
}
