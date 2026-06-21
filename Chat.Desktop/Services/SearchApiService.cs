using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Chat.Desktop.Models;

namespace Chat.Desktop.Services;

/// <summary>
/// 搜索API服务
/// </summary>
public class SearchApiService
{
    private readonly HttpClient _httpClient;
    private string? _accessToken;

    public SearchApiService()
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
        _accessToken = token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    /// <summary>
    /// 全局搜索
    /// GET /api/search?q=keyword&type=posts&pageIndex=1&pageSize=10
    /// </summary>
    public async Task<SearchResult?> SearchAsync(string query, string type = "posts", int pageIndex = 1, int pageSize = 10)
    {
        try
        {
            var url = $"/api/search?q={Uri.EscapeDataString(query)}&type={type}&pageIndex={pageIndex}&pageSize={pageSize}";
            System.Diagnostics.Debug.WriteLine($"[Search] 请求URL: {url}");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[Search] 响应: {json.Substring(0, Math.Min(500, json.Length))}");

            var result = JsonSerializer.Deserialize<SearchResult>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Search] 搜索失败: {ex.Message}");
            return null;
        }
    }
}

/// <summary>
/// 搜索结果
/// </summary>
public class SearchResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<PostModel>? Posts { get; set; }
    public List<UserModel>? Users { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// 搜索用户模型
/// </summary>
public class UserModel
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? NickName { get; set; }
    public string? Avatar { get; set; }
    public string? Signature { get; set; }
    public DateTime? CreatedAt { get; set; }
}
