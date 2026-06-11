using Chat.Application.Dtos;
using Chat.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chat.Server.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// 分页获取帖子列表
    /// GET /api/posts?pageIndex=1&pageSize=10&sortBy=time&tag=xxx
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResponse<PostDto>>> GetPosts([FromQuery] PostQueryRequest request)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        var posts = await _postService.GetPostsAsync(request, userId);
        return Ok(posts);
    }

    /// <summary>
    /// 获取指定用户的帖子列表（我的帖子/查看他人帖子）
    /// GET /api/posts/user/{userId}?pageIndex=1&pageSize=10&tag=xxx
    /// </summary>
    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResponse<PostDto>>> GetUserPosts(int userId, [FromQuery] PostQueryRequest request)
    {
        int? currentUserId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        var posts = await _postService.GetUserPostsAsync(userId, request, currentUserId);
        return Ok(posts);
    }

    /// <summary>
    /// 获取当前登录用户的帖子（我的帖子）
    /// GET /api/posts/my?pageIndex=1&pageSize=10
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<PaginatedResponse<PostDto>>> GetMyPosts([FromQuery] PostQueryRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var posts = await _postService.GetUserPostsAsync(userId, request, userId);
        return Ok(posts);
    }

    /// <summary>
    /// 创建帖子
    /// POST /api/posts
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var post = await _postService.CreatePostAsync(userId, request);
        if (post == null)
        {
            return BadRequest();
        }
        return Created($"/api/posts/{post.Id}", post);
    }

    /// <summary>
    /// 查看帖子详情
    /// GET /api/posts/{postId}
    /// </summary>
    [HttpGet("{postId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetPost(int postId)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        var post = await _postService.GetPostByIdAsync(postId, userId);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    /// <summary>
    /// 编辑帖子（管理员可编辑任意帖子）
    /// PUT /api/posts/{postId}
    /// </summary>
    [HttpPut("{postId}")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> UpdatePost(int postId, [FromBody] CreatePostRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);
        var result = await _postService.UpdatePostAsync(postId, userId, request, role);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 删除帖子（管理员可删除任意帖子）
    /// DELETE /api/posts/{postId}
    /// </summary>
    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> DeletePost(int postId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);
        var result = await _postService.DeletePostAsync(postId, userId, role);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 点赞帖子
    /// POST /api/posts/{postId}/like
    /// </summary>
    [HttpPost("{postId}/like")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> LikePost(int postId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _postService.ToggleLikeAsync(postId, userId);
        return Ok(result);
    }

    /// <summary>
    /// 取消点赞
    /// DELETE /api/posts/{postId}/like
    /// </summary>
    [HttpDelete("{postId}/like")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> UnlikePost(int postId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _postService.ToggleLikeAsync(postId, userId);
        return Ok(result);
    }

    /// <summary>
    /// 收藏帖子
    /// POST /api/posts/{postId}/favorite
    /// </summary>
    [HttpPost("{postId}/favorite")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> FavoritePost(int postId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _postService.ToggleFavoriteAsync(postId, userId);
        return Ok(result);
    }

    /// <summary>
    /// 取消收藏
    /// DELETE /api/posts/{postId}/favorite
    /// </summary>
    [HttpDelete("{postId}/favorite")]
    [Authorize]
    public async Task<ActionResult<SimpleResponse>> UnfavoritePost(int postId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _postService.ToggleFavoriteAsync(postId, userId);
        return Ok(result);
    }
}
