using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Interfaces;

public interface ICommentRepository
{
    IQueryable<Comment> Query();
    Task<Comment?> GetByIdAsync(int id);
    Task AddAsync(Comment comment);
    void Remove(Comment comment);
    void RemoveRange(IEnumerable<Comment> comments);
    Task<List<Comment>> GetByUserIdAsync(int userId);
    Task SaveChangesAsync();
}
