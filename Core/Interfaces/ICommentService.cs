using StancaBlogApi.Core.Common;
using StancaBlogApi.DTOs;

namespace StancaBlogApi.Core.Interfaces;

public interface ICommentService
{
    Task<ServiceResult<List<CommentReadDto>>> GetByPostAsync(int postId);
    Task<ServiceResult<CommentReadDto>> CreateAsync(int postId, int userId, string userName, CommentCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, int userId, CommentUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id, int userId);
}
