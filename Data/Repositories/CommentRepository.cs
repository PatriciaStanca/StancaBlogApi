
namespace StancaBlogApi.Data.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly BlogDbContext _context;

    public CommentRepository(BlogDbContext context)
    {
        _context = context;
    }

    public IQueryable<Comment> Query() => _context.Comments.AsQueryable();

    public Task<Comment?> GetByIdAsync(int id) => _context.Comments.FindAsync(id).AsTask();

    public Task AddAsync(Comment comment) => _context.Comments.AddAsync(comment).AsTask();

    public void Remove(Comment comment) => _context.Comments.Remove(comment);

    public void RemoveRange(IEnumerable<Comment> comments) => _context.Comments.RemoveRange(comments);

    public Task<List<Comment>> GetByUserIdAsync(int userId) =>
        _context.Comments.Where(c => c.UserId == userId).ToListAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
