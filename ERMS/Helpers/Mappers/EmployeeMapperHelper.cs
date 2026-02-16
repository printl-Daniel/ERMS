using ERMS.DTOs.Employee;
using ERMS.DTOs.Profile;
using ERMS.Models;
using ERMS.ViewModels.Employee;
using ERMS.ViewModels.Profile;

namespace ERMS.Helpers.Mappers
{
    public static class EmployeeMapperHelper
    {
        // DtoToModel
        public static EmployeeListViewModel DtoToModel(this EmployeeResponseDto employee)
        {
            return new EmployeeListViewModel
            {
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
        }

        // ModelToDto - UPDATED WITH ADDRESS AND DATEOFBIRTH
        public static CreateEmployeeDto ModelToDto(this CreateEmployeeViewModel model)
        {
            return new CreateEmployeeDto
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
        }

        // Map from EmployeeResponseDto to EmployeeDetailsViewModel
        public static EmployeeDetailsViewModel ToDetailsViewModel(this EmployeeResponseDto employee, Employee employeeEntity)
        {
            return new EmployeeDetailsViewModel
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
                Subordinates = employeeEntity.Subordinates?.Select(s => s.ToSubordinateViewModel()).ToList()
            };
        }

        // Map from Employee (entity) to SubordinateViewModel
        public static SubordinateViewModel ToSubordinateViewModel(this Employee employee)
        {
            return new SubordinateViewModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Position = employee.Position.Title,
                Email = employee.Email
            };
        }

        public static CreateEmployeeResultViewModel ToResultViewModel(
           this CreateEmployeeResponseDto result,
           CreateEmployeeViewModel model)
        {
            return new CreateEmployeeResultViewModel
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
        }

        public static ProfileViewModel ToViewModel(this ProfileDto dto)
        {
            return new ProfileViewModel
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
        }

        public static UpdatePasswordDto ToDto(this UpdatePasswordViewModel viewModel, int employeeId)
        {
            return new UpdatePasswordDto
            {
                EmployeeId = employeeId,
                CurrentPassword = viewModel.CurrentPassword,
                NewPassword = viewModel.NewPassword
            };
        }

        public static UpdateProfilePictureDto ToDto(this UpdateProfilePictureViewModel viewModel, string filePath)
        {
            return new UpdateProfilePictureDto
            {
                EmployeeId = viewModel.EmployeeId,
                ProfilePicturePath = filePath
            };
        }

        public static UpdatePersonalInfoDto ToDto(this UpdatePersonalInfoViewModel viewModel)
        {
            return new UpdatePersonalInfoDto
            {
                EmployeeId = viewModel.EmployeeId,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                Address = viewModel.Address,
                DateOfBirth = viewModel.DateOfBirth
            };
        }

        //LIST
        public static List<EmployeeListViewModel> ToListViewModels(this IEnumerable<EmployeeResponseDto> employees)
        {
            return employees.Select(e => e.DtoToModel()).ToList();
        }
    }
}