using ERMS.Data;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Repositories.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ERMSDbContext _context;

        public EmployeeRepository(ERMSDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .Include(e => e.User)
                .Include(e => e.Subordinates)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .Include(e => e.User)
                .OrderBy(e => e.Department.Name)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .Include(e => e.User)
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByManagerAsync(int managerId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.User)
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email);
        }
    }
}
