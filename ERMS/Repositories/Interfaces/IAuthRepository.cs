using ERMS.Models;

namespace ERMS.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> ValidateUserAsync(string username, string password);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email); // ✅ NEW METHOD
        Task<bool> UsernameExistsAsync(string username);

        // Password reset methods
        Task<PasswordResetToken> CreatePasswordResetTokenAsync(int userId, string token, DateTime expiresAt);
        Task<PasswordResetToken> GetPasswordResetTokenAsync(string token);
        Task<bool> MarkTokenAsUsedAsync(int tokenId);
        Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash);
        Task<bool> InvalidateOldTokensAsync(int userId);
        Task<PasswordResetToken> GetMostRecentPasswordResetTokenAsync(int userId);
    }
}