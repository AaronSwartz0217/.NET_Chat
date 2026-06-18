using System;

namespace Chat.Desktop.Models;

/// <summary>
/// 帖子模型
/// </summary>
public class PostModel
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
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    /// <summary>
    /// 当前用户是否已点赞
    /// </summary>
    public bool IsLiked { get; set; }

    /// <summary>
    /// 当前用户是否已收藏
    /// </summary>
    public bool IsFavorited { get; set; }

    /// <summary>
    /// 格式化的创建时间
    /// </summary>
    public string FormattedTime => CreatedTime.ToString("MM-dd HH:mm");

    /// <summary>
    /// 截断的内容（用于列表显示）
    /// </summary>
    public string ShortContent => Content.Length > 100 ? Content.Substring(0, 100) + "..." : Content;
}

/// <summary>
/// 评论模型
/// </summary>
public class CommentModel
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public DateTime CreatedTime { get; set; }

    public string FormattedTime => CreatedTime.ToString("MM-dd HH:mm");
}

/// <summary>
/// 用户资料模型
/// </summary>
public class UserProfileModel
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

    // ===== 学生档案字段（与后端 UserProfileDto 对应） =====
    public string? No { get; set; }              // 学号
    public string? Name { get; set; }            // 真实姓名
    public string? IdNumber { get; set; }        // 身份证号
    public int? Gender { get; set; }             // 0=未知, 1=男, 2=女（后端为可空）
    public int? EthnicGroup { get; set; }        // 民族编码（后端为可空）
    public string? NativePlace { get; set; }     // 籍贯
    public DateTime? Birthday { get; set; }      // 生日
    public int? Weight { get; set; }             // 体重kg（后端为可空）
    public decimal? Height { get; set; }         // 身高cm（后端为可空）

    /// <summary>显示名称（优先昵称）</summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(Nickname) ? Nickname : UserName;

    /// <summary>格式化的注册时间</summary>
    public string FormattedCreatedTime => CreatedTime.ToString("yyyy-MM-dd");

    /// <summary>格式化的最后登录时间</summary>
    public string FormattedLastLogin => LastLoginTime?.ToString("yyyy-MM-dd HH:mm") ?? "未知";

    /// <summary>性别文本</summary>
    public string GenderText => Gender switch { 1 => "男", 2 => "女", _ => "未设置" };

    /// <summary>格式化的生日</summary>
    public string FormattedBirthday => Birthday?.ToString("yyyy-MM-dd") ?? "未设置";
}
