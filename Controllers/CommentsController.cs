using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Data;
using StancaBlogApi.DTOs;
using StancaBlogApi.Models;
using System.Security.Claims;

namespace StancaBlogApi.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    // ===========================
    // GET: api/blogposts/{postId}/comments
    // ===========================
    [HttpGet("/api/blogposts/{postId}/comments")]
    public async Task<IActionResult> GetByPost(int postId)
    {
        var comments = await _context.Comments
            .Where(c => c.BlogPostId == postId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Content,
                c.CreatedAt,
                AuthorName = c.User.UserName
            })
            .ToListAsync();

        return Ok(comments);
    }

    // ===========================
    // POST: api/blogposts/{postId}/comments
    // ===========================
    [Authorize]
    [HttpPost("/api/blogposts/{postId}/comments")]
    public async Task<IActionResult> Create(int postId, CommentCreateDto dto)
    {
        var post = await _context.BlogPosts.FindAsync(postId);
        if (post == null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (post.UserId == userId)
            return BadRequest("You cannot comment on your own blog post.");

        var comment = new Comment
        {
            Content = dto.Content,
            BlogPostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // ===========================
    // PUT: api/comments/{id}
    // ===========================
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CommentUpdateDto dto)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (comment.UserId != userId)
            return Forbid();

        comment.Content = dto.Content;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ===========================
    // DELETE: api/comments/{id}
    // ===========================
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (comment.UserId != userId)
            return Forbid();

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
