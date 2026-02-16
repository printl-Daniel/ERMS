using ERMS.Data;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Repositories.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ERMSDbContext _context;

        public DepartmentRepository(ERMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetAllActiveAsync()
        {
            return await _context.Departments
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<Department> GetByIdActiveAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<Department> CreateAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<Department> UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var department = await GetByIdAsync(id);
            if (department == null || department.IsDeleted)
                return false;

            department.IsDeleted = true;
            department.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            return await _context.Departments
                .AnyAsync(d => d.Name.ToLower() == name.ToLower()
                    && !d.IsDeleted
                    && (!excludeId.HasValue || d.Id != excludeId.Value));
        }

        public async Task<IEnumerable<SelectListItem>> GetDepartmentDropdownAsync()
        {
            return await _context.Departments
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToListAsync();
        }
    }
}