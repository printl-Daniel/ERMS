using ERMS.Models;

namespace ERMS.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<IEnumerable<Department>> GetAllActiveAsync();
        Task<Department> GetByIdAsync(int id);
        Task<Department> GetByIdActiveAsync(int id);
        Task<Department> CreateAsync(Department department);
        Task<Department> UpdateAsync(Department department);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}