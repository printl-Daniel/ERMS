using ERMS.DTOs.Employee;
using ERMS.ViewModels.Employee;

namespace ERMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync();
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id);
        Task<UpdateEmployeeDto> GetEmployeeForEditAsync(int id);
        Task<EmployeeDetailsViewModel> GetEmployeeDetailsAsync(int id);
        Task<DeleteEmployeeViewModel> GetEmployeeForDeleteAsync(int id);
        Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<bool> UpdateEmployeeAsync(UpdateEmployeeDto dto);
        Task<bool> DeleteEmployeeAsync(int id);
        Task PopulateDropdowns(CreateEmployeeViewModel model, int? excludeEmployeeId = null);
        Task PopulateDropdowns(EmployeeEditViewModel model, int? excludeEmployeeId = null);
    }
}