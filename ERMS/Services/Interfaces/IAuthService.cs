using ERMS.DTOs.Auth;

namespace ERMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync();
    }
}
