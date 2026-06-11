using Chat.Application.Dtos;
using Chat.Application.Services;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Chat.Server.Controllers;

/// <summary>
/// 账号档案控制器 - RESTful + 安全版本
/// </summary>
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// 注册账户（公开接口）
    /// POST /api/account/register
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _accountService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return StatusCode(201, result);
    }

    /// <summary>
    /// 登录（公开接口）
    /// POST /api/account/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _accountService.LoginAsync(request);
        if (!result.Success)
        {
            return Unauthorized(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 获取当前登录用户自己的资料（需要JWT认证）
    /// GET /api/account/profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var profile = await _accountService.GetCurrentUserProfileAsync(token);
        
        if (profile == null)
        {
            return Unauthorized(new { Success = false, Message = "无效的token" });
        }
        return Ok(profile);
    }

    /// <summary>
    /// 获取指定用户资料（管理员权限）
    /// GET /api/account/profile/{userId}
    /// </summary>
    [HttpGet("profile/{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserProfileDto>> GetProfileById(int userId)
    {
        var profile = await _accountService.GetProfileByIdAsync(userId);
        if (profile == null)
        {
            return NotFound(new { Success = false, Message = "用户不存在" });
        }
        return Ok(profile);
    }

    /// <summary>
    /// 获取所有账户（管理员权限）
    /// GET /api/account/all-accounts
    /// </summary>
    [HttpGet("all-accounts")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<UserProfileDto>>> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsSecureAsync();
        return Ok(accounts);
    }

    /// <summary>
    /// 修改当前用户自己的资料（需要JWT认证）
    /// PUT /api/account/profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var userId = _accountService.ValidateTokenAndGetUserId(token);
        
        if (!userId.HasValue)
        {
            return Unauthorized(new { Success = false, Message = "无效的token" });
        }

        var result = await _accountService.UpdateProfileAsync(userId.Value, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 修改密码（需验证旧密码）
    /// POST /api/account/change-password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var userId = _accountService.ValidateTokenAndGetUserId(token);
        
        if (!userId.HasValue)
        {
            return Unauthorized(new { Success = false, Message = "无效的token" });
        }

        var result = await _accountService.ChangePasswordAsync(userId.Value, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
