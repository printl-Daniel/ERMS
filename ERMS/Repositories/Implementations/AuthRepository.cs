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
            // In production, compare hashed passwords using BCrypt or similar
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
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // DEMO ONLY - In production, use BCrypt.Net.BCrypt.Verify(password, passwordHash)
            return password == passwordHash;
        }





    }
}
