using ERMS.Models;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Helpers.Mappers
{
    public static class UserMapperHelper
    {
        public static User ToEntity(string username, string hashedPassword, UserRole role, int employeeId) =>
            new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.Now,
                EmployeeId = employeeId
            };

        public static string GenerateUsername(string email) =>
            email.Split('@')[0]
                 .ToLower()
                 .Replace(".", "")
                 .Replace("-", "");
    }
}