using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime HireDate { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        // Foreign Keys
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int PositionId { get; set; }

        public int? ManagerId { get; set; }

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position Position { get; set; }

        // Self-referencing for hierarchy
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; }

        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        // One-to-one with User for login
        public virtual User User { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}