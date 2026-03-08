
namespace StancaBlogApi.Data.Interfaces;

public interface IBlogRepository
{
    IQueryable<BlogPost> Query();
    Task<BlogPost?> GetByIdAsync(int id);
    Task AddAsync(BlogPost post);
    void Remove(BlogPost post);
    void RemoveRange(IEnumerable<BlogPost> posts);
    Task<List<BlogPost>> GetByUserIdAsync(int userId);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task SaveChangesAsync();
}
