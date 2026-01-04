using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Data;
using StancaBlogApi.DTOs;
using StancaBlogApi.Models;
using System.Security.Claims;

namespace StancaBlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogPostsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BlogPostsController(AppDbContext context)
    {
        _context = context;
    }

    // ======================================================
    // GET: api/BlogPosts
    // Public – list blog posts with pagination + category filter
    // ======================================================
    [HttpGet]
    public async Task<ActionResult<PagedResult<BlogPostReadDto>>> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 5;

        var query = _context.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.User)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Title, pattern));
        }

        var totalItems = await query.CountAsync();

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new BlogPostReadDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                UserId = p.UserId,
                UserName = p.User!.UserName
            })
            .ToListAsync();

        var result = new PagedResult<BlogPostReadDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = posts
        };

        return Ok(result);
    }

    // ======================================================
    // GET: api/BlogPosts/{id}
    // Public – single post
    // ======================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<BlogPostReadDto>> GetById(int id)
    {
        var post = await _context.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
                 .Where(p => p.Id == id)
            .Select(p => new BlogPostReadDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                UserId = p.UserId,
                UserName = p.User!.UserName,

                Comments = p.Comments
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentReadDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User!.UserName
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (post == null)
            return NotFound();

        return Ok(post);
    }

    // ======================================================
    // POST: api/BlogPosts
    // Auth required – create post
    // ======================================================
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create(BlogPostCreateDto dto)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
            return BadRequest($"Category with id {dto.CategoryId} does not exist.");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var post = new BlogPost
        {
            Title = dto.Title,
            Content = dto.Content,
            CategoryId = dto.CategoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogPosts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, null);
    }

    // ======================================================
    // PUT: api/BlogPosts/{id}
    // Auth required – only owner
    // ======================================================
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BlogPostUpdateDto dto)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null) return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (post.UserId != userId) return Forbid();

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
            return BadRequest($"Category with id {dto.CategoryId} does not exist.");

        post.Title = dto.Title;
        post.Content = dto.Content;
        post.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ======================================================
    // DELETE: api/BlogPosts/{id}
    // Auth required – only owner
    // ======================================================
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null) return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (post.UserId != userId) return Forbid();

        _context.BlogPosts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
