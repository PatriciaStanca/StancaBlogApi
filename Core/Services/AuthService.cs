using Microsoft.AspNetCore.Http;
using StancaBlogApi.Core.Common;
using StancaBlogApi.Core.Interfaces;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.DTOs;
using StancaBlogApi.Models;

namespace StancaBlogApi.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IBlogPostRepository blogPostRepository,
        ICommentRepository commentRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _blogPostRepository = blogPostRepository;
        _commentRepository = commentRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ServiceResult<object>> RegisterAsync(RegisterDto dto)
    {
        var validationResult = await ValidateRegisterAsync(dto);
        if (validationResult is not null)
            return validationResult;

        var user = CreateUser(dto);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return ServiceResult<object>.Created(ToUserResponse(user));
    }

    public async Task<ServiceResult<object>> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUserNameAsync(dto.Name);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return InvalidCredentials();

        var token = _jwtTokenGenerator.Generate(user);
        return ServiceResult<object>.Ok(new { token, userId = user.Id });
    }

    public async Task<ServiceResult<object>> UpdateMeAsync(int userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return UnauthorizedObject();

        var validationResult = await ValidateUpdateAsync(dto, user.Id);
        if (validationResult is not null)
            return validationResult;

        ApplyProfileUpdates(user, dto);

        await _userRepository.SaveChangesAsync();

        return ServiceResult<object>.Ok(ToUserResponse(user));
    }

    public async Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Unauthorized();

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Current password is incorrect.");

        if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "New password must be different from current password.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _userRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    public async Task<ServiceResult> DeleteMeAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Unauthorized();

        var comments = await _commentRepository.GetByUserIdAsync(userId);
        _commentRepository.RemoveRange(comments);

        var posts = await _blogPostRepository.GetByUserIdAsync(userId);
        _blogPostRepository.RemoveRange(posts);

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    private async Task<ServiceResult<object>?> ValidateRegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Email is already in use.");

        if (await _userRepository.UserNameExistsAsync(dto.Name))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Username is already in use.");

        return null;
    }

    private async Task<ServiceResult<object>?> ValidateUpdateAsync(UpdateUserDto dto, int userId)
    {
        if (dto.Name is not null && string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Username cannot be empty.");

        if (!string.IsNullOrWhiteSpace(dto.Email) && await _userRepository.EmailExistsAsync(dto.Email, userId))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Email is already in use.");

        if (!string.IsNullOrWhiteSpace(dto.Name) && await _userRepository.UserNameExistsAsync(dto.Name, userId))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Username is already in use.");

        return null;
    }

    private static User CreateUser(RegisterDto dto)
    {
        return new User
        {
            UserName = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
    }

    private static void ApplyProfileUpdates(User user, UpdateUserDto dto)
    {
        user.UserName = dto.Name ?? user.UserName;
        user.Email = dto.Email ?? user.Email;
    }

    private static object ToUserResponse(User user) => new
    {
        userId = user.Id,
        userName = user.UserName,
        email = user.Email
    };

    private static ServiceResult<object> InvalidCredentials() =>
        ServiceResult<object>.Fail(StatusCodes.Status401Unauthorized, "Invalid credentials");

    private static ServiceResult<object> UnauthorizedObject() =>
        ServiceResult<object>.Fail(StatusCodes.Status401Unauthorized, "Unauthorized.");

    private static ServiceResult Unauthorized() =>
        ServiceResult.Fail(StatusCodes.Status401Unauthorized, "Unauthorized.");
}
