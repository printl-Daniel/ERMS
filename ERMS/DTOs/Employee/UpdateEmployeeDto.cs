namespace ERMS.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public int? ManagerId { get; set; }
        public string Status { get; set; }
    }
}