using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StancaBlogApi.Core.Interfaces;
using StancaBlogApi.DTOs;
using StancaBlogApi.Infrastructure.Security;

namespace StancaBlogApi.Controllers;

[Route("api/[controller]")]
public class BlogPostsController : ApiControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostsController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var result = await _blogPostService.GetAllAsync(categoryId, search, page, pageSize);
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _blogPostService.GetByIdAsync(id);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(BlogPostCreateDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _blogPostService.CreateAsync(userId, dto);

        if (result.StatusCode == StatusCodes.Status201Created && result.Data is not null)
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);

        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BlogPostUpdateDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _blogPostService.UpdateAsync(id, userId, dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _blogPostService.DeleteAsync(id, userId);
        return ToActionResult(result);
    }
}
