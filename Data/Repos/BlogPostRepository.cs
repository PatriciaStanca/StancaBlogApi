using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Repos;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly AppDbContext _context;

    public BlogPostRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<BlogPost> Query() => _context.BlogPosts.AsQueryable();

    public Task<BlogPost?> GetByIdAsync(int id) => _context.BlogPosts.FindAsync(id).AsTask();

    public async Task AddAsync(BlogPost post) => await _context.BlogPosts.AddAsync(post);

    public void Remove(BlogPost post) => _context.BlogPosts.Remove(post);

    public void RemoveRange(IEnumerable<BlogPost> posts) => _context.BlogPosts.RemoveRange(posts);

    public Task<List<BlogPost>> GetByUserIdAsync(int userId) =>
        _context.BlogPosts.Where(p => p.UserId == userId).ToListAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
