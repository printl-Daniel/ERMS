using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } // NEW FIELD

        // Foreign Key
        [Required]
        public int EmployeeId { get; set; }

        // Navigation Property
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}