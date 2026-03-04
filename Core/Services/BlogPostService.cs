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
        var paging = NormalizePaging(page, pageSize);
        var query = ApplyFilters(BasePostQuery(), categoryId, search);

        var totalItems = await query.CountAsync();
        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return ServiceResult<PagedResult<BlogPostReadDto>>.Ok(
            ToPagedResult(posts, totalItems, paging.Page, paging.PageSize));
    }

    public async Task<ServiceResult<BlogPostReadDto>> GetByIdAsync(int id)
    {
        var post = await QueryForDetailsById(id);

        if (post is null)
            return ServiceResult<BlogPostReadDto>.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        return ServiceResult<BlogPostReadDto>.Ok(post.ToReadDto());
    }

    public async Task<ServiceResult<BlogPostReadDto>> CreateAsync(int userId, BlogPostCreateDto dto)
    {
        var categoryValidation = await ValidateCategoryAsync(dto.CategoryId);
        if (categoryValidation is not null)
            return categoryValidation;

        var post = CreatePostEntity(userId, dto.Title, dto.Content, dto.CategoryId);

        await _blogPostRepository.AddAsync(post);
        await _blogPostRepository.SaveChangesAsync();

        var createdPost = await QueryForListById(post.Id);

        return ServiceResult<BlogPostReadDto>.Created(createdPost.ToReadDto());
    }

    public async Task<ServiceResult> UpdateAsync(int id, int userId, BlogPostUpdateDto dto)
    {
        var ownershipResult = await GetOwnedPostAsync(id, userId);
        if (ownershipResult.Error is not null)
            return ownershipResult.Error;

        var categoryValidation = await ValidateCategoryForWriteAsync(dto.CategoryId);
        if (categoryValidation is not null)
            return categoryValidation;

        ApplyPostUpdates(ownershipResult.Post!, dto);

        await _blogPostRepository.SaveChangesAsync();
        return ServiceResult.NoContent();
    }

    public async Task<ServiceResult> DeleteAsync(int id, int userId)
    {
        var ownershipResult = await GetOwnedPostAsync(id, userId);
        if (ownershipResult.Error is not null)
            return ownershipResult.Error;

        _blogPostRepository.Remove(ownershipResult.Post!);
        await _blogPostRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    private IQueryable<BlogPost> BasePostQuery()
    {
        return _blogPostRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.User);
    }

    private static IQueryable<BlogPost> ApplyFilters(IQueryable<BlogPost> query, int? categoryId, string? search)
    {
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Title, pattern));
        }

        return query;
    }

    private static (int Page, int PageSize) NormalizePaging(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 || pageSize > 50 ? 5 : pageSize;
        return (normalizedPage, normalizedPageSize);
    }

    private static PagedResult<BlogPostReadDto> ToPagedResult(
        List<BlogPost> posts,
        int totalItems,
        int page,
        int pageSize)
    {
        return new PagedResult<BlogPostReadDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = posts.Select(p => p.ToReadDto()).ToList()
        };
    }

    private Task<BlogPost?> QueryForDetailsById(int id)
    {
        return _blogPostRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    private Task<BlogPost> QueryForListById(int id)
    {
        return BasePostQuery().FirstAsync(p => p.Id == id);
    }

    private async Task<ServiceResult<BlogPostReadDto>?> ValidateCategoryAsync(int categoryId)
    {
        if (await _categoryRepository.ExistsAsync(categoryId))
            return null;

        return ServiceResult<BlogPostReadDto>.Fail(
            StatusCodes.Status400BadRequest,
            $"Category with id {categoryId} does not exist.");
    }

    private async Task<ServiceResult?> ValidateCategoryForWriteAsync(int categoryId)
    {
        if (await _categoryRepository.ExistsAsync(categoryId))
            return null;

        return ServiceResult.Fail(
            StatusCodes.Status400BadRequest,
            $"Category with id {categoryId} does not exist.");
    }

    private async Task<(BlogPost? Post, ServiceResult? Error)> GetOwnedPostAsync(int id, int userId)
    {
        var post = await _blogPostRepository.GetByIdAsync(id);
        if (post is null)
            return (null, ServiceResult.Fail(StatusCodes.Status404NotFound, "Blog post not found."));

        if (post.UserId != userId)
            return (null, ServiceResult.Fail(StatusCodes.Status403Forbidden, "Forbidden."));

        return (post, null);
    }

    private static BlogPost CreatePostEntity(int userId, string title, string content, int categoryId)
    {
        return new BlogPost
        {
            Title = title,
            Content = content,
            CategoryId = categoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static void ApplyPostUpdates(BlogPost post, BlogPostUpdateDto dto)
    {
        post.Title = dto.Title;
        post.Content = dto.Content;
        post.CategoryId = dto.CategoryId;
    }
}
