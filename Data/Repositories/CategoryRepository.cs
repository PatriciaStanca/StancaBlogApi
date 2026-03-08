namespace StancaBlogApi.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly BlogDbContext _context;

    public CategoryRepository(BlogDbContext context)
    {
        _context = context;
    }

    public Task<List<Category>> GetAllAsync() =>
        _context.Categories.OrderBy(c => c.Id).ToListAsync();
}
