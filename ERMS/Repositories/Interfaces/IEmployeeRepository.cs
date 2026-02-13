using ERMS.Models;

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
    }
}
