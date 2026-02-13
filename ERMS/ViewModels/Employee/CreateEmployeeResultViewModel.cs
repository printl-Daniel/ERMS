namespace ERMS.ViewModels.Employee
{
    public class CreateEmployeeResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string GeneratedPassword { get; set; }
        public bool EmailSent { get; set; }
    }
}