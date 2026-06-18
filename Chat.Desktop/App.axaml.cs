using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Chat.Application.Services;
using Chat.Core;
using Chat.Desktop.ViewModels;
using Chat.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Chat.Desktop
{
    public partial class App : Avalonia.Application
    {
        public App()
        {
            Serve.RunNative(services =>
            {
                services.AddHttpRemote();

                services.AddMySqlSetup();
                services.AddSingleton<IStudentService, StudentService>();
                services.AddSingleton<DataListViewModel>();
                services.AddSingleton<LoginWindowViewModel>();
                services.AddSingleton<MainViewModel>();

            }, includeWeb: false);
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 启动时显示登录窗口
                var loginVm = Furion.App.GetRequiredService<LoginWindowViewModel>();
                var loginWindow = new LoginWindow { DataContext = loginVm };

                // 监听登录结果，成功后切换到聊天窗口
                loginVm.RequestClose += async (sender, result) =>
                {
                    if (result is bool success && success)
                    {
                        await ShowMainWindowAsync(desktop, loginVm, loginWindow);
                    }
                    else
                    {
                        desktop.Shutdown();
                    }
                };

                desktop.MainWindow = loginWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
    /// 登录成功后显示主界面并连接WebSocket
    /// </summary>
    private static async Task ShowMainWindowAsync(IClassicDesktopStyleApplicationLifetime desktop, LoginWindowViewModel loginVm, LoginWindow? loginWindow)
    {
        System.Diagnostics.Debug.WriteLine($"[App] ShowMainWindowAsync: token={loginVm.AccessToken?.Substring(0, Math.Min(20, loginVm.AccessToken?.Length ?? 0))}..., userId={loginVm.CurrentUserId}");

        var mainVm = Furion.App.GetRequiredService<MainViewModel>();

        // 订阅退出登录事件，返回登录界面
        mainVm.OnLogoutRequested += () =>
        {
            ShowLoginWindow(desktop);
        };

        // 设置Token到所有子ViewModel
        if (!string.IsNullOrEmpty(loginVm.AccessToken) && loginVm.CurrentUserId.HasValue)
        {
            var userName = loginVm.NickName ?? loginVm.UserName ?? "用户";
            mainVm.SetToken(loginVm.AccessToken, loginVm.CurrentUserId.Value, userName);
        }

        // 先创建并显示主窗口（不等待WS连接）
        var mainView = new MainWindow { DataContext = mainVm };
        desktop.MainWindow = mainView;
        mainView.Show();
        System.Diagnostics.Debug.WriteLine($"[App] 主窗口已显示");

        // 再关闭登录窗口
        loginWindow?.Close();

        // 异步连接WebSocket（不阻塞UI）
        if (!string.IsNullOrEmpty(loginVm.AccessToken) && loginVm.CurrentUserId.HasValue)
        {
            var userName = loginVm.NickName ?? loginVm.UserName ?? "用户";
            var token = loginVm.AccessToken;
            var userId = loginVm.CurrentUserId.Value;
            System.Diagnostics.Debug.WriteLine($"[App] 准备连接WS: userId={userId}, userName={userName}");

            _ = ConnectWebSocketAsync(mainVm.ChatVM, token, userId, userName);
        }
    }

    /// <summary>
    /// 异步连接WebSocket（确保在UI线程更新状态）
    /// </summary>
    private static async Task ConnectWebSocketAsync(ChatViewModel chatVm, string token, int userId, string userName)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[App] WS连接开始: userId={userId}");
            await chatVm.ConnectAsync(token, userId, userName);

            // 在UI线程更新状态
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                System.Diagnostics.Debug.WriteLine($"[App] WS连接完成, IsConnected={chatVm.IsConnected}");
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App] WS连接异常: {ex.Message}");
            // 在UI线程显示错误
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                chatVm.ChatLists.Add(new Chat.Desktop.Models.ChatModel
                {
                    NickName = "系统",
                    Content = $"WS连接失败: {ex.Message}",
                    SendTime = DateTime.Now,
                    MessageType = "system",
                    TextAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    TextDock = Avalonia.Controls.Dock.Top
                });
            });
        }
    }

    /// <summary>
    /// 显示登录窗口
    /// </summary>
    private static void ShowLoginWindow(IClassicDesktopStyleApplicationLifetime desktop)
    {
        // 创建新的登录ViewModel（不再依赖IHttpRemoteService）
        var loginVm = new LoginWindowViewModel();
        var loginWindow = new LoginWindow { DataContext = loginVm };

        loginVm.RequestClose += async (sender, result) =>
        {
            if (result is bool success && success)
            {
                await ShowMainWindowAsync(desktop, loginVm, loginWindow);
            }
            else
            {
                desktop.Shutdown();
            }
        };

        desktop.MainWindow = loginWindow;
    }
    }
}
