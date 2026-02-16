using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Repos;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(int id) => _context.Categories.AnyAsync(c => c.Id == id);

    public Task<List<Category>> GetAllAsync() => _context.Categories.ToListAsync();
}
