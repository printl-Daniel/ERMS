using ERMS.Constants;
using ERMS.DTOs.Auth;
using ERMS.Models;
using ERMS.ViewModels.Auth;

namespace ERMS.Helpers
{
    public static class AuthMapper
    {
        //VIEWMODEL TO DTO
        public static LoginDto ViewModelToDto(this LoginViewModel viewModel) =>
            new LoginDto
            {
                Username = viewModel.Username,
                Password = viewModel.Password,
                RememberMe = viewModel.RememberMe
            };

        //LOGIN RESPONSE
        public static LoginResponseDto ToLoginResponse(User user) =>
            new LoginResponseDto
            {
                Success = true,
                Message = Messages.Success.Auth.LoginSuccess,
                Role = user.Role.ToString(),
                UserId = user.Id,
                EmployeeId = user.EmployeeId,
                FullName = user.Employee.FullName,
                IsFirstLogin = user.IsFirstLogin  // ← ADD
            };
    }
}
