using StancaBlogApi.DTOs;
using StancaBlogApi.Models;

namespace StancaBlogApi.Application.Mappings;

public static class BlogPostMappings
{
    public static BlogPostReadDto ToReadDto(this BlogPost post)
    {
        return new BlogPostReadDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            CategoryId = post.CategoryId,
            CategoryName = post.Category?.Name ?? string.Empty,
            UserId = post.UserId,
            UserName = post.User?.UserName ?? string.Empty,
            Comments = post.Comments
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentReadDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User?.UserName ?? string.Empty
                })
                .ToList()
        };
    }
}
