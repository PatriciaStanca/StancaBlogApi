using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Data;
using StancaBlogApi.Models;

namespace StancaBlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        return await _context.Categories.ToListAsync();
    }
}
