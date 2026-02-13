using ERMS.Data;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Repositories.Implementations
{
    public class PositionRepository : IPositionRepository
    {
        private readonly ERMSDbContext _context;

        public PositionRepository(ERMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Position>> GetAllAsync()
        {
            return await _context.Positions
                .Include(p => p.Employees)
                .OrderBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Position>> GetAllActiveAsync()
        {
            return await _context.Positions
                .Include(p => p.Employees)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<Position> GetByIdAsync(int id)
        {
            return await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Position> GetActiveByIdAsync(int id)
        {
            return await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Position> CreateAsync(Position position)
        {
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<Position> UpdateAsync(Position position)
        {
            _context.Positions.Update(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var position = await GetByIdAsync(id);
            if (position == null || position.IsDeleted)
                return false;

            position.IsDeleted = true;
            position.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Positions
                .AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<bool> TitleExistsAsync(string title, int? excludeId = null)
        {
            return await _context.Positions
                .AnyAsync(p => p.Title.ToLower() == title.ToLower()
                    && !p.IsDeleted
                    && (!excludeId.HasValue || p.Id != excludeId.Value));
        }
    }
}