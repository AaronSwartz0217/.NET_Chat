using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class AuthService : IAuthService
{
    private readonly ISqlSugarClient _db;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ISqlSugarClient db, IJwtService jwtService, JwtSettings jwtSettings)
    {
        _db = db;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
        _db.CodeFirst.InitTables<User>();
        _db.CodeFirst.InitTables<RefreshToken>();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Queryable<User>()
            .FirstAsync(u => u.UserName == request.UserName);

        if (user == null || user.Password != request.Password)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        var accessToken = _jwtService.GenerateToken(user.Id, user.UserName, user.Role);
        var refreshToken = GenerateRefreshToken();

        await _db.Insertable(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        }).ExecuteCommandAsync();

        await _db.Updateable<User>()
            .SetColumns(u => u.LastLoginTime == DateTime.UtcNow)
            .SetColumns(u => u.OnlineStatus == true)
            .Where(u => u.Id == user.Id)
            .ExecuteCommandAsync();

        return new LoginResponse
        {
            Success = true,
            Message = "登录成功",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.ExpiresInSeconds,
            UserInfo = new UserProfileDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Nickname = user.Nickname,
                Avatar = user.Avatar,
                Signature = user.Signature,
                OnlineStatus = true,
                LastLoginTime = DateTime.UtcNow,
                CreatedTime = user.CreatedTime,
                Role = user.Role
            }
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshToken = await _db.Queryable<RefreshToken>()
            .FirstAsync(r => r.Token == request.RefreshToken && !r.Revoked && r.ExpiresAt > DateTime.UtcNow);

        if (refreshToken == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "无效的刷新令牌"
            };
        }

        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == refreshToken.UserId);
        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "用户不存在"
            };
        }

        var newAccessToken = _jwtService.GenerateToken(user.Id, user.UserName, user.Role);
        var newRefreshToken = GenerateRefreshToken();

        await _db.Updateable<RefreshToken>()
            .SetColumns(r => r.Revoked == true)
            .Where(r => r.Id == refreshToken.Id)
            .ExecuteCommandAsync();

        await _db.Insertable(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        }).ExecuteCommandAsync();

        return new LoginResponse
        {
            Success = true,
            Message = "令牌刷新成功",
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _jwtSettings.ExpiresInSeconds,
            UserInfo = new UserProfileDto
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
            }
        };
    }

    public async Task<SimpleResponse> LogoutAsync(string refreshToken)
    {
        var token = await _db.Queryable<RefreshToken>()
            .FirstAsync(r => r.Token == refreshToken);

        if (token == null)
        {
            return new SimpleResponse
            {
                Success = false,
                Message = "令牌不存在"
            };
        }

        await _db.Updateable<RefreshToken>()
            .SetColumns(r => r.Revoked == true)
            .Where(r => r.Id == token.Id)
            .ExecuteCommandAsync();

        return new SimpleResponse
        {
            Success = true,
            Message = "登出成功"
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
