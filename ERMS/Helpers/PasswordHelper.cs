using System.Security.Cryptography;

namespace ERMS.Helpers
{
    public static class PasswordHelper
    {
        public static string GenerateSecureToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public static string GenerateRandomPassword(int length = 12)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string special = "!@#$%";
            const string all = lower + upper + digits + special;

            var password = new char[length];
            var bytes = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            password[0] = lower[bytes[0] % lower.Length];
            password[1] = upper[bytes[1] % upper.Length];
            password[2] = digits[bytes[2] % digits.Length];
            password[3] = special[bytes[3] % special.Length];

            for (int i = 4; i < length; i++)
                password[i] = all[bytes[i] % all.Length];

            rng.GetBytes(bytes);
            return new string(password.OrderBy(x => bytes[Array.IndexOf(password, x)]).ToArray());
        }

        public static string HashPassword(string password)
        {
  
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

    }
}