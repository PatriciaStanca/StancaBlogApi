
namespace StancaBlogApi.Data.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly BlogDbContext _context;

    public BlogRepository(BlogDbContext context)
    {
        _context = context;
    }

    public IQueryable<BlogPost> Query() => _context.BlogPosts.AsQueryable();

    public Task<BlogPost?> GetByIdAsync(int id) => _context.BlogPosts.FindAsync(id).AsTask();

    public Task AddAsync(BlogPost post) => _context.BlogPosts.AddAsync(post).AsTask();

    public void Remove(BlogPost post) => _context.BlogPosts.Remove(post);

    public void RemoveRange(IEnumerable<BlogPost> posts) => _context.BlogPosts.RemoveRange(posts);

    public Task<List<BlogPost>> GetByUserIdAsync(int userId) =>
        _context.BlogPosts.Where(p => p.UserId == userId).ToListAsync();

    public Task<bool> CategoryExistsAsync(int categoryId) =>
        _context.Categories.AnyAsync(c => c.Id == categoryId);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
