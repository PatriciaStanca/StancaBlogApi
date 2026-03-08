
namespace StancaBlogApi.Core.Interfaces;

public interface IBlogService
{
    Task<ServiceResult<PagedResult<BlogPostDto>>> GetAllAsync(int? categoryId, string? search, int page, int pageSize);
    Task<ServiceResult<BlogPostDto>> GetByIdAsync(int id);
    Task<ServiceResult<BlogPostDto>> CreateAsync(int userId, BlogPostCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, int userId, BlogPostUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id, int userId);
}
