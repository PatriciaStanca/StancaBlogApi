using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Repos;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Comment> Query() => _context.Comments.AsQueryable();

    public Task<Comment?> GetByIdAsync(int id) => _context.Comments.FindAsync(id).AsTask();

    public async Task AddAsync(Comment comment) => await _context.Comments.AddAsync(comment);

    public void Remove(Comment comment) => _context.Comments.Remove(comment);

    public void RemoveRange(IEnumerable<Comment> comments) => _context.Comments.RemoveRange(comments);

    public Task<List<Comment>> GetByUserIdAsync(int userId) =>
        _context.Comments.Where(c => c.UserId == userId).ToListAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
