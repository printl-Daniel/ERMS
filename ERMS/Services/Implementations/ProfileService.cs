using ERMS.Data;
using ERMS.DTOs.Profile;
using ERMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ERMS.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly ERMSDbContext _context;

        public ProfileService(ERMSDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto> GetProfileAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return null;
            }

            return new ProfileDto
            {
                EmployeeId = employee.Id,
                EmployeeNumber = $"EMP{employee.Id:D5}",
                FullName = $"{employee.FirstName} {employee.LastName}",
                Position = employee.Position?.Title,
                Department = employee.Department?.Name,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                DateOfBirth = employee.DateOfBirth ?? DateTime.MinValue, // Handle nullable
                HireDate = employee.HireDate,
                ProfilePicturePath = employee.ProfilePicturePath ?? "/images/default-avatar.png"
            };
        }

        public async Task<UpdateResultDto> UpdatePersonalInfoAsync(UpdatePersonalInfoDto dto)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(dto.EmployeeId);

                if (employee == null)
                {
                    return new UpdateResultDto
                    {
                        Success = false,
                        Message = "Employee not found"
                    };
                }

                employee.Email = dto.Email;
                employee.PhoneNumber = dto.PhoneNumber;
                employee.Address = dto.Address;
                employee.DateOfBirth = dto.DateOfBirth;
                employee.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new UpdateResultDto
                {
                    Success = true,
                    Message = "Personal information updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new UpdateResultDto
                {
                    Success = false,
                    Message = $"Error updating personal information: {ex.Message}"
                };
            }
        }

        public async Task<UpdateResultDto> UpdatePasswordAsync(UpdatePasswordDto dto)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.User)
                    .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);

                if (employee == null || employee.User == null)
                {
                    return new UpdateResultDto
                    {
                        Success = false,
                        Message = "Employee not found"
                    };
                }

                // Verify current password using SHA256
                var hashedCurrentPassword = HashPassword(dto.CurrentPassword);
                if (hashedCurrentPassword != employee.User.PasswordHash)
                {
                    return new UpdateResultDto
                    {
                        Success = false,
                        Message = "Current password is incorrect"
                    };
                }

                // Update password with SHA256 hash
                employee.User.PasswordHash = HashPassword(dto.NewPassword);
                employee.User.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new UpdateResultDto
                {
                    Success = true,
                    Message = "Password updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new UpdateResultDto
                {
                    Success = false,
                    Message = $"Error updating password: {ex.Message}"
                };
            }
        }

        public async Task<UpdateResultDto> UpdateProfilePictureAsync(UpdateProfilePictureDto dto)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(dto.EmployeeId);

                if (employee == null)
                {
                    return new UpdateResultDto
                    {
                        Success = false,
                        Message = "Employee not found"
                    };
                }

                employee.ProfilePicturePath = dto.ProfilePicturePath;
                employee.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new UpdateResultDto
                {
                    Success = true,
                    Message = "Profile picture updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new UpdateResultDto
                {
                    Success = false,
                    Message = $"Error updating profile picture: {ex.Message}"
                };
            }
        }

        // Hash password using SHA256 (same as EmployeeService)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}