using System.ComponentModel.DataAnnotations;

namespace ERMS.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Soft Delete Properties
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Navigation Properties
        public ICollection<Employee> Employees { get; set; }
    }
}