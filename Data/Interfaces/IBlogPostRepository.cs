using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Interfaces;

public interface IBlogPostRepository
{
    IQueryable<BlogPost> Query();
    Task<BlogPost?> GetByIdAsync(int id);
    Task AddAsync(BlogPost post);
    void Remove(BlogPost post);
    void RemoveRange(IEnumerable<BlogPost> posts);
    Task<List<BlogPost>> GetByUserIdAsync(int userId);
    Task SaveChangesAsync();
}
