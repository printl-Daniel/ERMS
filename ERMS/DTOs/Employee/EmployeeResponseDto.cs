namespace ERMS.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }
        public string DepartmentName { get; set; }
        public string PositionTitle { get; set; }
        public string ManagerName { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}