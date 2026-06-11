using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Furion.HttpRemote;
using Irihi.Avalonia.Shared.Contracts;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Ursa.Controls;
using Chat.Desktop.Views;

namespace Chat.Desktop.ViewModels;

public partial class LoginWindowViewModel : ViewModelBase, IDialogContext
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    private readonly IHttpRemoteService _httpRemoteService;

    /// <summary>
    /// 登录成功后的AccessToken
    /// </summary>
    public string? AccessToken { get; private set; }

    /// <summary>
    /// 登录成功后的用户ID
    /// </summary>
    public int? CurrentUserId { get; private set; }

    public LoginWindowViewModel(IHttpRemoteService httpRemoteService)
    {
        _httpRemoteService = httpRemoteService;
    }

    [RelayCommand]
    private async Task OkClick()
    {
        try
        {
            var ret = await _httpRemoteService.PostAsAsync<string>(
                $"http://127.0.0.1:5002/api/auth/login"
                , builder => builder.SetJsonContent(
                    new
                    {
                        UserName = UserName,
                        Password = Password
                    })
                    .SetTimeout(30)
                );

            if (!string.IsNullOrEmpty(ret))
            {
                // 解析登录响应获取Token和UserId
                try
                {
                    var loginResp = JsonSerializer.Deserialize<JsonElement>(ret);
                    if (loginResp.TryGetProperty("accessToken", out var tokenEl))
                        AccessToken = tokenEl.GetString();
                    if (loginResp.TryGetProperty("userInfo", out var userEl))
                    {
                        if (userEl.TryGetProperty("userId", out var uidEl))
                            CurrentUserId = uidEl.GetInt32();
                    }
                }
                catch
                {
                    // 解析失败时使用原始响应
                }

                RequestClose?.Invoke(this, true);
            }
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            await MessageBox.ShowAsync("网络连接超时，请检查服务器是否运行正常", "请求超时", MessageBoxIcon.Error, MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(ex.Message, "登录失败", MessageBoxIcon.Error, MessageBoxButton.OK);
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
        var vm = new RegisterViewModel();
        var ret = await Dialog.ShowCustomAsync<
            RegisterView,
            RegisterViewModel,
            bool>(vm);
        if (ret)
        {

        }
    }

    public event EventHandler<object?>? RequestClose;
    public void Close()
    {
        RequestClose?.Invoke(this, false);
    }
}
