using StancaBlogApi.Core.Common;
using StancaBlogApi.Core.Interfaces;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.Models;

namespace StancaBlogApi.Core.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ServiceResult<List<Category>>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return ServiceResult<List<Category>>.Ok(categories);
    }
}
