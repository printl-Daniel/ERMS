using ERMS.DTOs.Auth;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;

namespace ERMS.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.ValidateUserAsync(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            if (!user.IsActive)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Your account has been deactivated. Please contact HR."
                };
            }

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful",
                Role = user.Role.ToString(),
                UserId = user.Id,
                EmployeeId = user.EmployeeId,
                FullName = user.Employee.FullName
            };
        }

        public async Task<bool> LogoutAsync()
        {
            // Implement logout logic (clear session, etc.)
            return await Task.FromResult(true);
        }
    }
}
