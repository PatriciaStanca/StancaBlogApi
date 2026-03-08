
namespace StancaBlogApi.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IBlogRepository _blogRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IBlogRepository blogRepository,
        ICommentRepository commentRepository,
        IJwtService jwtService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _blogRepository = blogRepository;
        _commentRepository = commentRepository;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            return ServiceResult<UserDto>.Fail(StatusCodes.Status409Conflict, "Email is already in use.");

        if (await _userRepository.UserNameExistsAsync(dto.Name))
            return ServiceResult<UserDto>.Fail(StatusCodes.Status409Conflict, "Username is already in use.");

        var user = new User
        {
            UserName = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return ServiceResult<UserDto>.Created(_mapper.Map<UserDto>(user));
    }

    public async Task<ServiceResult<object>> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUserNameAsync(dto.Name);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ServiceResult<object>.Fail(StatusCodes.Status401Unauthorized, "Invalid credentials");

        var token = _jwtService.Generate(user);
        return ServiceResult<object>.Ok(new { token, userId = user.Id });
    }

    public async Task<ServiceResult<UserDto>> UpdateMeAsync(int userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return ServiceResult<UserDto>.Fail(StatusCodes.Status401Unauthorized, "Unauthorized.");

        if (dto.Name is not null && string.IsNullOrWhiteSpace(dto.Name))
            return ServiceResult<UserDto>.Fail(StatusCodes.Status400BadRequest, "Username cannot be empty.");

        if (!string.IsNullOrWhiteSpace(dto.Email) && await _userRepository.EmailExistsAsync(dto.Email, user.Id))
            return ServiceResult<UserDto>.Fail(StatusCodes.Status409Conflict, "Email is already in use.");

        if (!string.IsNullOrWhiteSpace(dto.Name) && await _userRepository.UserNameExistsAsync(dto.Name, user.Id))
            return ServiceResult<UserDto>.Fail(StatusCodes.Status409Conflict, "Username is already in use.");

        user.UserName = dto.Name ?? user.UserName;
        user.Email = dto.Email ?? user.Email;

        await _userRepository.SaveChangesAsync();

        return ServiceResult<UserDto>.Ok(_mapper.Map<UserDto>(user));
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

        var posts = await _blogRepository.GetByUserIdAsync(userId);
        _blogRepository.RemoveRange(posts);

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();

        return ServiceResult.NoContent();
    }
}
