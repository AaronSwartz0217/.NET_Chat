using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.Desktop.Services;

/// <summary>
/// 注册API服务
/// </summary>
public class RegisterApiService
{
    private readonly HttpClient _httpClient;

    public RegisterApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(AppConfig.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    /// <summary>
    /// 注册新用户
    /// POST /api/auth/register
    /// </summary>
    public async Task<(bool Success, string Message, LoginResponse? Data)> RegisterAsync(
        string userName, string password, string? nickname = null, string? email = null)
    {
        try
        {
            var body = new
            {
                UserName = userName.Trim(),
                Password = password,
                NickName = nickname?.Trim(),
                Email = email?.Trim()
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", body);
            var json = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[Register] HTTP状态码: {(int)response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"[Register] 响应内容: {json}");

            var result = JsonSerializer.Deserialize<JsonElement>(json);

            // 检查是否成功
            var succeeded = result.TryGetProperty("succeeded", out var succEl) && succEl.GetBoolean();
            var message = "注册结果未知";

            if (result.TryGetProperty("message", out var msgEl))
                message = msgEl.GetString() ?? message;
            else if (result.TryGetProperty("Message", out var msgEl2))
                message = msgEl2.GetString() ?? message;

            if (!succeeded)
            {
                // 尝试获取详细错误
                if (result.TryGetProperty("errors", out var errEl))
                    message = errEl.GetString() ?? message;
                else if (result.TryGetProperty("data", out var dataEl) && dataEl.TryGetProperty("message", out var dataMsgEl))
                    message = dataMsgEl.GetString() ?? message;

                return (false, message, null);
            }

            // 解析返回数据
            LoginResponse? loginData = null;
            if (result.TryGetProperty("data", out var respData))
            {
                loginData = JsonSerializer.Deserialize<LoginResponse>(respData.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }

            return (true, message ?? "注册成功！", loginData);
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Register] HTTP错误: {ex.Message}");
            return (false, $"网络错误：{ex.Message}", null);
        }
        catch (TaskCanceledException)
        {
            return (false, "连接超时，请检查服务器是否启动（端口5002）", null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Register] 异常: {ex.Message}");
            return (false, $"注册失败：{ex.Message}", null);
        }
    }
}

/// <summary>
/// 登录响应数据
/// </summary>
public class LoginResponse
{
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public UserInfo? UserInfo { get; set; }
}

public class UserInfo
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? NickName { get; set; }
    public string? Role { get; set; }
}
