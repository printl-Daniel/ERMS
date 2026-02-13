using ERMS.Models;

namespace ERMS.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> ValidateUserAsync(string username, string password);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
    }
}
