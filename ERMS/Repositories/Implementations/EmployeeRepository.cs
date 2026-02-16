using ERMS.Data;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
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



        public async Task<IEnumerable<Employee>> GetAllWithRelationsAsync()
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .Include(e => e.Subordinates)
                .Where(e => e.Status == Enums.EmployeeEnum.EmployeeStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetTopLevelEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Include(e => e.Subordinates)
                    .ThenInclude(s => s.Position)
                .Include(e => e.Subordinates)
                    .ThenInclude(s => s.Department)
                .Where(e => e.ManagerId == null && e.Status == Enums.EmployeeEnum.EmployeeStatus.Active)
                .ToListAsync();
        }

        public async Task<Employee> GetByIdWithHierarchyAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Include(e => e.Manager)
                    .ThenInclude(m => m.Position)
                .Include(e => e.Manager)
                    .ThenInclude(m => m.Department)
                .Include(e => e.Subordinates)
                    .ThenInclude(s => s.Position)
                .Include(e => e.Subordinates)
                    .ThenInclude(s => s.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetSubordinatesAsync(int managerId)
        {
            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Include(e => e.Subordinates)
                .Where(e => e.ManagerId == managerId && e.Status == Enums.EmployeeEnum.EmployeeStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeeChainAsync(int employeeId)
        {
            var chain = new List<Employee>();
            var employee = await GetByIdWithHierarchyAsync(employeeId);

            if (employee == null) return chain;

            chain.Add(employee);

            // Traverse up the management chain
            while (employee.Manager != null)
            {
                employee = await GetByIdWithHierarchyAsync(employee.Manager.Id);
                if (employee != null)
                    chain.Insert(0, employee);
            }

            return chain;
        }

        public async Task<IEnumerable<SelectListItem>> GetManagerDropdownAsync(int? excludeEmployeeId = null)
        {
            var query = _context.Employees
                .Where(e => e.Status == Enums.EmployeeEnum.EmployeeStatus.Active);

            // Exclude the employee themselves (can't be their own manager)
            if (excludeEmployeeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeEmployeeId.Value);
            }

            var managers = await query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FirstName + " " + e.LastName
                })
                .ToListAsync();

            // Add "No Manager" option at the beginning
            managers.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- No Manager --"
            });

            return managers;
        }
    }
}
