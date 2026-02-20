using ERMS.Constants;
using ERMS.DTOs.Employee;
using ERMS.DTOs.Profile;
using ERMS.Models;
using ERMS.ViewModels.Employee;
using ERMS.ViewModels.Profile;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Helpers.Mappers
{
    public static class EmployeeMapper
    {
        // ══════════════════════════════════════════════════════════════
        // ENTITY MAPPERS  (Entity → DTO)
        // ══════════════════════════════════════════════════════════════

        // Employee → EmployeeResponseDto
        public static EmployeeResponseDto MapToResponseDto(Employee employee) =>
            new EmployeeResponseDto
            {
                ProfilePicturePath = employee.ProfilePicturePath,
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                HireDate = employee.HireDate,
                Status = employee.Status.ToString(),
                DepartmentName = employee.Department?.Name,
                PositionTitle = employee.Position?.Title,
                ManagerName = employee.Manager?.FullName,
                Username = employee.User?.Username,
                Role = employee.User?.Role.ToString(),
                CreatedAt = employee.CreatedAt,       // ← added
                UpdatedAt = employee.UpdatedAt        // ← added
            };

        // Employee → UpdateEmployeeDto  (used by service: GetEmployeeForEditAsync)
        public static UpdateEmployeeDto ToEditDto(this Employee employee) =>
            new UpdateEmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                DepartmentId = employee.DepartmentId,
                PositionId = employee.PositionId,
                ManagerId = employee.ManagerId,
                Status = employee.Status.ToString(),
                CreatedAt = employee.CreatedAt,       // ← added
                UpdatedAt = employee.UpdatedAt        // ← added
            };

        // Employee → SubordinateViewModel  (used by service: GetEmployeeDetailsAsync)
        public static SubordinateViewModel ToSubordinateViewModel(this Employee employee) =>
            new SubordinateViewModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Position = employee.Position.Title,
                Email = employee.Email
            };

        // CreateEmployeeDto → Employee  (used by service: CreateEmployeeAsync)
        public static Employee ToEntity(CreateEmployeeDto dto) =>
            new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                HireDate = dto.HireDate,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                ManagerId = dto.ManagerId,
                Status = EmployeeStatus.Active,
                CreatedAt = DateTime.Now            // ← added
            };

        // UpdateEmployeeDto → Employee  (used by service: UpdateEmployeeAsync)
        public static void ApplyUpdate(Employee employee, UpdateEmployeeDto dto, EmployeeStatus status)
        {
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.DepartmentId = dto.DepartmentId;
            employee.PositionId = dto.PositionId;
            employee.ManagerId = dto.ManagerId;
            employee.Status = status;
            employee.UpdatedAt = DateTime.Now;      // ← already existed, kept
        }

        // ══════════════════════════════════════════════════════════════
        // DTO MAPPERS  (DTO → ViewModel)
        // ══════════════════════════════════════════════════════════════

        // UpdateEmployeeDto → EmployeeEditViewModel  (used by controller: GET Edit)
        public static EmployeeEditViewModel ToEditViewModel(this UpdateEmployeeDto dto) =>
            new EmployeeEditViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                ManagerId = dto.ManagerId,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt,          // ← added
                UpdatedAt = dto.UpdatedAt           // ← added
            };

        public static EmployeeListViewModel DtoToModel(this EmployeeResponseDto employee) =>
            new EmployeeListViewModel
            {
                ProfilePicturePath = employee.ProfilePicturePath,
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Department = employee.DepartmentName,
                Position = employee.PositionTitle,
                Manager = employee.ManagerName,
                Status = employee.Status.ToString(),
                HireDate = employee.HireDate,
                Role = employee.Role
            };

        // EmployeeResponseDto + Employee → EmployeeDetailsViewModel  (used by service: GetEmployeeDetailsAsync)
        public static EmployeeDetailsViewModel ToDetailsViewModel(this EmployeeResponseDto employee, Employee employeeEntity) =>
            new EmployeeDetailsViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                HireDate = employee.HireDate,
                Status = employee.Status,
                Department = employee.DepartmentName,
                Position = employee.PositionTitle,
                Manager = employee.ManagerName,
                Username = employee.Username,
                Role = employee.Role,
                CreatedAt = employee.CreatedAt,     // ← added
                UpdatedAt = employee.UpdatedAt,     // ← added
                Subordinates = employeeEntity.Subordinates?
                                   .Select(s => s.ToSubordinateViewModel())
                                   .ToList()
            };

        // EmployeeResponseDto → DeleteEmployeeViewModel  (used by service: GetEmployeeForDeleteAsync)
        public static DeleteEmployeeViewModel ToDeleteViewModel(this EmployeeResponseDto employee, int subordinateCount = 0) =>
            new DeleteEmployeeViewModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Department = employee.DepartmentName,
                Position = employee.PositionTitle,
                Manager = employee.ManagerName,
                Status = employee.Status.ToString(),
                Role = employee.Role,
                HireDate = employee.HireDate,
                SubordinateCount = subordinateCount
            };

        // CreateEmployeeResponseDto + CreateEmployeeViewModel → CreateEmployeeResultViewModel  (used by controller: POST Create)
        public static CreateEmployeeResultViewModel ToResultViewModel(this CreateEmployeeResponseDto result, CreateEmployeeViewModel model) =>
            new CreateEmployeeResultViewModel
            {
                Success = true,
                Message = result.Message,
                EmployeeId = result.EmployeeId,
                EmployeeName = $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                Username = result.Username,
                GeneratedPassword = result.GeneratedPassword,
                EmailSent = result.EmailSent
            };

        // ══════════════════════════════════════════════════════════════
        // VIEWMODEL MAPPERS  (ViewModel → DTO)
        // ══════════════════════════════════════════════════════════════

        // EmployeeEditViewModel → UpdateEmployeeDto  (used by controller: POST Edit)
        public static UpdateEmployeeDto ToUpdateDto(this EmployeeEditViewModel vm) =>
            new UpdateEmployeeDto
            {
                Id = vm.Id,
                FirstName = vm.FirstName.Trim(),
                LastName = vm.LastName.Trim(),
                Email = vm.Email.Trim(),
                PhoneNumber = vm.PhoneNumber?.Trim(),
                DepartmentId = vm.DepartmentId,
                PositionId = vm.PositionId,
                ManagerId = vm.ManagerId,
                Status = vm.Status,
                // CreatedAt/UpdatedAt intentionally not mapped here
                // — ApplyUpdate() handles UpdatedAt on the entity directly
            };

        // CreateEmployeeViewModel → CreateEmployeeDto  (used by controller: POST Create)
        public static CreateEmployeeDto ModelToDto(this CreateEmployeeViewModel model) =>
            new CreateEmployeeDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                HireDate = model.HireDate,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                DepartmentId = model.DepartmentId,
                PositionId = model.PositionId,
                ManagerId = model.ManagerId,
                Role = model.Role
            };

        // ══════════════════════════════════════════════════════════════
        // RESPONSE MAPPERS  (used by service: CreateEmployeeAsync)
        // ══════════════════════════════════════════════════════════════

        public static CreateEmployeeResponseDto ToCreateResponse(
            Employee createdEmployee,
            string username,
            string generatedPassword,
            bool emailSent) =>
            new CreateEmployeeResponseDto
            {
                Success = true,
                Message = Messages.Success.Employee.Created,
                EmployeeId = createdEmployee.Id,
                Username = username,
                GeneratedPassword = generatedPassword,
                EmailSent = emailSent
            };

        public static CreateEmployeeResponseDto ToCreateFailResponse(string message) =>
            new CreateEmployeeResponseDto
            {
                Success = false,
                Message = message
            };

        // ══════════════════════════════════════════════════════════════
        // PROFILE MAPPERS
        // ══════════════════════════════════════════════════════════════

        // ProfileDto → ProfileViewModel  (used by controller: Profile)
        public static ProfileViewModel ToViewModel(this ProfileDto dto) =>
            new ProfileViewModel
            {
                EmployeeId = dto.EmployeeId,
                EmployeeNumber = dto.EmployeeNumber,
                FullName = dto.FullName,
                Position = dto.Position,
                Department = dto.Department,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                HireDate = dto.HireDate,
                ProfilePicturePath = dto.ProfilePicturePath
            };

        // UpdatePasswordViewModel → UpdatePasswordDto  (used by controller: Profile)
        public static UpdatePasswordDto ToDto(this UpdatePasswordViewModel viewModel, int employeeId) =>
            new UpdatePasswordDto
            {
                EmployeeId = employeeId,
                CurrentPassword = viewModel.CurrentPassword,
                NewPassword = viewModel.NewPassword
            };

        // UpdateProfilePictureViewModel → UpdateProfilePictureDto  (used by controller: Profile)
        public static UpdateProfilePictureDto ToDto(this UpdateProfilePictureViewModel viewModel, string filePath) =>
            new UpdateProfilePictureDto
            {
                EmployeeId = viewModel.EmployeeId,
                ProfilePicturePath = filePath
            };

        // UpdatePersonalInfoViewModel → UpdatePersonalInfoDto  (used by controller: Profile)
        public static UpdatePersonalInfoDto ToDto(this UpdatePersonalInfoViewModel viewModel) =>
            new UpdatePersonalInfoDto
            {
                EmployeeId = viewModel.EmployeeId,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                Address = viewModel.Address,
                DateOfBirth = viewModel.DateOfBirth
            };

        // ══════════════════════════════════════════════════════════════
        // COLLECTION MAPPERS
        // ══════════════════════════════════════════════════════════════

        // IEnumerable<EmployeeResponseDto> → List<EmployeeListViewModel>  (used by controller: Index)
        public static List<EmployeeListViewModel> ToListViewModels(this IEnumerable<EmployeeResponseDto> employees) =>
            employees.Select(e => e.DtoToModel()).ToList();
    }
}