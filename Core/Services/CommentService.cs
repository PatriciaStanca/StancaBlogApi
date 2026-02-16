using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Core.Common;
using StancaBlogApi.Core.Interfaces;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.DTOs;
using StancaBlogApi.Models;

namespace StancaBlogApi.Core.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IBlogPostRepository _blogPostRepository;

    public CommentService(ICommentRepository commentRepository, IBlogPostRepository blogPostRepository)
    {
        _commentRepository = commentRepository;
        _blogPostRepository = blogPostRepository;
    }

    public async Task<ServiceResult<List<CommentReadDto>>> GetByPostAsync(int postId)
    {
        var comments = await _commentRepository.Query()
            .Where(c => c.BlogPostId == postId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentReadDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                UserName = c.User.UserName
            })
            .ToListAsync();

        return ServiceResult<List<CommentReadDto>>.Ok(comments);
    }

    public async Task<ServiceResult<CommentReadDto>> CreateAsync(int postId, int userId, string userName, CommentCreateDto dto)
    {
        var post = await _blogPostRepository.GetByIdAsync(postId);
        if (post is null)
            return ServiceResult<CommentReadDto>.Fail(StatusCodes.Status404NotFound, "Blog post not found.");

        if (post.UserId == userId)
            return ServiceResult<CommentReadDto>.Fail(StatusCodes.Status400BadRequest, "You cannot comment on your own blog post.");

        var comment = new Comment
        {
            Content = dto.Content,
            BlogPostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var created = new CommentReadDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UserId = comment.UserId,
            UserName = userName
        };

        return ServiceResult<CommentReadDto>.Created(created);
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
