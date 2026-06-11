using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class UserServiceV2 : IUserServiceV2
{
    private readonly ISqlSugarClient _db;

    public UserServiceV2(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<UserProfileDto?> GetCurrentUserProfileAsync(int userId)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null) return null;

        return MapToProfileDto(user);
    }

    public async Task<UserProfileDto?> GetUserProfileByIdAsync(int userId)
    {
        return await GetCurrentUserProfileAsync(userId);
    }

    public async Task<UserProfileDto?> GetPublicProfileAsync(int userId)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null) return null;

        return new UserProfileDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Signature = user.Signature,
            OnlineStatus = user.OnlineStatus,
            CreatedTime = user.CreatedTime,
            Role = user.Role
        };
    }

    public async Task<SimpleResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null)
        {
            return new SimpleResponse
            {
                Success = false,
                Message = "用户不存在"
            };
        }

        var updateable = _db.Updateable<User>().Where(u => u.Id == userId);

        if (!string.IsNullOrEmpty(request.Nickname))
            updateable = updateable.SetColumns(u => u.Nickname == request.Nickname);
        if (!string.IsNullOrEmpty(request.Avatar))
            updateable = updateable.SetColumns(u => u.Avatar == request.Avatar);
        if (!string.IsNullOrEmpty(request.Signature))
            updateable = updateable.SetColumns(u => u.Signature == request.Signature);

        var result = await updateable.ExecuteCommandAsync();

        return result > 0
            ? new SimpleResponse { Success = true, Message = "资料更新成功" }
            : new SimpleResponse { Success = false, Message = "更新失败" };
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Queryable<User>().ToListAsync();
    }

    private UserProfileDto MapToProfileDto(User user)
    {
        return new UserProfileDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Signature = user.Signature,
            OnlineStatus = user.OnlineStatus,
            LastLoginTime = user.LastLoginTime,
            CreatedTime = user.CreatedTime,
            Role = user.Role
        };
    }
}
