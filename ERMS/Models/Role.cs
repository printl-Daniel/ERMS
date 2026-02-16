using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERMS.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
