using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 主界面ViewModel
/// 管理三个页面导航：我的 / 聊天室 / 论坛
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    /// <summary>
    /// 搜索视图ViewModel
    /// </summary>
    public SearchViewModel SearchVM { get; }

    /// <summary>
    /// 通知视图ViewModel
    /// </summary>
    public NotificationViewModel NotificationVM { get; }

    /// <summary>
    /// 聊天室ViewModel
    /// </summary>
    public ChatViewModel ChatVM { get; }

    /// <summary>
    /// 论坛ViewModel
    /// </summary>
    public ForumViewModel ForumVM { get; }

    /// <summary>
    /// 个人中心ViewModel
    /// </summary>
    public ProfileViewModel ProfileVM { get; }

    /// <summary>
    /// 退出登录事件
    /// </summary>
    public event Action? OnLogoutRequested;

    /// <summary>
    /// 当前用户名
    /// </summary>
    [ObservableProperty]
    private string _currentUserName = "用户";

    public MainViewModel()
    {
        SearchVM = new SearchViewModel();
        NotificationVM = new NotificationViewModel();
        ChatVM = new ChatViewModel();
        ForumVM = new ForumViewModel();
        ProfileVM = new ProfileViewModel();

        // 订阅聊天室的退出登录事件
        ChatVM.OnLogoutRequested += () => RequestLogout();

        // 默认显示搜索
        SelectedTabIndex = 0;
    }

    /// <summary>
    /// 当前选中的Tab索引 (0=搜索, 1=通知, 2=我的, 3=聊天室, 4=论坛)
    /// </summary>
    [ObservableProperty]
    private int _selectedTabIndex = 0;

    partial void OnSelectedTabIndexChanged(int value)
    {
        switch (value)
        {
            case 0:
                // 搜索页面 - 不需要额外加载
                break;
            case 1:
                // 通知页面 - 加载通知
                _ = NotificationVM.LoadNotificationsAsync();
                break;
            case 2:
                if (!ProfileVM.HasProfile)
                    _ = ProfileVM.LoadProfileAsync();
                break;
            case 3:
                break;
            case 4:
                _ = ForumVM.LoadPostsAsync();
                break;
        }
    }

    /// <summary>
    /// 设置认证Token（登录成功后调用）
    /// </summary>
    public void SetToken(string token, int userId, string userName)
    {
        SearchVM.SetToken(token);
        NotificationVM.SetToken(token);
        ChatVM.SetToken(token);
        ForumVM.SetToken(token);
        ProfileVM.SetToken(token);
        CurrentUserName = userName;

        // 加载通知
        _ = NotificationVM.LoadNotificationsAsync();
    }

    /// <summary>
    /// 请求退出登录
    /// </summary>
    public void RequestLogout()
    {
        OnLogoutRequested?.Invoke();
    }

    /// <summary>
    /// 连接WebSocket（登录后调用）
    /// </summary>
    public async Task ConnectWebSocketAsync(string token, int userId, string userName)
    {
        await ChatVM.ConnectAsync(token, userId, userName);
    }

    // ===== 导航命令 =====

    [RelayCommand]
    private void SwitchToSearch() => SelectedTabIndex = 0;

    [RelayCommand]
    private void SwitchToNotification() => SelectedTabIndex = 1;

    [RelayCommand]
    private void SwitchToProfile() => SelectedTabIndex = 2;

    [RelayCommand]
    private void SwitchToChat() => SelectedTabIndex = 3;

    [RelayCommand]
    private void SwitchToForum() => SelectedTabIndex = 4;

    [RelayCommand]
    private void ToggleTheme()
    {
        // 使用 Avalonia 原生 API 切换主题
        var app = Avalonia.Application.Current;
        if (app != null)
        {
            var current = app.RequestedThemeVariant;
            app.RequestedThemeVariant = current == Avalonia.Styling.ThemeVariant.Dark
                ? Avalonia.Styling.ThemeVariant.Light
                : Avalonia.Styling.ThemeVariant.Dark;
            System.Diagnostics.Debug.WriteLine($"[MainVM] 主题切换为: {app.RequestedThemeVariant}");
        }
    }

    [RelayCommand]
    private void DoLogout()
    {
        ChatVM.DisconnectAsync().GetAwaiter().GetResult();
        OnLogoutRequested?.Invoke();
    }
}
