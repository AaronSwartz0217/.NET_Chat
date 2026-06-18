using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Ursa.Controls;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 登录窗口ViewModel
/// </summary>
public partial class LoginWindowViewModel : ViewModelBase, IDialogContext
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _rememberMe = true;

    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    /// <summary>
    /// 登录成功后的AccessToken
    /// </summary>
    public string? AccessToken { get; private set; }

    /// <summary>
    /// 登录成功后的用户ID
    /// </summary>
    public int? CurrentUserId { get; private set; }

    /// <summary>
    /// 登录成功后的用户昵称
    /// </summary>
    public string? NickName { get; private set; }

    public LoginWindowViewModel()
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
        if (string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "请输入密码";
            return;
        }

        IsLoading = true;
        StatusMessage = "正在登录...";

        try
        {
            // 使用原生HttpClient发送请求（更可靠）
            var response = await _httpClient.PostAsJsonAsync(
                "http://127.0.0.1:5002/api/auth/login",
                new { UserName = UserName.Trim(), Password = Password }
            );

            var ret = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(ret))
            {
                var loginResp = JsonSerializer.Deserialize<JsonElement>(ret);

                // 检查是否成功
                var succeeded = loginResp.TryGetProperty("succeeded", out var succEl) && succEl.GetBoolean();

                if (!succeeded)
                {
                    var errors = loginResp.TryGetProperty("errors", out var errEl) ? errEl.GetString() : "登录失败";
                    StatusMessage = errors ?? "登录失败";
                    IsLoading = false;
                    return;
                }

                // Furion RESTfulResult: 数据在 data 字段中
                JsonElement dataEl;
                if (loginResp.TryGetProperty("data", out dataEl))
                {
                    // 从 data 中解析 Token
                    if (dataEl.TryGetProperty("accessToken", out var tokenEl))
                        AccessToken = tokenEl.GetString();
                    else if (dataEl.TryGetProperty("AccessToken", out var tokenEl2))
                        AccessToken = tokenEl2.GetString();

                    // 从 data 中解析用户信息
                    if (dataEl.TryGetProperty("userInfo", out var userEl))
                    {
                        if (userEl.TryGetProperty("userId", out var uidEl))
                            CurrentUserId = uidEl.GetInt32();
                        if (userEl.TryGetProperty("nickname", out var nickEl))
                            NickName = nickEl.GetString();
                        if (userEl.TryGetProperty("userName", out var nameEl) && string.IsNullOrEmpty(NickName))
                            NickName = nameEl.GetString();
                    }
                }
                else
                {
                    // 兼容：如果没有 data 字段，尝试从根级别解析（非 Furion 格式）
                    if (loginResp.TryGetProperty("accessToken", out var tokenEl))
                        AccessToken = tokenEl.GetString();
                    if (loginResp.TryGetProperty("userInfo", out var userEl))
                    {
                        if (userEl.TryGetProperty("userId", out var uidEl))
                            CurrentUserId = uidEl.GetInt32();
                        if (userEl.TryGetProperty("nickname", out var nickEl))
                            NickName = nickEl.GetString();
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[Login] 解析结果: Token={AccessToken?.Substring(0, Math.Min(20, AccessToken?.Length ?? 0))}..., UserId={CurrentUserId}, NickName={NickName}");

                StatusMessage = "登录成功！";
                RequestClose?.Invoke(this, true);
            }
            else
            {
                StatusMessage = "服务器无响应，请检查网络";
            }
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            StatusMessage = "连接超时，请检查服务器是否启动（端口5002）";
        }
        catch (Exception ex)
        {
            StatusMessage = $"登录失败：{ex.Message}";
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

    [RelayCommand]
    private async Task RegisterClick()
    {
        // TODO: 注册功能待实现
    }

    public event EventHandler<object?>? RequestClose;

    public void Close()
    {
        RequestClose?.Invoke(this, false);
    }
}
