using System.ComponentModel.DataAnnotations;

namespace ERMS.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }

        // NEW PROFILE FIELDS
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public int? ManagerId { get; set; }
        public string Role { get; set; }
    }
}