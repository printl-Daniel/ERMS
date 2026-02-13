namespace ERMS.DTOs.Employee
{
    public class CreateEmployeeResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string GeneratedPassword { get; set; }
        public bool EmailSent { get; set; }
    }
}