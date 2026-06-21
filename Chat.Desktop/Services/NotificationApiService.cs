using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.Desktop.Services;

/// <summary>
/// 通知API服务
/// </summary>
public class NotificationApiService
{
    private readonly HttpClient _httpClient;

    public NotificationApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(AppConfig.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    public void SetToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    /// <summary>
    /// 获取通知列表
    /// GET /api/notifications
    /// </summary>
    public async Task<List<NotificationModel>?> GetNotificationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/notifications");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[Notification] 响应: {json.Substring(0, Math.Min(500, json.Length))}");

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("data", out var dataEl))
            {
                var notifications = new List<NotificationModel>();
                foreach (var item in dataEl.EnumerateArray())
                {
                    var notification = JsonSerializer.Deserialize<NotificationModel>(
                        item.GetRawText(), 
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                    );
                    if (notification != null)
                        notifications.Add(notification);
                }
                return notifications;
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Notification] 获取通知失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 单条标记已读
    /// PUT /api/notifications/{id}/read
    /// </summary>
    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/api/notifications/{notificationId}/read", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Notification] 标记已读失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 全部标记已读
    /// PUT /api/notifications/read-all
    /// </summary>
    public async Task<bool> MarkAllAsReadAsync()
    {
        try
        {
            var response = await _httpClient.PutAsync("/api/notifications/read-all", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Notification] 全部标记已读失败: {ex.Message}");
            return false;
        }
    }
}

/// <summary>
/// 通知模型
/// </summary>
public class NotificationModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public string? TypeName { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? RelatedId { get; set; }
    public string? RelatedType { get; set; }
    public string? Icon { get; set; }
}
