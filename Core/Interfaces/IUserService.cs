
namespace StancaBlogApi.Core.Interfaces;

public interface IUserService
{
    Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<object>> LoginAsync(LoginDto dto);
    Task<ServiceResult<UserDto>> UpdateMeAsync(int userId, UpdateUserDto dto);
    Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<ServiceResult> DeleteMeAsync(int userId);
}
