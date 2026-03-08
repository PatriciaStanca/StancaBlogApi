using StancaBlogApi.Infrastructure.Security;

namespace StancaBlogApi.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _userService.RegisterAsync(dto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _userService.UpdateMeAsync(userId, dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _userService.ChangePasswordAsync(userId, dto);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        if (!User.TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _userService.DeleteMeAsync(userId);
        return ToActionResult(result);
    }
}
