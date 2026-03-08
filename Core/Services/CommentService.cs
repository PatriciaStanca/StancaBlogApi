
namespace StancaBlogApi.Core.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepository, IBlogRepository blogRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _blogRepository = blogRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<List<CommentDto>>> GetByPostAsync(int postId)
    {
        var comments = await _commentRepository.Query()
            .Where(c => c.BlogPostId == postId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return ServiceResult<List<CommentDto>>.Ok(_mapper.Map<List<CommentDto>>(comments));
    }

    public async Task<ServiceResult<CommentDto>> CreateAsync(int postId, int userId, string userName, CommentCreateDto dto)
    {
        var post = await _blogRepository.GetByIdAsync(postId);
        if (post is null)
            return ServiceResult<CommentDto>.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        if (post.UserId == userId)
            return ServiceResult<CommentDto>.Fail(StatusCodes.Status400BadRequest, "You cannot comment on your own blog post.");

        var comment = new Comment
        {
            Content = dto.Content,
            BlogPostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var created = new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UserId = comment.UserId,
            UserName = userName
        };

        return ServiceResult<CommentDto>.Created(created);
    }

    public async Task<ServiceResult> UpdateAsync(int id, int userId, CommentUpdateDto dto)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment is null)
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Comment not found.");

        if (comment.UserId != userId)
            return ServiceResult.Fail(StatusCodes.Status403Forbidden, "Forbidden.");

        comment.Content = dto.Content;
        await _commentRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    public async Task<ServiceResult> DeleteAsync(int id, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment is null)
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Comment not found.");

        if (comment.UserId != userId)
            return ServiceResult.Fail(StatusCodes.Status403Forbidden, "Forbidden.");

        _commentRepository.Remove(comment);
        await _commentRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }
}
