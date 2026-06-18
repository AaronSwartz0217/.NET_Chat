using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Chat.Desktop.Models;

namespace Chat.Desktop.Services;

/// <summary>
/// 论坛API服务
/// 负责与后端REST API交互
/// </summary>
public class ForumApiService
{
    private readonly HttpClient _httpClient;
    private string? _accessToken;

    /// <summary>
    /// JSON序列化选项 - 使用camelCase命名策略匹配后端API
    /// </summary>
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ForumApiService()
    {
        _httpClient = new HttpClient {
            BaseAddress = new Uri(AppConfig.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(10)  // 10秒超时
        };
        // 设置 UTF-8 编码支持
        _httpClient.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    public void SetToken(string token)
    {
        _accessToken = token;
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
    /// 获取帖子列表（分页）
    /// </summary>
    public async Task<(List<PostModel>? Posts, int TotalCount, int TotalPages)> GetPostsAsync(int pageIndex = 1, int pageSize = 10, string sortBy = "time", string? tag = null)
    {
        try
        {
            var fullUrl = $"{AppConfig.ApiBaseUrl}/api/posts?pageIndex={pageIndex}&pageSize={pageSize}&sortBy={sortBy}";
            if (!string.IsNullOrEmpty(tag))
                fullUrl += $"&tag={Uri.EscapeDataString(tag)}";
            
            System.Diagnostics.Debug.WriteLine($"[Forum] 请求URL: {fullUrl}");
            System.Diagnostics.Debug.WriteLine($"[Forum] Authorization header: {(_httpClient.DefaultRequestHeaders.Authorization?.Parameter != null ? "已设置" : "未设置")}");

            var response = await _httpClient.GetAsync(fullUrl);
            System.Diagnostics.Debug.WriteLine($"[Forum] HTTP状态码: {(int)response.StatusCode}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[Forum] GetPosts API原始响应: {json.Substring(0, Math.Min(1000, json.Length))}");

            using var doc = JsonDocument.Parse(json);

            // Furion RESTfulResult 包装在 data 字段中
            if (doc.RootElement.TryGetProperty("data", out var dataEl))
            {
                System.Diagnostics.Debug.WriteLine($"[Forum] 找到外层data字段");
                var posts = new List<PostModel>();
                var totalCount = 0;
                var totalPages = 0;

                if (dataEl.TryGetProperty("data", out var listEl) && listEl.ValueKind == JsonValueKind.Array)
                {
                    System.Diagnostics.Debug.WriteLine($"[Forum] 找到帖子数组，长度={listEl.GetArrayLength()}");
                    foreach (var item in listEl.EnumerateArray())
                    {
                        var rawJson = item.GetRawText();
                        System.Diagnostics.Debug.WriteLine($"[Forum] 帖子原始JSON: {rawJson}");
                        var post = JsonSerializer.Deserialize<PostModel>(rawJson, _jsonOptions) ?? new PostModel();
                        System.Diagnostics.Debug.WriteLine($"[Forum] 解析后 - Id={post.Id}, Title='{post.Title}', UserName='{post.UserName}', Content长度={post.Content?.Length ?? 0}");
                        posts.Add(post);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[Forum] 未找到帖子数组或格式错误");
                }

                if (dataEl.TryGetProperty("totalCount", out var tc))
                    totalCount = tc.GetInt32();
                if (dataEl.TryGetProperty("totalPages", out var tp))
                    totalPages = tp.GetInt32();

                System.Diagnostics.Debug.WriteLine($"[Forum] 返回: posts={posts.Count}, totalCount={totalCount}, totalPages={totalPages}");
                return (posts, totalCount, totalPages);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Forum] 未找到外层data字段");
            }

            return (null, 0, 0);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 获取帖子列表失败: {ex.Message}");
            return (null, 0, 0);
        }
    }

    /// <summary>
    /// 获取帖子详情
    /// </summary>
    public async Task<PostModel?> GetPostAsync(int postId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/posts/{postId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("data", out var dataEl))
            {
                return JsonSerializer.Deserialize<PostModel>(dataEl.GetRawText(), _jsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 获取帖子详情失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 创建帖子
    /// </summary>
    public async Task<PostModel?> CreatePostAsync(string title, string content, string? tags = null)
    {
        try
        {
            var body = new { Title = title, Content = content, Tags = tags };
            var contentStr = JsonSerializer.Serialize(body, _jsonOptions);
            var httpContent = new StringContent(contentStr, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/posts", httpContent);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("data", out var dataEl))
            {
                return JsonSerializer.Deserialize<PostModel>(dataEl.GetRawText(), _jsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 创建帖子失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 删除帖子
    /// </summary>
    public async Task<bool> DeletePostAsync(int postId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/posts/{postId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 删除帖子失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 收藏/取消收藏（POST 收藏，DELETE 取消收藏）
    /// </summary>
    public async Task<bool> ToggleFavoriteAsync(int postId, bool isCurrentlyFavorited)
    {
        try
        {
            HttpResponseMessage response;
            if (isCurrentlyFavorited)
                response = await _httpClient.DeleteAsync($"/api/posts/{postId}/favorite");
            else
                response = await _httpClient.PostAsync($"/api/posts/{postId}/favorite", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 收藏操作失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 点赞/取消点赞（POST 点赞，DELETE 取消）
    /// </summary>
    public async Task<bool> ToggleLikeAsync(int postId, bool isCurrentlyLiked)
    {
        try
        {
            HttpResponseMessage response;
            if (isCurrentlyLiked)
                response = await _httpClient.DeleteAsync($"/api/posts/{postId}/like");
            else
                response = await _httpClient.PostAsync($"/api/posts/{postId}/like", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 点赞操作失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取评论列表
    /// </summary>
    public async Task<(List<CommentModel>? Comments, int TotalCount, int TotalPages)> GetCommentsAsync(int postId, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/posts/{postId}/comments?pageIndex={pageIndex}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("data", out var dataEl))
            {
                var comments = new List<CommentModel>();
                int totalCount = 0;
                int totalPages = 0;

                if (dataEl.TryGetProperty("data", out var listEl) && listEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in listEl.EnumerateArray())
                    {
                        var comment = JsonSerializer.Deserialize<CommentModel>(item.GetRawText(), _jsonOptions);
                        if (comment != null)
                            comments.Add(comment);
                    }
                }

                if (dataEl.TryGetProperty("totalCount", out var tc))
                    totalCount = tc.GetInt32();
                if (dataEl.TryGetProperty("totalPages", out var tp))
                    totalPages = tp.GetInt32();

                return (comments, totalCount, totalPages);
            }
            return (null, 0, 0);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 获取评论失败: {ex.Message}");
            return (null, 0, 0);
        }
    }

    /// <summary>
    /// 发表评论
    /// </summary>
    public async Task<bool> AddCommentAsync(int postId, string content, int? parentId = null)
    {
        try
        {
            var body = new { Content = content, ParentId = parentId };
            var httpContent = new StringContent(
                JsonSerializer.Serialize(body, _jsonOptions),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"/api/posts/{postId}/comments", httpContent);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 发表评论失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 删除评论
    /// </summary>
    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/comments/{commentId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Forum] 删除评论失败: {ex.Message}");
            return false;
        }
    }
}
