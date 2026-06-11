using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class PostService : IPostService
{
    private readonly ISqlSugarClient _db;

    public PostService(ISqlSugarClient db)
    {
        _db = db;
        _db.CodeFirst.InitTables<Post>();
        _db.CodeFirst.InitTables<PostLike>();
        _db.CodeFirst.InitTables<PostFavorite>();
    }

    public async Task<PostDto?> CreatePostAsync(int userId, CreatePostRequest request)
    {
        var post = new Post
        {
            UserId = userId,
            BoardId = request.BoardId,
            Title = request.Title,
            Content = request.Content,
            Type = request.Type,
            Status = 0,
            Tags = request.Tags,
            Images = request.Images,
            ShareCount = 0,
            LastCommentAt = null,
            Ip = null
        };

        var postId = await _db.Insertable(post).ExecuteReturnIdentityAsync();
        return await GetPostByIdAsync((int)postId, userId);
    }

    public async Task<PaginatedResponse<PostDto>> GetPostsAsync(PostQueryRequest request, int? userId = null)
    {
        var baseQuery = _db.Queryable<Post>()
            .Where(p => !p.IsDeleted && p.Status == 0);

        if (!string.IsNullOrEmpty(request.Tag))
        {
            baseQuery = baseQuery.Where(p => p.Tags!.Contains(request.Tag));
        }

        baseQuery = request.SortBy?.ToLower() == "hot"
            ? baseQuery.OrderByDescending(p => p.LikeCount + p.CommentCount * 2)
            : baseQuery.OrderByDescending(p => p.CreatedTime);

        var query = baseQuery
            .LeftJoin<User>((p, u) => p.UserId == u.Id)
            .Select((p, u) => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = u.UserName,
                Avatar = u.Avatar,
                BoardId = p.BoardId,
                Title = p.Title,
                Content = p.Content,
                Type = p.Type,
                Status = p.Status,
                Tags = p.Tags,
                Images = p.Images,
                ViewCount = p.ViewCount,
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                ShareCount = p.ShareCount,
                LastCommentAt = p.LastCommentAt,
                Ip = p.Ip,
                CreatedTime = p.CreatedTime,
                UpdatedTime = p.UpdatedTime,
                IsLiked = false,
                IsFavorited = false
            });

        var totalCount = await query.CountAsync();
        var data = await query.ToPageListAsync(request.PageIndex, request.PageSize);

        if (userId.HasValue)
        {
            foreach (var post in data)
            {
                post.IsLiked = await _db.Queryable<PostLike>()
                    .AnyAsync(l => l.UserId == userId.Value && l.PostId == post.Id);
                post.IsFavorited = await _db.Queryable<PostFavorite>()
                    .AnyAsync(f => f.UserId == userId.Value && f.PostId == post.Id);
            }
        }

        return new PaginatedResponse<PostDto>
        {
            Data = data,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }

    /// <summary>
    /// 获取指定用户的帖子列表（我的帖子/查看他人帖子）
    /// </summary>
    public async Task<PaginatedResponse<PostDto>> GetUserPostsAsync(int targetUserId, PostQueryRequest request, int? currentUserId = null)
    {
        var baseQuery = _db.Queryable<Post>()
            .Where(p => !p.IsDeleted && p.Status == 0 && p.UserId == targetUserId);

        if (!string.IsNullOrEmpty(request.Tag))
        {
            baseQuery = baseQuery.Where(p => p.Tags!.Contains(request.Tag));
        }

        baseQuery = baseQuery.OrderByDescending(p => p.CreatedTime);

        var query = baseQuery
            .LeftJoin<User>((p, u) => p.UserId == u.Id)
            .Select((p, u) => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = u.UserName,
                Avatar = u.Avatar,
                BoardId = p.BoardId,
                Title = p.Title,
                Content = p.Content,
                Type = p.Type,
                Status = p.Status,
                Tags = p.Tags,
                Images = p.Images,
                ViewCount = p.ViewCount,
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                ShareCount = p.ShareCount,
                LastCommentAt = p.LastCommentAt,
                Ip = p.Ip,
                CreatedTime = p.CreatedTime,
                UpdatedTime = p.UpdatedTime,
                IsLiked = false,
                IsFavorited = false
            });

        var totalCount = await query.CountAsync();
        var data = await query.ToPageListAsync(request.PageIndex, request.PageSize);

        if (currentUserId.HasValue)
        {
            foreach (var post in data)
            {
                post.IsLiked = await _db.Queryable<PostLike>()
                    .AnyAsync(l => l.UserId == currentUserId.Value && l.PostId == post.Id);
                post.IsFavorited = await _db.Queryable<PostFavorite>()
                    .AnyAsync(f => f.UserId == currentUserId.Value && f.PostId == post.Id);
            }
        }

        return new PaginatedResponse<PostDto>
        {
            Data = data,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }

    public async Task<PostDto?> GetPostByIdAsync(int postId, int? userId = null)
    {
        await _db.Updateable<Post>()
            .SetColumns(p => p.ViewCount == p.ViewCount + 1)
            .Where(p => p.Id == postId)
            .ExecuteCommandAsync();

        var post = await _db.Queryable<Post>()
            .Where(p => p.Id == postId && !p.IsDeleted && p.Status == 0)
            .LeftJoin<User>((p, u) => p.UserId == u.Id)
            .Select((p, u) => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = u.UserName,
                Avatar = u.Avatar,
                BoardId = p.BoardId,
                Title = p.Title,
                Content = p.Content,
                Type = p.Type,
                Status = p.Status,
                Tags = p.Tags,
                Images = p.Images,
                ViewCount = p.ViewCount,
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                ShareCount = p.ShareCount,
                LastCommentAt = p.LastCommentAt,
                Ip = p.Ip,
                CreatedTime = p.CreatedTime,
                UpdatedTime = p.UpdatedTime,
                IsLiked = false,
                IsFavorited = false
            }).FirstAsync();

        if (post != null && userId.HasValue)
        {
            post.IsLiked = await _db.Queryable<PostLike>()
                .AnyAsync(l => l.UserId == userId.Value && l.PostId == postId);
            post.IsFavorited = await _db.Queryable<PostFavorite>()
                .AnyAsync(f => f.UserId == userId.Value && f.PostId == postId);
        }

        return post;
    }

    public async Task<SimpleResponse> UpdatePostAsync(int postId, int userId, CreatePostRequest request, string? role = null)
    {
        var post = await _db.Queryable<Post>().FirstAsync(p => p.Id == postId);
        if (post == null)
        {
            return new SimpleResponse { Success = false, Message = "帖子不存在" };
        }

        // 只有帖子作者或管理员可以编辑帖子
        if (post.UserId != userId && role != "admin")
        {
            return new SimpleResponse { Success = false, Message = "无权修改此帖子" };
        }

        var result = await _db.Updateable<Post>()
            .SetColumns(p => p.BoardId == request.BoardId)
            .SetColumns(p => p.Title == request.Title)
            .SetColumns(p => p.Content == request.Content)
            .SetColumns(p => p.Type == request.Type)
            .SetColumns(p => p.Tags == request.Tags)
            .SetColumns(p => p.Images == request.Images)
            .SetColumns(p => p.UpdatedTime == DateTime.UtcNow)
            .Where(p => p.Id == postId)
            .ExecuteCommandAsync();

        return result > 0
            ? new SimpleResponse { Success = true, Message = "修改成功" }
            : new SimpleResponse { Success = false, Message = "修改失败" };
    }

    public async Task<SimpleResponse> DeletePostAsync(int postId, int userId, string? role = null)
    {
        var post = await _db.Queryable<Post>().FirstAsync(p => p.Id == postId);
        if (post == null)
        {
            return new SimpleResponse { Success = false, Message = "帖子不存在" };
        }

        // 只有帖子作者或管理员可以删除帖子
        if (post.UserId != userId && role != "admin")
        {
            return new SimpleResponse { Success = false, Message = "无权删除此帖子" };
        }

        var result = await _db.Updateable<Post>()
            .SetColumns(p => p.IsDeleted == true)
            .Where(p => p.Id == postId)
            .ExecuteCommandAsync();

        return result > 0
            ? new SimpleResponse { Success = true, Message = "删除成功" }
            : new SimpleResponse { Success = false, Message = "删除失败" };
    }

    public async Task<SimpleResponse> ToggleLikeAsync(int postId, int userId)
    {
        var existing = await _db.Queryable<PostLike>()
            .FirstAsync(l => l.UserId == userId && l.PostId == postId);

        if (existing != null)
        {
            await _db.Deleteable<PostLike>()
                .Where(l => l.UserId == userId && l.PostId == postId)
                .ExecuteCommandAsync();

            await _db.Updateable<Post>()
                .SetColumns(p => p.LikeCount == p.LikeCount - 1)
                .Where(p => p.Id == postId)
                .ExecuteCommandAsync();

            return new SimpleResponse { Success = true, Message = "取消点赞" };
        }
        else
        {
            await _db.Insertable(new PostLike
            {
                UserId = userId,
                PostId = postId
            }).ExecuteCommandAsync();

            await _db.Updateable<Post>()
                .SetColumns(p => p.LikeCount == p.LikeCount + 1)
                .Where(p => p.Id == postId)
                .ExecuteCommandAsync();

            return new SimpleResponse { Success = true, Message = "点赞成功" };
        }
    }

    public async Task<SimpleResponse> ToggleFavoriteAsync(int postId, int userId)
    {
        var existing = await _db.Queryable<PostFavorite>()
            .FirstAsync(f => f.UserId == userId && f.PostId == postId);

        if (existing != null)
        {
            await _db.Deleteable<PostFavorite>()
                .Where(f => f.UserId == userId && f.PostId == postId)
                .ExecuteCommandAsync();

            return new SimpleResponse { Success = true, Message = "取消收藏" };
        }
        else
        {
            await _db.Insertable(new PostFavorite
            {
                UserId = userId,
                PostId = postId
            }).ExecuteCommandAsync();

            return new SimpleResponse { Success = true, Message = "收藏成功" };
        }
    }
}
