using System.ComponentModel.DataAnnotations;

namespace ERMS.DTOs.Auth
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
