using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Role
{
    public class EditRoleViewModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
