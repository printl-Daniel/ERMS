using ERMS.Models;
using System.Text.RegularExpressions;
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

        public static string GenerateUsername(string email)
        {
            var local = email.Split('@')[0];
            var cleaned = Regex.Replace(local.ToLower(), @"[^a-z0-9]", "");

            if (cleaned.Length < 3)
                cleaned = cleaned.PadRight(3, 'x');

            if (cleaned.Length > 20)
                cleaned = cleaned[..20];

            return cleaned;
        }
    }
}