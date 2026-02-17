namespace ERMS.DTOs.Employee
{
    public class DeleteEmployeeDto
    {
        public int EmployeeId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}