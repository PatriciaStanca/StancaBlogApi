namespace StancaBlogApi.Core.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<List<CategoryDto>>> GetAllAsync();
}
