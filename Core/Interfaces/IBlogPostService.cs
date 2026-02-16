using StancaBlogApi.Core.Common;
using StancaBlogApi.DTOs;

namespace StancaBlogApi.Core.Interfaces;

public interface IBlogPostService
{
    Task<ServiceResult<PagedResult<BlogPostReadDto>>> GetAllAsync(int? categoryId, string? search, int page, int pageSize);
    Task<ServiceResult<BlogPostReadDto>> GetByIdAsync(int id);
    Task<ServiceResult<BlogPostReadDto>> CreateAsync(int userId, BlogPostCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, int userId, BlogPostUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id, int userId);
}
