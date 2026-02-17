using ERMS.DTOs.Auth;

namespace ERMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync();

        // New methods for password reset (using username)
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ValidateResetTokenAsync(string token);
    }
}
