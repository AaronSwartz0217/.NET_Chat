using Chat.Application.Dtos;
using Chat.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Chat.Server.Controllers;

[ApiController]
[Route("api/search")]
[AllowAnonymous]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// 全局搜索
    /// GET /api/search?q=keyword&type=posts&pageIndex=1&pageSize=10
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SearchResultDto>> Search([FromQuery] SearchRequest request)
    {
        var result = await _searchService.SearchAsync(request);
        return Ok(result);
    }
}
