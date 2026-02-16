using StancaBlogApi.Core.Common;
using StancaBlogApi.DTOs;

namespace StancaBlogApi.Core.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<object>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<object>> LoginAsync(LoginDto dto);
    Task<ServiceResult<object>> UpdateMeAsync(int userId, UpdateUserDto dto);
    Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<ServiceResult> DeleteMeAsync(int userId);
}
