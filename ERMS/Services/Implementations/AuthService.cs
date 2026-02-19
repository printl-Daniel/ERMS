using BCrypt.Net;
using ERMS.Constants;
using ERMS.DTOs.Auth;
using ERMS.Helpers;
using ERMS.Helpers.Mappers;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;

namespace ERMS.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        private const int RESET_REQUEST_COOLDOWN_MINUTES = 5;

        public AuthService(
            IAuthRepository authRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _authRepository = authRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var username = loginDto.Username?.Trim();
            if (string.IsNullOrWhiteSpace(username))
                return Fail(Messages.Error.Auth.InvalidCredentials);

            var user = await _authRepository.GetUserByUsernameAsync(username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Fail(Messages.Error.Auth.InvalidCredentials);

            if (!user.IsActive)
                return Fail(Messages.Error.Auth.AccountDeactivated);

            // ── First login → generate reset token, skip signing in ──
            if (user.IsFirstLogin)
            {
                await _authRepository.InvalidateOldTokensAsync(user.Id);

                var token = PasswordHelper.GenerateSecureToken();
                var expiry = DateTime.UtcNow.AddHours(1);
                await _authRepository.CreatePasswordResetTokenAsync(user.Id, token, expiry);

                return new LoginResponseDto
                {
                    Success = true,
                    IsFirstLogin = true,
                    FirstLoginToken = token
                };
            }

            return AuthMapper.ToLoginResponse(user);
        }

        public async Task<bool> LogoutAsync() => await Task.FromResult(true);

        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var email = forgotPasswordDto.Email?.Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(email))
                    return new ForgotPasswordResponseDto { Success = false, Message = Messages.Error.Auth.EmailRequired };

                var user = await _authRepository.GetUserByEmailAsync(email);

                if (user == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    return new ForgotPasswordResponseDto
                    {
                        Success = false,
                        Message = Messages.Error.Auth.EmailNotFound
                    };
                }

                var recentToken = await _authRepository.GetMostRecentPasswordResetTokenAsync(user.Id);
                if (recentToken != null && !recentToken.IsUsed)
                {
                    var elapsed = DateTime.UtcNow - recentToken.CreatedAt;
                    if (elapsed.TotalMinutes < RESET_REQUEST_COOLDOWN_MINUTES)
                    {
                        var remaining = RESET_REQUEST_COOLDOWN_MINUTES - (int)elapsed.TotalMinutes;
                        return new ForgotPasswordResponseDto
                        {
                            Success = false,
                            Message = string.Format(Messages.Warning.Auth.ResetCooldown, remaining)
                        };
                    }
                }

                await _authRepository.InvalidateOldTokensAsync(user.Id);

                var resetToken = PasswordHelper.GenerateSecureToken();
                var resetExpiry = DateTime.UtcNow.AddHours(1);
                await _authRepository.CreatePasswordResetTokenAsync(user.Id, resetToken, resetExpiry);

                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7280";
                var encodedToken = Uri.EscapeDataString(resetToken);
                var resetLink = $"{baseUrl}/Auth/ResetPassword?token={encodedToken}";

                var sent = await _emailService.SendPasswordResetEmailAsync(
                    user.Employee.Email, resetLink, user.Employee.FullName);

                if (!sent)
                    return new ForgotPasswordResponseDto { Success = false, Message = Messages.Error.Auth.EmailSendFailed };

                return EnumerationSafeSuccess();
            }
            catch
            {
                return new ForgotPasswordResponseDto { Success = false, Message = Messages.Error.Auth.GenericError };
            }
        }

        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var resetToken = await _authRepository.GetPasswordResetTokenAsync(resetPasswordDto.Token);

                if (resetToken == null || resetToken.IsUsed)
                    return new ResetPasswordResponseDto { Success = false, Message = Messages.Error.Auth.InvalidToken };

                if (resetToken.ExpiresAt < DateTime.UtcNow)
                    return new ResetPasswordResponseDto { Success = false, Message = Messages.Error.Auth.ExpiredToken };

                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
                var updated = await _authRepository.UpdatePasswordAsync(resetToken.UserId, newPasswordHash);

                if (!updated)
                    return new ResetPasswordResponseDto { Success = false, Message = Messages.Error.Auth.PasswordUpdateFailed };

                await _authRepository.MarkTokenAsUsedAsync(resetToken.Id);

                return new ResetPasswordResponseDto
                {
                    Success = true,
                    Message = Messages.Success.Auth.PasswordResetSuccess
                };
            }
            catch
            {
                return new ResetPasswordResponseDto { Success = false, Message = Messages.Error.Auth.GenericError };
            }
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            try
            {
                var resetToken = await _authRepository.GetPasswordResetTokenAsync(token);
                return resetToken != null && !resetToken.IsUsed && resetToken.ExpiresAt > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        // ── PRIVATES ─────────────────────────────────────────────────────────────

        private static LoginResponseDto Fail(string message) =>
            new LoginResponseDto { Success = false, Message = message };

        private static ForgotPasswordResponseDto EnumerationSafeSuccess() =>
            new ForgotPasswordResponseDto
            {
                Success = true,
                Message = Messages.Success.Auth.PasswordResetSent
            };
    }
}