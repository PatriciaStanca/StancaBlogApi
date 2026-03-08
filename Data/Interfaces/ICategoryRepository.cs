namespace StancaBlogApi.Data.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
}
