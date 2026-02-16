using StancaBlogApi.Models;

namespace StancaBlogApi.Data.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUserNameAsync(string userName);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null);
    Task AddAsync(User user);
    void Remove(User user);
    Task SaveChangesAsync();
}
