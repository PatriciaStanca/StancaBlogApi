using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StancaBlogApi.Core.Interfaces;
using StancaBlogApi.DTOs;
using StancaBlogApi.Infrastructure.Security;

namespace StancaBlogApi.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _authService.UpdateMeAsync(userId, dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _authService.ChangePasswordAsync(userId, dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _authService.DeleteMeAsync(userId);
        return ToActionResult(result);
    }
}
