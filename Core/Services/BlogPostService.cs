using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Application.Mappings;
using StancaBlogApi.Core.Common;
using StancaBlogApi.Core.Interfaces;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.DTOs;
using StancaBlogApi.Models;

namespace StancaBlogApi.Core.Services;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ICategoryRepository _categoryRepository;

    public BlogPostService(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
    {
        _blogPostRepository = blogPostRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ServiceResult<PagedResult<BlogPostReadDto>>> GetAllAsync(int? categoryId, string? search, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 5;

        var query = _blogPostRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.User)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

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
            .ToListAsync();

        var result = new PagedResult<BlogPostReadDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = posts.Select(p => p.ToReadDto()).ToList()
        };

        return ServiceResult<PagedResult<BlogPostReadDto>>.Ok(result);
    }

    public async Task<ServiceResult<BlogPostReadDto>> GetByIdAsync(int id)
    {
        var post = await _blogPostRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null)
            return ServiceResult<BlogPostReadDto>.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        return ServiceResult<BlogPostReadDto>.Ok(post.ToReadDto());
    }

    public async Task<ServiceResult<BlogPostReadDto>> CreateAsync(int userId, BlogPostCreateDto dto)
    {
        if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
            return ServiceResult<BlogPostReadDto>.Fail(StatusCodes.Status400BadRequest, $"Category with id {dto.CategoryId} does not exist.");

        var post = new BlogPost
        {
            Title = dto.Title,
            Content = dto.Content,
            CategoryId = dto.CategoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _blogPostRepository.AddAsync(post);
        await _blogPostRepository.SaveChangesAsync();

        var createdPost = await _blogPostRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.User)
            .FirstAsync(p => p.Id == post.Id);

        return ServiceResult<BlogPostReadDto>.Created(createdPost.ToReadDto());
    }

    public async Task<ServiceResult> UpdateAsync(int id, int userId, BlogPostUpdateDto dto)
    {
        var post = await _blogPostRepository.GetByIdAsync(id);
        if (post is null)
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        if (post.UserId != userId)
            return ServiceResult.Fail(StatusCodes.Status403Forbidden, "Forbidden.");

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, $"Category with id {dto.CategoryId} does not exist.");

        post.Title = dto.Title;
        post.Content = dto.Content;
        post.CategoryId = dto.CategoryId;

        await _blogPostRepository.SaveChangesAsync();
        return ServiceResult.NoContent();
    }

    public async Task<ServiceResult> DeleteAsync(int id, int userId)
    {
        var post = await _blogPostRepository.GetByIdAsync(id);
        if (post is null)
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        if (post.UserId != userId)
            return ServiceResult.Fail(StatusCodes.Status403Forbidden, "Forbidden.");

        _blogPostRepository.Remove(post);
        await _blogPostRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }
}
