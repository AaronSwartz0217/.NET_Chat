using Chat.Application.Dtos;
using Chat.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chat.Server.Controllers;

[ApiController]
[Route("api/channels")]
[Authorize]
public class ChannelController : ControllerBase
{
    private readonly IChannelService _channelService;

    public ChannelController(IChannelService channelService)
    {
        _channelService = channelService;
    }

    /// <summary>
    /// 获取当前用户的会话列表
    /// GET /api/channels
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ChannelDto>>> GetChannels()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var channels = await _channelService.GetChannelsAsync(userId);
        return Ok(channels);
    }

    /// <summary>
    /// 创建新会话
    /// POST /api/channels
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ChannelDto>> CreateChannel([FromBody] CreateChannelRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var channel = await _channelService.CreateChannelAsync(userId, request);
        if (channel == null)
        {
            return BadRequest();
        }
        return Created($"/api/channels/{channel.Id}", channel);
    }

    /// <summary>
    /// 获取会话历史消息
    /// GET /api/channels/{channelId}/messages?pageIndex=1&pageSize=50
    /// </summary>
    [HttpGet("{channelId}/messages")]
    public async Task<ActionResult<PaginatedResponse<MessageDto>>> GetMessages(int channelId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 50)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var messages = await _channelService.GetMessagesAsync(channelId, userId, pageIndex, pageSize);
        return Ok(messages);
    }

    /// <summary>
    /// 发送消息
    /// POST /api/channels/{channelId}/messages
    /// </summary>
    [HttpPost("{channelId}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(int channelId, [FromBody] SendMessageRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var message = await _channelService.SendMessageAsync(channelId, userId, request);
        if (message == null)
        {
            return BadRequest();
        }
        return Ok(message);
    }

    /// <summary>
    /// 撤回消息
    /// POST /api/messages/{messageId}/recall
    /// </summary>
    [HttpPost("/api/messages/{messageId}/recall")]
    public async Task<ActionResult<SimpleResponse>> RecallMessage(int messageId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _channelService.RecallMessageAsync(messageId, userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 标记会话为已读
    /// POST /api/channels/{channelId}/read
    /// </summary>
    [HttpPost("{channelId}/read")]
    public async Task<ActionResult<SimpleResponse>> MarkAsRead(int channelId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _channelService.MarkAsReadAsync(channelId, userId);
        return Ok(result);
    }
}
