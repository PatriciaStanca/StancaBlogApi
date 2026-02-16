using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StancaBlogApi.Core.Interfaces;
using StancaBlogApi.DTOs;
using StancaBlogApi.Infrastructure.Security;

namespace StancaBlogApi.Controllers;

[Route("api/comments")]
public class CommentsController : ApiControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("/api/blogposts/{postId}/comments")]
    public async Task<IActionResult> GetByPost(int postId)
    {
        var result = await _commentService.GetByPostAsync(postId);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPost("/api/blogposts/{postId}/comments")]
    public async Task<IActionResult> Create(int postId, CommentCreateDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _commentService.CreateAsync(postId, userId, User.GetUserNameOrEmpty(), dto);

        if (result.StatusCode == StatusCodes.Status201Created && result.Data is not null)
            return CreatedAtAction(nameof(GetByPost), new { postId }, result.Data);

        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CommentUpdateDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _commentService.UpdateAsync(id, userId, dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _commentService.DeleteAsync(id, userId);
        return ToActionResult(result);
    }
}
