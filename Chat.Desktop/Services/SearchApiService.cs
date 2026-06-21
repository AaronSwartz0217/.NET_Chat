using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    /// <summary>
    /// JSON序列化选项 - 匹配Furion后端API格式
    /// </summary>
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

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

            // 解析Furion RESTfulResult - 数据在data字段中
            using var doc = JsonDocument.Parse(json);
            
            var result = new SearchResult();
            
            if (doc.RootElement.TryGetProperty("data", out var dataEl))
            {
                // 解析帖子列表
                if (dataEl.TryGetProperty("posts", out var postsEl) && postsEl.ValueKind == JsonValueKind.Array)
                {
                    result.Posts = new List<PostModel>();
                    foreach (var item in postsEl.EnumerateArray())
                    {
                        try
                        {
                            var post = JsonSerializer.Deserialize<PostModel>(item.GetRawText(), _jsonOptions);
                            if (post != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"[Search] 解析帖子: Id={post.Id}, Title={post.Title}");
                                result.Posts.Add(post);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Search] 解析帖子失败: {ex.Message}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[Search] posts字段不存在或不是数组");
                }
                
                // 解析用户列表
                if (dataEl.TryGetProperty("users", out var usersEl) && usersEl.ValueKind == JsonValueKind.Array)
                {
                    result.Users = new List<UserModel>();
                    foreach (var item in usersEl.EnumerateArray())
                    {
                        try
                        {
                            var user = JsonSerializer.Deserialize<UserModel>(item.GetRawText(), _jsonOptions);
                            if (user != null)
                                result.Users.Add(user);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Search] 解析用户失败: {ex.Message}");
                        }
                    }
                }
                
                // 解析总数
                if (dataEl.TryGetProperty("totalCount", out var totalEl))
                    result.TotalCount = totalEl.GetInt32();
                    
                if (dataEl.TryGetProperty("totalPages", out var pagesEl))
                    result.TotalPages = pagesEl.GetInt32();

                System.Diagnostics.Debug.WriteLine($"[Search] 解析结果: Posts={result.Posts?.Count}, Users={result.Users?.Count}, Total={result.TotalCount}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[Search] data字段不存在!");
            }

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
