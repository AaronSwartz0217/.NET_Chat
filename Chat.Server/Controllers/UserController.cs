using Chat.Application.Dtos;
using Chat.Application.Services;
using Chat.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chat.Server.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserServiceV2 _userService;

    public UserController(IUserServiceV2 userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 获取所有用户列表（JWT认证）
    /// GET /api/users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// 获取当前登录用户的完整资料
    /// GET /api/users/me
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var profile = await _userService.GetCurrentUserProfileAsync(userId);
        if (profile == null)
        {
            return NotFound();
        }
        return Ok(profile);
    }

    /// <summary>
    /// 更新个人资料
    /// PUT /api/users/me
    /// </summary>
    [HttpPut("me")]
    public async Task<ActionResult<SimpleResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _userService.UpdateProfileAsync(userId, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 查看他人公开信息
    /// GET /api/users/{userId}/profile
    /// </summary>
    [HttpGet("{userId}/profile")]
    public async Task<ActionResult<UserProfileDto>> GetPublicProfile(int userId)
    {
        var profile = await _userService.GetPublicProfileAsync(userId);
        if (profile == null)
        {
            return NotFound();
        }
        return Ok(profile);
    }
}
