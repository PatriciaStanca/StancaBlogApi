using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Data.Interfaces;
using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Repos;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(int id) => _context.Users.FindAsync(id).AsTask();

    public Task<User?> GetByUserNameAsync(string userName) =>
        _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

    public Task<bool> EmailExistsAsync(string email, int? excludeUserId = null) =>
        _context.Users.AnyAsync(u => u.Email == email && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));

    public Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null) =>
        _context.Users.AnyAsync(u => u.UserName == userName && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));

    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

    public void Remove(User user) => _context.Users.Remove(user);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
