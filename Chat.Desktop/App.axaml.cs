using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Chat.Application.Services;
using Chat.Core;
using Chat.Desktop.ViewModels;
using Chat.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

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
                services.AddSingleton<ChatViewModel>();

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
                        await ShowMainWindowAsync(desktop, loginVm);
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
        private static async Task ShowMainWindowAsync(IClassicDesktopStyleApplicationLifetime desktop, LoginWindowViewModel loginVm)
        {
            var chatVm = Furion.App.GetRequiredService<ChatViewModel>();

            // 连接WebSocket（使用登录获取的Token和UserId）
            if (!string.IsNullOrEmpty(loginVm.AccessToken) && loginVm.CurrentUserId.HasValue)
            {
                await chatVm.ConnectAsync(loginVm.AccessToken, loginVm.CurrentUserId.Value);
            }

            var mainView = new MainView { DataContext = chatVm };
            desktop.MainWindow = mainView;
        }
    }
}
