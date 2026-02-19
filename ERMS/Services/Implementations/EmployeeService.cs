using ERMS.Constants;
using ERMS.DTOs.Employee;
using ERMS.Helpers;
using ERMS.Helpers.Mappers;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Employee;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(EmployeeMapper.MapToResponseDto);
        }

        public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee != null ? EmployeeMapper.MapToResponseDto(employee) : null;
        }

        public async Task<UpdateEmployeeDto> GetEmployeeForEditAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee?.ToEditDto();
        }

        public async Task<EmployeeDetailsViewModel> GetEmployeeDetailsAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return null;

            var dto = EmployeeMapper.MapToResponseDto(employee);
            return dto.ToDetailsViewModel(employee);
        }

        public async Task<DeleteEmployeeViewModel> GetEmployeeForDeleteAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return null;

            var dto = EmployeeMapper.MapToResponseDto(employee);
            var subordinateCount = employee.Subordinates?.Count() ?? 0;
            return dto.ToDeleteViewModel(subordinateCount);
        }

        public async Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            try
            {
                if (await _employeeRepository.EmailExistsAsync(dto.Email))
                    return EmployeeMapper.ToCreateFailResponse(Messages.Error.Employee.EmailExists);

                var employee = EmployeeMapper.ToEntity(dto);
                var createdEmployee = await _employeeRepository.CreateAsync(employee);

                var username = UserMapperHelper.GenerateUsername(dto.Email);
                var generatedPassword = PasswordHelper.GenerateRandomPassword();
                var hashedPassword = PasswordHelper.HashPassword(generatedPassword);

                if (!Enum.TryParse(dto.Role, out UserRole userRole))
                    userRole = UserRole.Employee;

                var user = UserMapperHelper.ToEntity(username, hashedPassword, userRole, createdEmployee.Id);
                await _userRepository.CreateAsync(user);

                bool isManager = userRole == UserRole.Manager || userRole == UserRole.Admin;

                var emailSent = await _emailService.SendPasswordEmailAsync(
                    dto.Email,
                    $"{dto.FirstName} {dto.LastName}",
                    username,
                    generatedPassword,
                    isManager
                );

                return EmployeeMapper.ToCreateResponse(createdEmployee, username, generatedPassword, emailSent);
            }
            catch (Exception ex)
            {
                return new CreateEmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error creating employee: {ex.Message}"
                };
            }
        }

        public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(dto.Id);
            if (employee == null)
                throw new InvalidOperationException(Messages.Error.Employee.NotFound);

            if (!Enum.TryParse<EmployeeStatus>(dto.Status, ignoreCase: true, out var status))
                throw new InvalidOperationException(Messages.Error.InvalidInput);

            EmployeeMapper.ApplyUpdate(employee, dto, status);
            await _employeeRepository.UpdateAsync(employee);
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return false;

            if (employee.User != null)
                employee.User.IsActive = false;

            return await _employeeRepository.DeleteAsync(id);
        }

        public async Task PopulateDropdowns(CreateEmployeeViewModel model, int? excludeEmployeeId = null)
        {
            model.Departments = await _departmentRepository.GetDepartmentDropdownAsync();
            model.Positions = await _positionRepository.GetPositionDropdownAsync();
            model.Managers = await _employeeRepository.GetManagerDropdownAsync(excludeEmployeeId);
        }

        public async Task PopulateDropdowns(EmployeeEditViewModel model, int? excludeEmployeeId = null)
        {
            model.Departments = await _departmentRepository.GetDepartmentDropdownAsync();
            model.Positions = await _positionRepository.GetPositionDropdownAsync();
            model.Managers = await _employeeRepository.GetManagerDropdownAsync(excludeEmployeeId);
        }
    }
}