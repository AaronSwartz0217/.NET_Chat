using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class CommentService : ICommentService
{
    private readonly ISqlSugarClient _db;
    private readonly INotificationService _notificationService;

    public CommentService(ISqlSugarClient db, INotificationService notificationService)
    {
        _db = db;
        _notificationService = notificationService;
        _db.CodeFirst.InitTables<Comment>();
    }

    public async Task<CommentDto?> CreateCommentAsync(int postId, int userId, CreateCommentRequest request)
    {
        var post = await _db.Queryable<Post>().FirstAsync(p => p.Id == postId);
        if (post == null) return null;

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Content = request.Content,
            ParentId = request.ParentId
        };

        var commentId = await _db.Insertable(comment).ExecuteReturnIdentityAsync();

        await _db.Updateable<Post>()
            .SetColumns(p => p.CommentCount == p.CommentCount + 1)
            .Where(p => p.Id == postId)
            .ExecuteCommandAsync();

        if (post.UserId != userId)
        {
            await _notificationService.CreateNotificationAsync(
                post.UserId,
                NotificationType.Comment,
                "新评论",
                $"您的帖子有新评论",
                postId
            );
        }

        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        return new CommentDto
        {
            Id = (int)commentId,
            PostId = postId,
            UserId = userId,
            UserName = user.UserName,
            Avatar = user.Avatar,
            Content = request.Content,
            ParentId = request.ParentId,
            CreatedTime = DateTime.UtcNow
        };
    }

    public async Task<PaginatedResponse<CommentDto>> GetCommentsAsync(int postId, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<Comment>()
            .Where(c => c.PostId == postId && !c.IsDeleted)
            .OrderBy(c => c.CreatedTime)
            .LeftJoin<User>((c, u) => c.UserId == u.Id)
            .Select((c, u) => new CommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                UserId = c.UserId,
                UserName = u.UserName,
                Avatar = u.Avatar,
                Content = c.Content,
                ParentId = c.ParentId,
                CreatedTime = c.CreatedTime
            });

        var totalCount = await query.CountAsync();
        var data = await query.ToPageListAsync(pageIndex, pageSize);

        return new PaginatedResponse<CommentDto>
        {
            Data = data,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<SimpleResponse> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _db.Queryable<Comment>().FirstAsync(c => c.Id == commentId);
        if (comment == null)
        {
            return new SimpleResponse { Success = false, Message = "评论不存在" };
        }
        if (comment.UserId != userId)
        {
            return new SimpleResponse { Success = false, Message = "无权删除此评论" };
        }

        await _db.Updateable<Comment>()
            .SetColumns(c => c.IsDeleted == true)
            .Where(c => c.Id == commentId)
            .ExecuteCommandAsync();

        await _db.Updateable<Post>()
            .SetColumns(p => p.CommentCount == p.CommentCount - 1)
            .Where(p => p.Id == comment.PostId)
            .ExecuteCommandAsync();

        return new SimpleResponse { Success = true, Message = "删除成功" };
    }
}
