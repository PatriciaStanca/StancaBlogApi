using Microsoft.AspNetCore.Mvc;
using StancaBlogApi.Core.Interfaces;

namespace StancaBlogApi.Controllers;

[Route("api/[controller]")]
public class CategoriesController : ApiControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return ToActionResult(result);
    }
}
