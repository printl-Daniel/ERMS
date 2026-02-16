using ERMS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERMS.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByEmailAsync(string email);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetByManagerAsync(int managerId);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);


        // New methods for hierarchy
        Task<IEnumerable<Employee>> GetAllWithRelationsAsync();
        Task<IEnumerable<Employee>> GetTopLevelEmployeesAsync();
        Task<Employee> GetByIdWithHierarchyAsync(int id);
        Task<IEnumerable<Employee>> GetSubordinatesAsync(int managerId);
        Task<IEnumerable<Employee>> GetEmployeeChainAsync(int employeeId);

        Task<IEnumerable<SelectListItem>> GetManagerDropdownAsync(int? excludeEmployeeId = null);
    }
}
