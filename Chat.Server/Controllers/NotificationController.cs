using Chat.Application.Dtos;
using Chat.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chat.Server.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// 获取通知列表
    /// GET /api/notifications
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var notifications = await _notificationService.GetNotificationsAsync(userId);
        return Ok(notifications);
    }

    /// <summary>
    /// 单条标记已读
    /// PUT /api/notifications/{id}/read
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult<SimpleResponse>> MarkAsRead(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _notificationService.MarkAsReadAsync(userId, id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 全部标记已读
    /// PUT /api/notifications/read-all
    /// </summary>
    [HttpPut("read-all")]
    public async Task<ActionResult<SimpleResponse>> MarkAllAsRead()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(result);
    }
}
