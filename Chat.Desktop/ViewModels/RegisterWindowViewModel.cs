using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Chat.Desktop.Services;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 注册窗口ViewModel
/// </summary>
public partial class RegisterWindowViewModel : ViewModelBase, IDialogContext
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _nickName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string? _email = null;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess = false;

    /// <summary>
    /// 注册成功后的登录信息
    /// </summary>
    public LoginResponse? LoginData { get; private set; }

    public RegisterWindowViewModel()
    {
    }

    [RelayCommand]
    private async Task OkClick()
    {
        // 验证输入
        if (string.IsNullOrWhiteSpace(UserName))
        {
            StatusMessage = "请输入用户名";
            return;
        }

        if (UserName.Length < 3)
        {
            StatusMessage = "用户名至少3个字符";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "请输入密码";
            return;
        }

        if (Password.Length < 6)
        {
            StatusMessage = "密码至少6个字符";
            return;
        }

        if (Password != ConfirmPassword)
        {
            StatusMessage = "两次输入的密码不一致";
            return;
        }

        IsLoading = true;
        StatusMessage = "正在注册...";

        try
        {
            var registerService = new RegisterApiService();
            var (success, message, data) = await registerService.RegisterAsync(
                UserName.Trim(),
                Password,
                string.IsNullOrWhiteSpace(NickName) ? null : NickName.Trim(),
                string.IsNullOrWhiteSpace(Email) ? null : Email?.Trim()
            );

            if (success)
            {
                LoginData = data;
                StatusMessage = "注册成功！即将登录...";
                IsSuccess = true;
                await Task.Delay(1000); // 等待1秒让用户看到成功消息
                RequestClose?.Invoke(this, true);
            }
            else
            {
                StatusMessage = message;
                IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"注册失败：{ex.Message}";
            IsSuccess = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void CancelClick()
    {
        RequestClose?.Invoke(this, false);
    }

    public event EventHandler<object?>? RequestClose;

    public void Close()
    {
        RequestClose?.Invoke(this, false);
    }
}
