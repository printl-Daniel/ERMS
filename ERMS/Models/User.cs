using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign Key
        public int EmployeeId { get; set; }

        // Navigation property
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }

}