using ERMS.DTOs.Auth;
using ERMS.ViewModels.Auth;

namespace ERMS.Helpers
{
    public static class AuthMapper
    {
        //VIEWMODEL TO DTO
        public static LoginDto ViewModelToDto(this LoginViewModel viewModel)
        {
            return new LoginDto
            {
                Username = viewModel.Username,
                Password = viewModel.Password,
                RememberMe = viewModel.RememberMe
            };
        }
    }
}