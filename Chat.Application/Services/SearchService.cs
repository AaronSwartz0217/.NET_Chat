using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class SearchService : ISearchService
{
    private readonly ISqlSugarClient _db;

    public SearchService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<SearchResultDto> SearchAsync(SearchRequest request)
    {
        var result = new SearchResultDto();
        var searchType = request.Type?.ToLower() ?? "all";

        if (searchType == "all" || searchType == "posts")
        {
            var posts = await _db.Queryable<Post>()
                .Where(p => !p.IsDeleted && (p.Title.Contains(request.Q) || p.Content.Contains(request.Q)))
                .OrderByDescending(p => p.CreatedTime)
                .LeftJoin<User>((p, u) => p.UserId == u.Id)
                .Select((p, u) => new PostDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = u.UserName,
                    Avatar = u.Avatar,
                    Title = p.Title,
                    Content = p.Content,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    CreatedTime = p.CreatedTime,
                    IsLiked = false,
                    IsFavorited = false
                })
                .Take(request.PageSize)
                .ToListAsync();

            result.Posts = posts;
        }

        if (searchType == "all" || searchType == "users")
        {
            var users = await _db.Queryable<User>()
                .Where(u => u.UserName.Contains(request.Q) || u.Nickname.Contains(request.Q))
                .Select(u => new UserProfileDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Nickname = u.Nickname,
                    Avatar = u.Avatar,
                    Signature = u.Signature,
                    OnlineStatus = u.OnlineStatus,
                    CreatedTime = u.CreatedTime,
                    Role = u.Role
                })
                .Take(request.PageSize)
                .ToListAsync();

            result.Users = users;
        }

        result.TotalCount = (result.Posts?.Count ?? 0) + (result.Users?.Count ?? 0);
        return result;
    }
}
