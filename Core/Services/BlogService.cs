
namespace StancaBlogApi.Core.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public BlogService(IBlogRepository blogRepository, IUserRepository userRepository, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<BlogPostDto>>> GetAllAsync(int? categoryId, string? search, int page, int pageSize)
    {
        var paging = NormalizePaging(page, pageSize);
        var query = ApplyFilters(BasePostQuery(), categoryId, search);

        var totalItems = await query.CountAsync();
        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        var mappedPosts = _mapper.Map<List<BlogPostDto>>(posts);

        return ServiceResult<PagedResult<BlogPostDto>>.Ok(new PagedResult<BlogPostDto>
        {
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)paging.PageSize),
            Items = mappedPosts
        });
    }

    public async Task<ServiceResult<BlogPostDto>> GetByIdAsync(int id)
    {
        var post = await _blogRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null)
            return ServiceResult<BlogPostDto>.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        return ServiceResult<BlogPostDto>.Ok(_mapper.Map<BlogPostDto>(post));
    }

    public async Task<ServiceResult<BlogPostDto>> CreateAsync(int userId, BlogPostCreateDto dto)
    {
        if (await _userRepository.GetByIdAsync(userId) is null)
        {
            return ServiceResult<BlogPostDto>.Fail(
                StatusCodes.Status401Unauthorized,
                "Unauthorized.");
        }

        if (!await _blogRepository.CategoryExistsAsync(dto.CategoryId))
        {
            return ServiceResult<BlogPostDto>.Fail(
                StatusCodes.Status400BadRequest,
                $"Category with id {dto.CategoryId} does not exist.");
        }

        var post = new BlogPost
        {
            Title = dto.Title,
            Content = dto.Content,
            CategoryId = dto.CategoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _blogRepository.AddAsync(post);
        await _blogRepository.SaveChangesAsync();

        var createdPost = await BasePostQuery().FirstAsync(p => p.Id == post.Id);
        return ServiceResult<BlogPostDto>.Created(_mapper.Map<BlogPostDto>(createdPost));
    }

    public async Task<ServiceResult> UpdateAsync(int id, int userId, BlogPostUpdateDto dto)
    {
        var post = await _blogRepository.GetByIdAsync(id);
        if (post is null)
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        if (post.UserId != userId)
            return ServiceResult.Fail(StatusCodes.Status403Forbidden, "Forbidden.");

        if (!await _blogRepository.CategoryExistsAsync(dto.CategoryId))
        {
            return ServiceResult.Fail(
                StatusCodes.Status400BadRequest,
                $"Category with id {dto.CategoryId} does not exist.");
        }

        post.Title = dto.Title;
        post.Content = dto.Content;
        post.CategoryId = dto.CategoryId;

        await _blogRepository.SaveChangesAsync();
        return ServiceResult.NoContent();
    }

    public async Task<ServiceResult> DeleteAsync(int id, int userId)
    {
        var post = await _blogRepository.GetByIdAsync(id);
        if (post is null)
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        if (post.UserId != userId)
            return ServiceResult.Fail(StatusCodes.Status403Forbidden, "Forbidden.");

        _blogRepository.Remove(post);
        await _blogRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    private IQueryable<BlogPost> BasePostQuery()
    {
        return _blogRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.User);
    }

    private static IQueryable<BlogPost> ApplyFilters(IQueryable<BlogPost> query, int? categoryId, string? search)
    {
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Title, pattern));
        }

        return query;
    }

    private static (int Page, int PageSize) NormalizePaging(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 || pageSize > 50 ? 5 : pageSize;
        return (normalizedPage, normalizedPageSize);
    }
}
