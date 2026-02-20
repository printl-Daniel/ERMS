namespace ERMS.DTOs.Auth
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public bool IsFirstLogin { get; set; }
        public string? FirstLoginToken { get; set; }
    }
}