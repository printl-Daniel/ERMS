using ERMS.DTOs.Employee;

namespace ERMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync();
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id);
        Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<bool> UpdateEmployeeAsync(UpdateEmployeeDto dto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
