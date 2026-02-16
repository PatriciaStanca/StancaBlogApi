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
        if (await _userRepository.EmailExistsAsync(dto.Email))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Email is already in use.");

        if (await _userRepository.UserNameExistsAsync(dto.Name))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Username is already in use.");

        var user = new User
        {
            UserName = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return ServiceResult<object>.Created(new
        {
            userId = user.Id,
            userName = user.UserName,
            email = user.Email
        });
    }

    public async Task<ServiceResult<object>> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUserNameAsync(dto.Name);
        if (user is null)
            return ServiceResult<object>.Fail(StatusCodes.Status401Unauthorized, "Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ServiceResult<object>.Fail(StatusCodes.Status401Unauthorized, "Invalid credentials");

        var token = _jwtTokenGenerator.Generate(user);
        return ServiceResult<object>.Ok(new { token, userId = user.Id });
    }

    public async Task<ServiceResult<object>> UpdateMeAsync(int userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return ServiceResult<object>.Fail(StatusCodes.Status401Unauthorized, "Unauthorized.");

        if (!string.IsNullOrWhiteSpace(dto.Email) && await _userRepository.EmailExistsAsync(dto.Email, user.Id))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Email is already in use.");

        if (!string.IsNullOrWhiteSpace(dto.Name) && await _userRepository.UserNameExistsAsync(dto.Name, user.Id))
            return ServiceResult<object>.Fail(StatusCodes.Status409Conflict, "Username is already in use.");

        user.UserName = dto.Name ?? user.UserName;
        user.Email = dto.Email ?? user.Email;

        await _userRepository.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            userName = user.UserName,
            email = user.Email
        });
    }

    public async Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return ServiceResult.Fail(StatusCodes.Status401Unauthorized, "Unauthorized.");

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
            return ServiceResult.Fail(StatusCodes.Status401Unauthorized, "Unauthorized.");

        var comments = await _commentRepository.GetByUserIdAsync(userId);
        _commentRepository.RemoveRange(comments);

        var posts = await _blogPostRepository.GetByUserIdAsync(userId);
        _blogPostRepository.RemoveRange(posts);

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }
}
