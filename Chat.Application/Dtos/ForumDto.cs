using System;

namespace Chat.Application.Dtos;

// ===== 认证相关 =====
public class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? Token { get; set; }  // 兼容旧代码
    public int ExpiresIn { get; set; }
    public UserProfileDto? UserInfo { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

// ===== 用户资料 =====
public class UserProfileDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string? Avatar { get; set; }
    public string? Signature { get; set; }
    public bool OnlineStatus { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Role { get; set; } = "user";
    public bool HasProfile { get; set; }
    public string? Name { get; set; }
    public string? No { get; set; }
    public string? IdNumber { get; set; }
    public int? Gender { get; set; }
    public int? EthnicGroup { get; set; }
    public string? NativePlace { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Weight { get; set; }
    public decimal? Height { get; set; }
}

public class UpdateProfileRequest
{
    public string? Nickname { get; set; }
    public string? Avatar { get; set; }
    public string? Signature { get; set; }
    public string? Name { get; set; }
    public string? No { get; set; }
    public string? IdNumber { get; set; }
    public int? Gender { get; set; }
    public int? EthnicGroup { get; set; }
    public string? NativePlace { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Weight { get; set; }
    public decimal? Height { get; set; }
}

// ===== 帖子相关 =====
public class PostDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public int? BoardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Status { get; set; }
    public string? Tags { get; set; }
    public string? Images { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public DateTime? LastCommentAt { get; set; }
    public string? Ip { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
    public bool IsLiked { get; set; }
    public bool IsFavorited { get; set; }
}

public class CreatePostRequest
{
    public int? BoardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Type { get; set; } = 0;
    public string? Tags { get; set; }
    public string? Images { get; set; }
}

public class PostQueryRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "time"; // time, hot
    public string? Tag { get; set; }
}

// ===== 评论相关 =====
public class CommentDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}

// ===== 会话相关 =====
public class ChannelDto
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class CreateChannelRequest
{
    public int Type { get; set; } = 0;
    public string? Name { get; set; }
    public int[]? MemberIds { get; set; }
}

// ===== 消息相关 =====
public class MessageDto
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Type { get; set; }
    public bool Recalled { get; set; }
    public int? ReplyTo { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public int Type { get; set; } = 0;
    public int? ReplyTo { get; set; }
}

// ===== 通知相关 =====
public class NotificationDto
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int? RelatedId { get; set; }
    public bool Read { get; set; }
    public DateTime CreatedTime { get; set; }
}

// ===== 搜索相关 =====
public class SearchRequest
{
    public string Q { get; set; } = string.Empty;
    public string? Type { get; set; } = "all"; // all, posts, users, comments
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class SearchResultDto
{
    public List<PostDto>? Posts { get; set; }
    public List<UserProfileDto>? Users { get; set; }
    public int TotalCount { get; set; }
}

// ===== 通用响应 =====
public class SimpleResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class PaginatedResponse<T>
{
    public List<T>? Data { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
