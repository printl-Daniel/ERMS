using ERMS.DTOs;
using ERMS.DTOs.Department;

namespace ERMS.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto> GetByIdAsync(int id);
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
        Task<DepartmentDto> UpdateAsync(UpdateDepartmentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}