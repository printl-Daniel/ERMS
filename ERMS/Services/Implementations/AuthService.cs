using ERMS.DTOs.Auth;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;
using System.Security.Cryptography;

namespace ERMS.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        // ✅ Anti-spam: Cooldown period in minutes
        private const int RESET_REQUEST_COOLDOWN_MINUTES = 5;

        public AuthService(
            IAuthRepository authRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
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
            return await Task.FromResult(true);
        }

        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                // ✅ CHANGED: Find user by EMAIL instead of username
                var user = await _authRepository.GetUserByEmailAsync(forgotPasswordDto.Email);

                // Always return success to prevent email enumeration
                if (user == null)
                {
                    _logger.LogWarning($"Password reset requested for non-existent email: {forgotPasswordDto.Email}");

                    // ✅ Add artificial delay to prevent email enumeration timing attacks
                    await Task.Delay(TimeSpan.FromSeconds(2));

                    return new ForgotPasswordResponseDto
                    {
                        Success = true,
                        Message = "If an account with that email exists, a password reset link has been sent."
                    };
                }

                // ✅ ANTI-SPAM: Check if user recently requested a reset
                var recentToken = await _authRepository.GetMostRecentPasswordResetTokenAsync(user.Id);
                if (recentToken != null && !recentToken.IsUsed)
                {
                    var timeSinceLastRequest = DateTime.UtcNow - recentToken.CreatedAt;
                    if (timeSinceLastRequest.TotalMinutes < RESET_REQUEST_COOLDOWN_MINUTES)
                    {
                        var remainingMinutes = RESET_REQUEST_COOLDOWN_MINUTES - (int)timeSinceLastRequest.TotalMinutes;
                        _logger.LogWarning($"User with email {forgotPasswordDto.Email} attempted to request password reset within cooldown period");

                        return new ForgotPasswordResponseDto
                        {
                            Success = false,
                            Message = $"A password reset link was recently sent to your email. Please wait {remainingMinutes} more minute(s) before requesting another."
                        };
                    }
                }

                // Invalidate old tokens
                await _authRepository.InvalidateOldTokensAsync(user.Id);

                // Generate secure token
                var token = GenerateSecureToken();
                var expiresAt = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

                // Save token to database
                await _authRepository.CreatePasswordResetTokenAsync(user.Id, token, expiresAt);

                // Generate reset link with URL-encoded token
                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7280";
                var encodedToken = Uri.EscapeDataString(token);
                var resetLink = $"{baseUrl}/Auth/ResetPassword?token={encodedToken}";

                // ✅ Send email to the employee's email address
                var emailSent = await _emailService.SendPasswordResetEmailAsync(
                    user.Employee.Email,
                    resetLink,
                    user.Employee.FullName
                );

                if (!emailSent)
                {
                    _logger.LogError($"Failed to send password reset email to {user.Employee.Email}");
                    return new ForgotPasswordResponseDto
                    {
                        Success = false,
                        Message = "Failed to send reset email. Please try again later or contact your administrator."
                    };
                }

                _logger.LogInformation($"Password reset email sent successfully to {user.Employee.Email}");

                return new ForgotPasswordResponseDto
                {
                    Success = true,
                    Message = "If an account with that email exists, a password reset link has been sent."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ForgotPasswordAsync: {ex.Message}");
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "An error occurred. Please try again later."
                };
            }
        }

        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                // Validate token
                var resetToken = await _authRepository.GetPasswordResetTokenAsync(resetPasswordDto.Token);

                if (resetToken == null || resetToken.IsUsed)
                {
                    _logger.LogWarning($"Invalid or already used reset token attempted");
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired reset token."
                    };
                }

                if (resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning($"Expired reset token attempted for user {resetToken.UserId}");
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "This reset link has expired. Please request a new one."
                    };
                }

                // Hash the new password
                // ⚠️ IMPORTANT: Use BCrypt in production!
                // Install: dotnet add package BCrypt.Net-Next
                // var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
                var newPasswordHash = HashPassword(resetPasswordDto.NewPassword);

                // Update password
                var passwordUpdated = await _authRepository.UpdatePasswordAsync(
                    resetToken.UserId,
                    newPasswordHash
                );

                if (!passwordUpdated)
                {
                    _logger.LogError($"Failed to update password for user {resetToken.UserId}");
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Failed to update password. Please try again."
                    };
                }

                // Mark token as used
                await _authRepository.MarkTokenAsUsedAsync(resetToken.Id);

                _logger.LogInformation($"Password successfully reset for user {resetToken.UserId}");

                return new ResetPasswordResponseDto
                {
                    Success = true,
                    Message = "Your password has been reset successfully. You can now login with your new password."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ResetPasswordAsync: {ex.Message}");
                return new ResetPasswordResponseDto
                {
                    Success = false,
                    Message = "An error occurred. Please try again later."
                };
            }
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            try
            {
                var resetToken = await _authRepository.GetPasswordResetTokenAsync(token);

                return resetToken != null
                    && !resetToken.IsUsed
                    && resetToken.ExpiresAt > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating reset token: {ex.Message}");
                return false;
            }
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            // Make token URL-friendly by replacing problematic characters
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private string HashPassword(string password)
        {
            // ⚠️ DEMO ONLY - DO NOT USE IN PRODUCTION!
            // In production, use: BCrypt.Net.BCrypt.HashPassword(password)
            // Install NuGet: dotnet add package BCrypt.Net-Next
            return password;
        }
    }
}