using ERMS.Data;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ERMSDbContext _context;

        public AuthRepository(ERMSDbContext context)
        {
            _context = context;
        }

        public async Task<User> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Employee)
                .ThenInclude(e => e.Department)
                .Include(u => u.Employee)
                .ThenInclude(e => e.Position)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        // ✅ NEW METHOD: Find user by email address
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Employee.Email.ToLower() == email.ToLower() && u.IsActive);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<PasswordResetToken> CreatePasswordResetTokenAsync(int userId, string token, DateTime expiresAt)
        {
            var resetToken = new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IsUsed = false
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            return resetToken;
        }

        public async Task<PasswordResetToken> GetPasswordResetTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .Include(t => t.User)
                .ThenInclude(u => u.Employee)
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed);
        }

        public async Task<bool> MarkTokenAsUsedAsync(int tokenId)
        {
            var token = await _context.PasswordResetTokens.FindAsync(tokenId);
            if (token == null) return false;

            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = newPasswordHash;
            user.IsFirstLogin = false;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InvalidateOldTokensAsync(int userId)
        {
            var oldTokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == userId && !t.IsUsed)
                .ToListAsync();

            foreach (var token in oldTokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PasswordResetToken> GetMostRecentPasswordResetTokenAsync(int userId)
        {
            return await _context.PasswordResetTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // DEMO ONLY - In production, use BCrypt.Net.BCrypt.Verify(password, passwordHash)
            return password == passwordHash;
        }

        public async Task<bool> SetFirstLoginCompleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsFirstLogin = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}