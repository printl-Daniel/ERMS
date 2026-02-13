using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERMS.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
