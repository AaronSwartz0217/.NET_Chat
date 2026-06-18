using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Chat.Desktop.Models;

namespace Chat.Desktop.Services;

/// <summary>
/// 个人中心API服务
/// 负责与后端用户资料API交互
/// </summary>
public class ProfileApiService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// JSON序列化选项 - 使用camelCase命名策略匹配后端API
    /// </summary>
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ProfileApiService()
    {
        _httpClient = new HttpClient {
            BaseAddress = new Uri(AppConfig.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    public void SetToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    /// <summary>
    /// 获取当前用户资料 (GET /api/account/profile)
    /// </summary>
    public async Task<UserProfileModel?> GetCurrentUserProfileAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/account/profile");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("data", out var dataEl))
            {
                return JsonSerializer.Deserialize<UserProfileModel>(dataEl.GetRawText(), _jsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Profile] 获取用户资料失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 更新个人资料 (PUT /api/account/profile) - 包含学生档案
    /// </summary>
    public async Task<bool> UpdateProfileAsync(
        string? nickname, string? signature,
        string? studentNo = null, string? realName = null, string? idNumber = null,
        int? gender = null, int? ethnicGroup = null, string? nativePlace = null,
        DateTime? birthday = null, int? weight = null, decimal? height = null)
    {
        try
        {
            var body = new
            {
                Nickname = nickname,
                Signature = signature,
                No = studentNo,
                Name = realName,
                IdNumber = idNumber,
                Gender = gender,
                EthnicGroup = ethnicGroup,
                NativePlace = nativePlace,
                Birthday = birthday,
                Weight = weight,
                Height = height
            };
            var content = new StringContent(
                JsonSerializer.Serialize(body, _jsonOptions),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync("/api/account/profile", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Profile] 更新资料失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 修改密码 (POST /api/account/change-password)
    /// </summary>
    public async Task<(bool Success, string Message)> ChangePasswordAsync(string oldPassword, string newPassword)
    {
        try
        {
            var body = new { OldPassword = oldPassword, NewPassword = newPassword };
            var content = new StringContent(
                JsonSerializer.Serialize(body, _jsonOptions),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("/api/account/change-password", content);
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return (true, "密码修改成功");

            // 解析错误信息
            using var doc = JsonDocument.Parse(json);
            string msg = "修改失败";
            if (doc.RootElement.TryGetProperty("message", out var msgEl))
                msg = msgEl.GetString() ?? msg;
            return (false, msg);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Profile] 修改密码失败: {ex.Message}");
            return (false, ex.Message);
        }
    }
}
