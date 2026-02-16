using StancaBlogApi.Core.Common;
using StancaBlogApi.Models;

namespace StancaBlogApi.Core.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<List<Category>>> GetAllAsync();
}
