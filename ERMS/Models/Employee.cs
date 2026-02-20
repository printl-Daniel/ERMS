using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
        [Required]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        // PROFILE FIELDS
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicturePath { get; set; }

        // AUDIT FIELDS
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // SOFT DELETE
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // FOREIGN KEYS
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public int PositionId { get; set; }
        public int? ManagerId { get; set; }

        // NAVIGATION PROPERTIES
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        [ForeignKey("PositionId")]
        public virtual Position Position { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; }
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        public virtual User User { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}