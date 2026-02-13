using ERMS.Data;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ERMSDbContext _context;

        public UserRepository(ERMSDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
