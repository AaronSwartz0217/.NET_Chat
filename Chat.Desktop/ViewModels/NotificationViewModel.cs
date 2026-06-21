using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chat.Desktop.Services;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 通知视图ViewModel
/// </summary>
public partial class NotificationViewModel : ViewModelBase
{
    private readonly NotificationApiService _notificationService;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _hasNotifications = false;

    [ObservableProperty]
    private int _unreadCount = 0;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ObservableCollection<NotificationModel> Notifications { get; } = new();

    private string? _accessToken;

    public NotificationViewModel()
    {
        _notificationService = new NotificationApiService();
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    public void SetToken(string token)
    {
        _accessToken = token;
        _notificationService.SetToken(token);
    }

    /// <summary>
    /// 加载通知列表
    /// </summary>
    [RelayCommand]
    public async Task LoadNotificationsAsync()
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            StatusMessage = "未登录";
            return;
        }

        IsLoading = true;
        Notifications.Clear();

        try
        {
            var notifications = await _notificationService.GetNotificationsAsync();

            if (notifications != null && notifications.Count > 0)
            {
                foreach (var notification in notifications)
                {
                    Notifications.Add(notification);
                }
                HasNotifications = true;
                UnreadCount = notifications.Count(n => !n.IsRead);
                StatusMessage = UnreadCount > 0 ? $"有 {UnreadCount} 条未读通知" : "暂无未读通知";
            }
            else
            {
                HasNotifications = false;
                StatusMessage = "暂无通知";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败：{ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[Notification] 加载异常: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 标记单条已读
    /// </summary>
    [RelayCommand]
    private async Task MarkAsReadAsync(NotificationModel notification)
    {
        if (notification.IsRead)
            return;

        try
        {
            var success = await _notificationService.MarkAsReadAsync(notification.Id);
            if (success)
            {
                notification.IsRead = true;
                UnreadCount = Math.Max(0, UnreadCount - 1);
                UpdateStatusMessage();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Notification] 标记已读失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 全部标记已读
    /// </summary>
    [RelayCommand]
    private async Task MarkAllAsReadAsync()
    {
        try
        {
            var success = await _notificationService.MarkAllAsReadAsync();
            if (success)
            {
                foreach (var notification in Notifications)
                {
                    notification.IsRead = true;
                }
                UnreadCount = 0;
                StatusMessage = "已全部标记为已读";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Notification] 全部标记已读失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新状态消息
    /// </summary>
    private void UpdateStatusMessage()
    {
        if (UnreadCount > 0)
            StatusMessage = $"有 {UnreadCount} 条未读通知";
        else if (Notifications.Count > 0)
            StatusMessage = "暂无未读通知";
        else
            StatusMessage = "暂无通知";
    }

    /// <summary>
    /// 刷新通知
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadNotificationsAsync();
    }
}
