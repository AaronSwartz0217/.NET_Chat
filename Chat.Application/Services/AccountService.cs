using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Application.Services;

/// <summary>
/// 账号档案服务实现 - 业务层面合并User和Student
/// 底层使用两个独立的数据库表，通过业务代码组合
/// </summary>
public class AccountService : IAccountService
{
    private readonly ISqlSugarClient _db;
    private readonly IJwtService _jwtService;

    public AccountService(ISqlSugarClient db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
        _db.CodeFirst.InitTables<User>();
        _db.CodeFirst.InitTables<Student>();
    }

    /// <summary>
    /// 注册账号（POST方式）
    /// </summary>
    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _db.Queryable<User>()
            .FirstAsync(u => u.UserName == request.UserName);
        if (existing != null)
        {
            return new LoginResponse { Success = false, Message = "用户名已存在" };
        }

        var user = new User
        {
            UserName = request.UserName,
            Password = request.Password,
            LastLoginTime = DateTime.UtcNow,
            CreatedTime = DateTime.UtcNow
        };
        var userId = await _db.Insertable(user).ExecuteReturnIdentityAsync();

        var token = _jwtService.GenerateToken((int)userId, request.UserName);
        var profile = await BuildUserProfileDtoAsync(user);

        return new LoginResponse
        {
            Success = true,
            Message = "注册成功",
            Token = token,
            ExpiresIn = 3600,
            UserInfo = profile
        };
    }

    /// <summary>
    /// 登录验证（POST方式，返回JWT）
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Queryable<User>()
            .FirstAsync(u => u.UserName == request.UserName);

        if (user == null)
        {
            return new LoginResponse { Success = false, Message = "用户不存在" };
        }

        if (user.Password != request.Password)
        {
            return new LoginResponse { Success = false, Message = "密码错误" };
        }

        var token = _jwtService.GenerateToken(user.Id, user.UserName);
        var profile = await BuildUserProfileDtoAsync(user);

        return new LoginResponse
        {
            Success = true,
            Message = "登录成功",
            Token = token,
            ExpiresIn = 3600,
            UserInfo = profile
        };
    }

    /// <summary>
    /// 根据用户ID获取账号档案（安全版本，从token解析）
    /// </summary>
    public async Task<UserProfileDto?> GetProfileByIdAsync(int userId)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null) return null;
        return await BuildUserProfileDtoAsync(user);
    }

    /// <summary>
    /// 获取当前登录用户资料（从token解析用户ID）
    /// </summary>
    public async Task<UserProfileDto?> GetCurrentUserProfileAsync(string token)
    {
        var userId = _jwtService.GetUserIdFromToken(token);
        if (!userId.HasValue) return null;
        return await GetProfileByIdAsync(userId.Value);
    }

    /// <summary>
    /// 获取所有账号（安全版本）
    /// </summary>
    public async Task<List<UserProfileDto>> GetAllAccountsSecureAsync()
    {
        var users = await _db.Queryable<User>().ToListAsync();
        var students = await _db.Queryable<Student>().ToListAsync();

        var result = new List<UserProfileDto>();
        foreach (var user in users)
        {
            var profile = students.FirstOrDefault(s => s.Name == user.UserName);
            result.Add(new UserProfileDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = profile?.Name,
                No = profile?.No,
                HasProfile = profile != null
            });
        }
        return result;
    }

    /// <summary>
    /// 修改密码（需验证旧密码）
    /// </summary>
    public async Task<SimpleResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null)
        {
            return new SimpleResponse { Success = false, Message = "用户不存在" };
        }

        if (user.Password != request.OldPassword)
        {
            return new SimpleResponse { Success = false, Message = "旧密码错误" };
        }

        var result = await _db.Updateable<User>()
            .SetColumns(u => u.Password == request.NewPassword)
            .Where(u => u.Id == userId)
            .ExecuteCommandAsync();

        return result > 0
            ? new SimpleResponse { Success = true, Message = "密码修改成功" }
            : new SimpleResponse { Success = false, Message = "修改失败" };
    }

    /// <summary>
    /// 更新用户资料（从token获取用户ID）
    /// </summary>
    public async Task<SimpleResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null)
        {
            return new SimpleResponse { Success = false, Message = "用户不存在" };
        }

        var existing = await _db.Queryable<Student>()
            .FirstAsync(s => s.Name == user.UserName);

        if (existing == null)
        {
            var student = new Student
            {
                No = request.No ?? string.Empty,
                Name = user.UserName,
                IdNumber = request.IdNumber ?? string.Empty,
                Gender = (EnumGender)(request.Gender ?? 0),
                EthnicGroup = (EnumEthnicGroup)(request.EthnicGroup ?? 0),
                NativePlace = request.NativePlace,
                Birthday = request.Birthday ?? DateTime.MinValue,
                Weight = request.Weight ?? 0,
                Height = request.Height ?? 0m
            };
            await _db.Insertable(student).ExecuteCommandAsync();
        }
        else
        {
            if (!string.IsNullOrEmpty(request.No)) existing.No = request.No;
            if (!string.IsNullOrEmpty(request.IdNumber)) existing.IdNumber = request.IdNumber;
            if (request.Gender.HasValue) existing.Gender = (EnumGender)request.Gender.Value;
            if (request.EthnicGroup.HasValue) existing.EthnicGroup = (EnumEthnicGroup)request.EthnicGroup.Value;
            if (!string.IsNullOrEmpty(request.NativePlace)) existing.NativePlace = request.NativePlace;
            if (request.Birthday.HasValue) existing.Birthday = request.Birthday.Value;
            if (request.Weight.HasValue) existing.Weight = request.Weight.Value;
            if (request.Height.HasValue) existing.Height = request.Height.Value;

            await _db.Updateable(existing).ExecuteCommandAsync();
        }

        return new SimpleResponse { Success = true, Message = "资料更新成功" };
    }

    /// <summary>
    /// 验证token并获取用户ID
    /// </summary>
    public int? ValidateTokenAndGetUserId(string token)
    {
        return _jwtService.GetUserIdFromToken(token);
    }

    private async Task<UserProfileDto> BuildUserProfileDtoAsync(User user)
    {
        var student = await _db.Queryable<Student>()
            .FirstAsync(s => s.Name == user.UserName);

        var profile = new UserProfileDto
        {
            UserId = user.Id,
            UserName = user.UserName
        };

        if (student != null)
        {
            profile.Name = student.Name;
            profile.No = student.No;
            profile.IdNumber = student.IdNumber;
            profile.Gender = (int)student.Gender;
            profile.EthnicGroup = (int)student.EthnicGroup;
            profile.NativePlace = student.NativePlace;
            profile.Birthday = student.Birthday;
            profile.Weight = student.Weight;
            profile.Height = student.Height;
            profile.HasProfile = true;
        }

        return profile;
    }

    // 旧方法保留以兼容现有代码
    public async Task<bool> RegisterAsync(CreateAccountRequest request)
    {
        var existing = await _db.Queryable<User>()
            .FirstAsync(u => u.UserName == request.UserName);
        if (existing != null) return false;

        var user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };
        var userId = await _db.Insertable(user).ExecuteReturnIdentityAsync();

        if (HasAnyProfileField(request))
        {
            var student = new Student
            {
                No = request.No ?? string.Empty,
                Name = request.Name ?? string.Empty,
                IdNumber = request.IdNumber ?? string.Empty,
                Gender = request.Gender ?? EnumGender.男,
                EthnicGroup = request.EthnicGroup ?? EnumEthnicGroup.汉族,
                NativePlace = request.NativePlace,
                Birthday = request.Birthday ?? DateTime.MinValue,
                Weight = request.Weight ?? 0,
                Height = request.Height ?? 0m
            };
            await _db.Insertable(student).ExecuteCommandAsync();
        }

        return true;
    }

    public async Task<LoginResult> LoginAsync(string userName, string password)
    {
        var user = await _db.Queryable<User>()
            .FirstAsync(u => u.UserName == userName);

        if (user == null)
        {
            return new LoginResult { Success = false, Message = "用户不存在" };
        }

        if (user.Password != password)
        {
            return new LoginResult { Success = false, Message = "密码错误" };
        }

        var profile = await BuildProfileFromUserAsync(user);
        return new LoginResult
        {
            Success = true,
            Message = "登录成功",
            Account = profile
        };
    }

    public async Task<AccountProfileDto?> GetAccountProfileAsync(string userName)
    {
        var user = await _db.Queryable<User>()
            .FirstAsync(u => u.UserName == userName);
        if (user == null) return null;
        return await BuildProfileFromUserAsync(user);
    }

    public async Task<AccountProfileDto?> GetAccountProfileByIdAsync(int userId)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null) return null;
        return await BuildProfileFromUserAsync(user);
    }

    public async Task<List<AccountListItem>> GetAllAccounts()
    {
        var users = await _db.Queryable<User>().ToListAsync();
        var students = await _db.Queryable<Student>().ToListAsync();

        var result = new List<AccountListItem>();
        foreach (var user in users)
        {
            var profile = students.FirstOrDefault(s => s.Name == user.UserName);
            result.Add(new AccountListItem
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = profile?.Name,
                No = profile?.No,
                HasProfile = profile != null,
                UserCreatedTime = DateTime.UtcNow
            });
        }
        return result;
    }

    // 实现接口要求的方法
    public async Task<List<AccountListItem>> GetAllAccountsAsync()
    {
        return await GetAllAccounts();
    }

    public async Task<bool> UpdateProfileAsync(int userId, StudentProfileDto profile)
    {
        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        if (user == null) return false;

        var existing = await _db.Queryable<Student>()
            .FirstAsync(s => s.Name == user.UserName);

        if (existing == null)
        {
            var student = new Student
            {
                No = profile.No ?? string.Empty,
                Name = user.UserName,
                IdNumber = profile.IdNumber ?? string.Empty,
                Gender = profile.Gender ?? EnumGender.男,
                EthnicGroup = profile.EthnicGroup ?? EnumEthnicGroup.汉族,
                NativePlace = profile.NativePlace,
                Birthday = profile.Birthday ?? DateTime.MinValue,
                Weight = profile.Weight ?? 0,
                Height = profile.Height ?? 0m
            };
            return await _db.Insertable(student).ExecuteCommandAsync() > 0;
        }
        else
        {
            existing.No = profile.No ?? existing.No;
            existing.IdNumber = profile.IdNumber ?? existing.IdNumber;
            if (profile.Gender.HasValue) existing.Gender = profile.Gender.Value;
            if (profile.EthnicGroup.HasValue) existing.EthnicGroup = profile.EthnicGroup.Value;
            existing.NativePlace = profile.NativePlace ?? existing.NativePlace;
            if (profile.Birthday.HasValue) existing.Birthday = profile.Birthday.Value;
            if (profile.Weight.HasValue) existing.Weight = profile.Weight.Value;
            if (profile.Height.HasValue) existing.Height = profile.Height.Value;

            return await _db.Updateable(existing).ExecuteCommandAsync() > 0;
        }
    }

    public async Task<bool> ChangePasswordAsync(string userName, string newPassword)
    {
        var result = await _db.Updateable<User>()
            .SetColumns(u => u.Password == newPassword)
            .Where(u => u.UserName == userName)
            .ExecuteCommandAsync();
        return result > 0;
    }

    private async Task<AccountProfileDto> BuildProfileFromUserAsync(User user)
    {
        var student = await _db.Queryable<Student>()
            .FirstAsync(s => s.Name == user.UserName);

        var profile = new AccountProfileDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Password = user.Password
        };

        if (student != null)
        {
            profile.No = student.No;
            profile.Name = student.Name;
            profile.IdNumber = student.IdNumber;
            profile.Gender = student.Gender;
            profile.EthnicGroup = student.EthnicGroup;
            profile.NativePlace = student.NativePlace;
            profile.Birthday = student.Birthday;
            profile.Weight = student.Weight;
            profile.Height = student.Height;
            profile.HasProfile = true;
        }

        return profile;
    }

    private bool HasAnyProfileField(CreateAccountRequest request)
    {
        return !string.IsNullOrEmpty(request.No)
            || !string.IsNullOrEmpty(request.Name)
            || !string.IsNullOrEmpty(request.IdNumber)
            || request.Gender.HasValue
            || request.EthnicGroup.HasValue
            || !string.IsNullOrEmpty(request.NativePlace)
            || request.Birthday.HasValue
            || request.Weight.HasValue
            || request.Height.HasValue;
    }
}
