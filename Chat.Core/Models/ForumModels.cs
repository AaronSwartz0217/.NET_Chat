using SqlSugar;
using System;

namespace Chat.Core.Models;

/// <summary>
/// Refresh Token表
/// </summary>
[SugarTable("refresh_tokens")]
public class RefreshToken
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "token", Length = 500, IsNullable = false)]
    public string Token { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "expires_at")]
    public DateTime ExpiresAt { get; set; }

    [SugarColumn(ColumnName = "revoked")]
    public bool Revoked { get; set; } = false;

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 帖子表
/// </summary>
[SugarTable("posts")]
public class Post
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "board_id", IsNullable = true)]
    public int? BoardId { get; set; }

    [SugarColumn(ColumnName = "title", Length = 255, IsNullable = false)]
    public string Title { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "content", ColumnDataType = "TEXT", IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "type")]
    public int Type { get; set; } = 0;

    [SugarColumn(ColumnName = "status")]
    public int Status { get; set; } = 0;

    [SugarColumn(ColumnName = "tags", Length = 200, IsNullable = true)]
    public string? Tags { get; set; }

    [SugarColumn(ColumnName = "images", Length = 1000, IsNullable = true)]
    public string? Images { get; set; }

    [SugarColumn(ColumnName = "view_count")]
    public int ViewCount { get; set; } = 0;

    [SugarColumn(ColumnName = "like_count")]
    public int LikeCount { get; set; } = 0;

    [SugarColumn(ColumnName = "comment_count")]
    public int CommentCount { get; set; } = 0;

    [SugarColumn(ColumnName = "share_count")]
    public int ShareCount { get; set; } = 0;

    [SugarColumn(ColumnName = "last_comment_at", IsNullable = true)]
    public DateTime? LastCommentAt { get; set; }

    [SugarColumn(ColumnName = "ip", Length = 45, IsNullable = true)]
    public string? Ip { get; set; }

    [SugarColumn(ColumnName = "is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    [SugarColumn(ColumnName = "updated_time")]
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 评论表
/// </summary>
[SugarTable("comments")]
public class Comment
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "post_id")]
    public int PostId { get; set; }

    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "content", Length = 2000, IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "parent_id")]
    public int? ParentId { get; set; }

    [SugarColumn(ColumnName = "is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 帖子点赞表
/// </summary>
[SugarTable("post_likes")]
public class PostLike
{
    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "post_id")]
    public int PostId { get; set; }

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 帖子收藏表
/// </summary>
[SugarTable("post_favorites")]
public class PostFavorite
{
    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "post_id")]
    public int PostId { get; set; }

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 会话表（聊天频道）
/// </summary>
[SugarTable("channels")]
public class Channel
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "type")]
    public ChannelType Type { get; set; } = ChannelType.Private;

    [SugarColumn(ColumnName = "name", Length = 100, IsNullable = true)]
    public string? Name { get; set; }

    [SugarColumn(ColumnName = "avatar", Length = 255, IsNullable = true)]
    public string? Avatar { get; set; }

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 会话成员表
/// </summary>
[SugarTable("channel_members")]
public class ChannelMember
{
    [SugarColumn(ColumnName = "channel_id")]
    public int ChannelId { get; set; }

    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "role", Length = 20)]
    public string Role { get; set; } = "member";

    [SugarColumn(ColumnName = "last_read_time")]
    public DateTime? LastReadTime { get; set; }

    [SugarColumn(ColumnName = "joined_time", IsOnlyIgnoreUpdate = true)]
    public DateTime JoinedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 消息表
/// </summary>
[SugarTable("messages")]
public class Message
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "channel_id")]
    public int ChannelId { get; set; }

    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "content", Length = 2000, IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "type")]
    public MessageType Type { get; set; } = MessageType.Text;

    [SugarColumn(ColumnName = "recalled")]
    public bool Recalled { get; set; } = false;

    [SugarColumn(ColumnName = "reply_to")]
    public int? ReplyTo { get; set; }

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 通知表
/// </summary>
[SugarTable("notifications")]
public class Notification
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "user_id")]
    public int UserId { get; set; }

    [SugarColumn(ColumnName = "type")]
    public NotificationType Type { get; set; }

    [SugarColumn(ColumnName = "title", Length = 100, IsNullable = false)]
    public string Title { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "content", Length = 500, IsNullable = false)]
    public string Content { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "related_id")]
    public int? RelatedId { get; set; }

    [SugarColumn(ColumnName = "read")]
    public bool Read { get; set; } = false;

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 会话类型枚举
/// </summary>
public enum ChannelType
{
    Private = 0,  // 私聊
    Group = 1     // 群聊
}

/// <summary>
/// 消息类型枚举
/// </summary>
public enum MessageType
{
    Text = 0,
    Image = 1,
    File = 2,
    Quote = 3,
    System = 4
}

/// <summary>
/// 通知类型枚举
/// </summary>
public enum NotificationType
{
    Like = 0,      // 点赞
    Comment = 1,   // 评论
    Mention = 2,   // @我
    Follow = 3,    // 关注
    System = 4     // 系统消息
}
