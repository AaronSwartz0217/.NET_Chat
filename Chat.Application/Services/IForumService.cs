using Chat.Application.Dtos;
using Chat.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Application.Services;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<SimpleResponse> LogoutAsync(string refreshToken);
}

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserServiceV2
{
    Task<UserProfileDto?> GetCurrentUserProfileAsync(int userId);
    Task<UserProfileDto?> GetUserProfileByIdAsync(int userId);
    Task<SimpleResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<UserProfileDto?> GetPublicProfileAsync(int userId);
    Task<List<User>> GetAllUsersAsync();
}

/// <summary>
/// 帖子服务接口
/// </summary>
public interface IPostService
{
    Task<PostDto?> CreatePostAsync(int userId, CreatePostRequest request);
    Task<PaginatedResponse<PostDto>> GetPostsAsync(PostQueryRequest request, int? userId = null);
    Task<PaginatedResponse<PostDto>> GetUserPostsAsync(int targetUserId, PostQueryRequest request, int? currentUserId = null);
    Task<PostDto?> GetPostByIdAsync(int postId, int? userId = null);
    Task<SimpleResponse> UpdatePostAsync(int postId, int userId, CreatePostRequest request, string? role = null);
    Task<SimpleResponse> DeletePostAsync(int postId, int userId, string? role = null);
    Task<SimpleResponse> ToggleLikeAsync(int postId, int userId);
    Task<SimpleResponse> ToggleFavoriteAsync(int postId, int userId);
}

/// <summary>
/// 评论服务接口
/// </summary>
public interface ICommentService
{
    Task<CommentDto?> CreateCommentAsync(int postId, int userId, CreateCommentRequest request);
    Task<PaginatedResponse<CommentDto>> GetCommentsAsync(int postId, int pageIndex = 1, int pageSize = 20);
    Task<SimpleResponse> DeleteCommentAsync(int commentId, int userId);
}

/// <summary>
/// 会话服务接口
/// </summary>
public interface IChannelService
{
    Task<List<ChannelDto>> GetChannelsAsync(int userId);
    Task<ChannelDto?> CreateChannelAsync(int userId, CreateChannelRequest request);
    Task<PaginatedResponse<MessageDto>> GetMessagesAsync(int channelId, int userId, int pageIndex = 1, int pageSize = 50);
    Task<MessageDto?> SendMessageAsync(int channelId, int userId, SendMessageRequest request);
    Task<SimpleResponse> RecallMessageAsync(int messageId, int userId);
    Task<SimpleResponse> MarkAsReadAsync(int channelId, int userId);
}

/// <summary>
/// 通知服务接口
/// </summary>
public interface INotificationService
{
    Task<List<NotificationDto>> GetNotificationsAsync(int userId);
    Task<SimpleResponse> MarkAsReadAsync(int userId, int notificationId);
    Task<SimpleResponse> MarkAllAsReadAsync(int userId);
    Task CreateNotificationAsync(int userId, NotificationType type, string title, string content, int? relatedId = null);
}

/// <summary>
/// 搜索服务接口
/// </summary>
public interface ISearchService
{
    Task<SearchResultDto> SearchAsync(SearchRequest request);
}
