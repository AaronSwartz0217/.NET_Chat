using Chat.Application.Dtos;
using Chat.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chat.Server.Controllers;

[ApiController]
[Route("api")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary>
    /// 获取帖子评论列表
    /// GET /api/posts/{postId}/comments?pageIndex=1&pageSize=20
    /// </summary>
    [HttpGet("posts/{postId}/comments")]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResponse<CommentDto>>> GetComments(int postId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var comments = await _commentService.GetCommentsAsync(postId, pageIndex, pageSize);
        return Ok(comments);
    }

    /// <summary>
    /// 发表评论
    /// POST /api/posts/{postId}/comments
    /// </summary>
    [HttpPost("posts/{postId}/comments")]
    [Authorize]
    public async Task<ActionResult<CommentDto>> CreateComment(int postId, [FromBody] CreateCommentRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var comment = await _commentService.CreateCommentAsync(postId, userId, request);
        if (comment == null)
        {
            return BadRequest();
        }
        return Created($"/api/comments/{comment.Id}", comment);
    }

    /// <summary>
    /// 删除评论
    /// DELETE /api/comments/{commentId}
    /// </summary>
    [HttpDelete("comments/{commentId}")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> DeleteComment(int commentId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _commentService.DeleteCommentAsync(commentId, userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
