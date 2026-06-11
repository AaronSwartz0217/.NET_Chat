using Chat.Application.Dtos;
using Chat.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Application.Services;

/// <summary>
/// 学生档案DTO（用于更新档案）
/// </summary>
public class StudentProfileDto
{
    public string? No { get; set; }
    public string? Name { get; set; }
    public string? IdNumber { get; set; }
    public EnumGender? Gender { get; set; }
    public EnumEthnicGroup? EthnicGroup { get; set; }
    public string? NativePlace { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Weight { get; set; }
    public decimal? Height { get; set; }
}

/// <summary>
/// 账号档案服务 - 业务层面合并User和Student
/// 底层仍使用两个独立的数据库表，通过业务代码组合
/// </summary>
public interface IAccountService
{
    // ===== 旧版本方法（兼容）=====
    /// <summary>注册账号（可同时完善学生档案）</summary>
    Task<bool> RegisterAsync(CreateAccountRequest request);

    /// <summary>登录验证并返回账号+档案信息</summary>
    Task<LoginResult> LoginAsync(string userName, string password);

    /// <summary>根据用户名获取账号档案</summary>
    Task<AccountProfileDto?> GetAccountProfileAsync(string userName);

    /// <summary>根据用户ID获取账号档案</summary>
    Task<AccountProfileDto?> GetAccountProfileByIdAsync(int userId);

    /// <summary>获取所有账号列表（包含档案状态）</summary>
    Task<List<AccountListItem>> GetAllAccountsAsync();

    /// <summary>完善/更新学生档案</summary>
    Task<bool> UpdateProfileAsync(int userId, StudentProfileDto profile);

    /// <summary>修改密码</summary>
    Task<bool> ChangePasswordAsync(string userName, string newPassword);

    // ===== 安全版本方法（新）=====
    /// <summary>注册账号（POST方式，返回JWT）</summary>
    Task<LoginResponse> RegisterAsync(RegisterRequest request);

    /// <summary>登录（POST方式，返回JWT）</summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>获取当前登录用户资料（从token解析）</summary>
    Task<UserProfileDto?> GetCurrentUserProfileAsync(string token);

    /// <summary>获取所有账号（安全版本）</summary>
    Task<List<UserProfileDto>> GetAllAccountsSecureAsync();

    /// <summary>修改密码（需旧密码验证）</summary>
    Task<SimpleResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request);

    /// <summary>更新用户资料（安全版本）</summary>
    Task<SimpleResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request);

    /// <summary>验证token并获取用户ID</summary>
    int? ValidateTokenAndGetUserId(string token);
}
