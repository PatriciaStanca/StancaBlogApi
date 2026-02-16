using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Interfaces;

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(int id);
    Task<List<Category>> GetAllAsync();
}
